using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTOs;
using Domain.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IGenericRepository<User> _users;
        private readonly IGenericRepository<Role> _roles;
        private readonly IGenericRepository<UserRole> _userRoles;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IGenericRepository<User> users,
            IGenericRepository<Role> roles,
            IGenericRepository<UserRole> userRoles,
            IConfiguration config,
            ILogger<AuthController> logger)
        {
            _users = users;
            _roles = roles;
            _userRoles = userRoles;
            _config = config;
            _logger = logger;
        }

        // ────────────────────────────────────────────────────────────────────
        // POST api/auth/login
        // ────────────────────────────────────────────────────────────────────

        /// <summary>Authenticate and receive a JWT bearer token.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find user by username (case-insensitive)
            var user = (await _users.FindAsync(
                predicate: u => !u.IsDeleted && u.Username.ToLower() == req.Username.ToLower(),
                includes: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            )).FirstOrDefault();

            if (user is null)
            {
                _logger.LogWarning("Login failed – unknown username: {Username}", req.Username);
                return Unauthorized(new { message = "Identifiant ou mot de passe incorrect." });
            }

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed – wrong password for: {Username}", req.Username);
                return Unauthorized(new { message = "Identifiant ou mot de passe incorrect." });
            }

            // Resolve primary role
            var roleName = user.UserRoles
                .Where(ur => !ur.IsDeleted)
                .Select(ur => ur.Role?.RoleName)
                .FirstOrDefault() ?? "user";

            var token = BuildToken(user, roleName);
            var expiresAt = DateTime.UtcNow.AddHours(GetTokenHours());

            _logger.LogInformation("User {Username} logged in successfully.", user.Username);

            return Ok(new LoginResponse
            {
                Token = token,
                FullName = user.FullName,
                Username = user.Username,
                Role = roleName,
                UserId = user.Id,
                ExpiresAt = expiresAt,
            });
        }

        // ────────────────────────────────────────────────────────────────────
        // POST api/auth/register
        // ────────────────────────────────────────────────────────────────────

        /// <summary>Create a new user account. Admin-only in production.</summary>
        [HttpPost("register")]
        [AllowAnonymous]            // lock down to [Authorize(Roles="admin")] after first setup
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Duplicate username check
            var existing = (await _users.FindAsync(
                u => !u.IsDeleted && u.Username.ToLower() == req.Username.ToLower()
            )).Any();

            if (existing)
                return Conflict(new { message = $"L'identifiant '{req.Username}' est déjà utilisé." });

            // Hash password
            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 12);

            var user = new User
            {
                FullName = req.FullName,
                Username = req.Username.Trim(),
                PasswordHash = hash,
            };

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            // Assign role
            if (!string.IsNullOrWhiteSpace(req.RoleName))
            {
                var role = (await _roles.FindAsync(
                    r => !r.IsDeleted && r.RoleName.ToLower() == req.RoleName.ToLower()
                )).FirstOrDefault();

                if (role is not null)
                {
                    await _userRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
                    await _userRoles.SaveChangesAsync();
                }
            }

            _logger.LogInformation("New user registered: {Username}", user.Username);

            return CreatedAtAction(nameof(Me), null, new RegisterResponse
            {
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Message = "Compte créé avec succès.",
            });
        }

        // ────────────────────────────────────────────────────────────────────
        // GET api/auth/me
        // ────────────────────────────────────────────────────────────────────

        /// <summary>Returns the currently authenticated user's profile.</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(idClaim, out var userId))
                return Unauthorized();

            var user = await _users.GetAsync(
                predicate: u => u.Id == userId && !u.IsDeleted,
                includes: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            );

            if (user is null) return NotFound();

            var roleName = user.UserRoles
                .Where(ur => !ur.IsDeleted)
                .Select(ur => ur.Role?.RoleName)
                .FirstOrDefault() ?? "user";

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Username,
                Role = roleName,
                user.CreatedAt,
            });
        }

        // ────────────────────────────────────────────────────────────────────
        // PUT api/auth/change-password
        // ────────────────────────────────────────────────────────────────────

        /// <summary>Change password for the currently authenticated user.</summary>
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(idClaim, out var userId))
                return Unauthorized();

            var user = await _users.GetAsync(u => u.Id == userId && !u.IsDeleted);
            if (user is null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Mot de passe actuel incorrect." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword, workFactor: 12);
            await _users.UpdateAsync(user);
            await _users.SaveChangesAsync();

            _logger.LogInformation("Password changed for user {Username}", user.Username);
            return Ok(new { message = "Mot de passe modifié avec succès." });
        }

        // ────────────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────────────

        private string BuildToken(User user, string role)
        {
            var jwtCfg = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCfg["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(GetTokenHours());

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,            user.Username),
                new Claim(ClaimTypes.GivenName,       user.FullName),
                new Claim(ClaimTypes.Role,            role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtCfg["Issuer"],
                audience: jwtCfg["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private double GetTokenHours()
        {
            var h = _config["Jwt:ExpiresHours"];
            return double.TryParse(h, out var v) ? v : 8;
        }
    }
}