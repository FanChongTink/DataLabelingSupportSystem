using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.Entities
{
    public class ReviewComment
    {
        [Key]
        public int CommentId { get; set; }

        public int DataItemId { get; set; }

        public int ReviewerId { get; set; }

        [Required]
        public required string Content { get; set; }

        [MaxLength(50)]
        public string? ErrorType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("DataItemId")]
        public DataItem? DataItem { get; set; }

        [ForeignKey("ReviewerId")]
        public User? Reviewer { get; set; }
    }
}