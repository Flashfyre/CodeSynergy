using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Region
    {
        [Key]
        [Required]
        [Column("region_id", Order = 1, TypeName = "varchar(4)")]
        public string RegionID { get; set; }
        [Required]
        [Column("country_iso_code", Order = 2, TypeName = "varchar(2)")]
        public string ISO { get; set; }
        [ForeignKey("ISO")]
        public Country Country { get; set; }
        [Required]
        [Column("region_name", Order = 3, TypeName = "varchar(40)")]
        public string RegionName { get; set; }
    }
}
