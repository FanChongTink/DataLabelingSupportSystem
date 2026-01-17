using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.Entities
{
    public class Annotation
    {
        [Key]
        public int Id { get; set; }

        public int AssignmentId { get; set; }

        [ForeignKey("AssignmentId")]
        public virtual Assignment Assignment { get; set; } = null!;

        public int LabelClassId { get; set; }

        [ForeignKey("LabelClassId")]
        public virtual LabelClass LabelClass { get; set; } = null!;

        [Required]
        public string DataJson { get; set; } = "{}";
    }
}