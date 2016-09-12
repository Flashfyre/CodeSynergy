using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class Country
    {
        [Key]
        [Required]
        [Column("country_iso_code", Order = 1, TypeName = "varchar(2)")]
        public string ISO { get; set; }
        [Required]
        [Column("continent_code", Order = 2, TypeName = "varchar(2)")]
        public string ContinentCode { get; set; }
        [Required]
        [Column("continent_name", Order = 3, TypeName = "varchar(40)")]
        public string ContinentName { get; set; }
        [Required]
        [Column("country_name", Order = 4, TypeName = "varchar(40)")]
        public string CountryName { get; set; }
    }
}
