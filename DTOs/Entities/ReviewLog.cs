using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.Entities
{
    public class ReviewLog
    {
        [Key]
        public int Id { get; set; }

        public int AssignmentId { get; set; }

        [ForeignKey("AssignmentId")]
        public virtual Assignment Assignment { get; set; } = null!;

        public string ReviewerId { get; set; } = string.Empty;

        [ForeignKey("ReviewerId")]
        public virtual User Reviewer { get; set; } = null!;

        public string Decision { get; set; } = "Approve"; // Approve, Reject
        public string? Comment { get; set; }
        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
    }
}