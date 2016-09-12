using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class UntrustedURLPattern
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int PatternID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string AddedByUserID { get; set; }
        [ForeignKey("AddedByUserID")]
        public ApplicationUser AddedByUser { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string LastUpdatedByUserID { get; set; }
        [ForeignKey("LastUpdatedByUserID")]
        public ApplicationUser LastUpdatedByUser { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string RemovedByUserID { get; set; }
        [ForeignKey("RemovedByUserID")]
        public ApplicationUser RemovedByUser { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string PatternText { get; set; }
    }
}
