using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Tag
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int TagID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string TagName { get; set; }
    }
}
