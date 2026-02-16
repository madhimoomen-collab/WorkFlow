using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class User : BaseEntity
    {
        // 'Id' inherited from BaseEntity maps to 'UserId'

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        // Navigation
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class Role : BaseEntity
    {
        // 'Id' inherited from BaseEntity maps to 'RoleId'

        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; } = string.Empty; // Diagram calls this 'Role'

        // Navigation
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class UserRole : BaseEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [ForeignKey("Role")]
        public Guid RoleId { get; set; }

        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}