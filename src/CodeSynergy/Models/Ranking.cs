using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeSynergy.Models
{
    public class Ranking
    {
        [Key]
        [Required]
        [Column(TypeName = "tinyint")]
        public byte RankingCategoryID { get; set; }
        [ForeignKey("RankingCategoryID")]
        public RankingCategory RankingCategory { get; set; }
        [Key]
        [Required]
        [Column(TypeName = "smallint")]
        public short RankingPosID { get; set; }
        [ForeignKey("RankingPosID")]
        public RankingPos RankingPos { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int Score { get; set; }

        public string ProfileImageHtml {
            get
            {
                int dim = RankingPosID > 3 ? 48 : RankingPosID == 3 ? 64 : RankingPosID == 2 ? 96 : 128;
                return "<img src=\"" + User.ProfileImageUrl + "\" class=\"profile-image\" width=" + dim + " height=" + dim + " />";
            }
        }
    }
}
