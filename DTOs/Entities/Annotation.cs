using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.Entities
{
    public class Annotation
    {
        [Key]
        public int AnnotationId { get; set; }

        public int DataItemId { get; set; }

        public int LabelClassId { get; set; }

        // Lưu tọa độ dạng JSON string để linh hoạt (VD: {x:10, y:10, w:100, h:100})
        [Required]
        public required string CoordinatesJson { get; set; }

        public bool IsAiGenerated { get; set; } = false;

        [ForeignKey("DataItemId")]
        public DataItem? DataItem { get; set; }

        [ForeignKey("LabelClassId")]
        public LabelClass? LabelClass { get; set; }
    }
}