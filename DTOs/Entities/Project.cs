using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.Entities
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        public int ManagerId { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public string? Instruction { get; set; } // Hướng dẫn gán nhãn

        [Required]
        [MaxLength(20)]
        public required string DataType { get; set; } // "Image", "Text"...

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ManagerId")]
        public User? Manager { get; set; }

        public ICollection<LabelClass>? LabelClasses { get; set; }
        public ICollection<DataItem>? DataItems { get; set; }
    }
}