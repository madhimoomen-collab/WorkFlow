using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Domain.Commands;
using Domain.DTOs;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using API.Helpers;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(IMediator mediator, IConfiguration configuration, IMapper mapper)
        {
            _mediator = mediator;
            _configuration = configuration;
            _mapper = mapper;
        }

        /// <summary>Register a new user</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existing = await _mediator.Send(new GetGenericQuery<User>(
                condition: u => u.Email == request.Email && !u.IsDeleted
            ));
            if (existing is not null)
                return Conflict(new { message = "A user with this email already exists." });

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = PasswordHelper.HashPassword(request.Password)
            };

            var result = await _mediator.Send(new AddGenericCommand<User>(user));
            var dto = _mapper.Map<UserDto>(result);
            return CreatedAtAction(null, new { id = dto.Id }, dto);
        }

        /// <summary>Login and receive a JWT token</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _mediator.Send(new GetGenericQuery<User>(
                condition: u => u.Email == request.Email && !u.IsDeleted
            ));
            if (user is null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(new { token = GenerateToken(user) });
        }

        private string GenerateToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiryMinutes"]!)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public record RegisterRequest(string FullName, string Email, string Password);
    public record LoginRequest(string Email, string Password);
}
