using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.Entities
{
    public class LabelClass
    {
        [Key]
        public int LabelClassId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(10)]
        public string ColorCode { get; set; } = "#FFFFFF";

        // Loại hình: "BoundingBox", "Polygon"...
        [MaxLength(20)]
        public string? ShapeType { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }
    }
}
