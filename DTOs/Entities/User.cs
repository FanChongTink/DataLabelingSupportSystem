using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DTOs.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Role { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Project>? ManagedProjects { get; set; } 
        public ICollection<DataItem>? AssignedDataItems { get; set; } 
        public ICollection<DataItem>? ReviewedDataItems { get; set; } 
    }
}
