using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.Entities
{
    public class DataItem
    {
        [Key]
        public int DataItemId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public required string FileName { get; set; }

        [Required]
        public required string StorageUrl { get; set; }

        // Status: "New", "Assigned", "Submitted", "Approved", "Rejected"
        [MaxLength(20)]
        public string Status { get; set; } = "New";

        public int? AnnotatorId { get; set; }
        public int? ReviewerId { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        [ForeignKey("AnnotatorId")]
        public User? Annotator { get; set; }

        [ForeignKey("ReviewerId")]
        public User? Reviewer { get; set; }

        public ICollection<Annotation>? Annotations { get; set; }
        public ICollection<ReviewComment>? ReviewComments { get; set; }
    }
}