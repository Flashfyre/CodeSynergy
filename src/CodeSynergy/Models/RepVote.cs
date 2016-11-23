using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class RepVote
    {
        [Key]
        [Required]
        [Column(Order = 0, TypeName = "nvarchar(450)")]
        public string VoteeUserID { get; set; }
        [ForeignKey("VoteeUserID")]
        public ApplicationUser VoteeUser { get; set; }
        [Key]
        [Required]
        [Column(Order = 1, TypeName = "nvarchar(450)")]
        public string VoterUserID { get; set; }
        [ForeignKey("VoterUserID")]
        public ApplicationUser VoterUser { get; set; }
        [Column(Order = 2, TypeName = "bit")]
        public bool Vote { get; set; }
    }
}
