using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CodeSynergy.Models
{
    public class RankingPos
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "smallint")]
        public short RankingPosID { get; set; }
        public ICollection<Ranking> Rankings { get; set; }

        [NotMapped]
        public string RankingPosHtml {
            get
            {
                string rankingPosHtml;

                if (RankingPosID <= 3)
                {
                    switch (RankingPosID)
                    {
                        case 1:
                            rankingPosHtml = "<span class=\"fa-stack fa-3x\"><strong class=\"fa-stack-1x ranking-position\" style=\"color: #fff; text-shadow: 2px 2px #888; margin-top: -8px; margin-left: 2px;\">" + RankingPosID + "</strong><i class=\"fa fa-trophy fa-stack-3x\" style=\"color: #ffd700; font-size: 96px\"></i></span>";
                            break;
                        case 2:
                            rankingPosHtml = "<span class=\"fa-stack fa-3x\"><strong class=\"fa-stack-1x ranking-position\" style=\"color: #fff; text-shadow: 2px 2px #888; font-size: 80%; margin-top: -12px;\">" + RankingPosID + "</strong><i class=\"fa fa-trophy fa-stack-3x\" style=\"color: #c0c0c0; font-size: 84px\"></i></span>";
                            break;
                        default:
                            rankingPosHtml = "<span class=\"fa-stack fa-3x\"><strong class=\"fa-stack-1x ranking-position\" style=\"color: #fff; text-shadow: 2px 2px #888; font-size: 65%; margin-top: -16px;\">" + RankingPosID + "</strong><i class=\"fa fa-trophy fa-stack-3x\" style=\"color: #cd7f32; font-size: 72px\"></i></span>";
                            break;
                    }
                }
                else if (RankingPosID <= 10)
                {
                    rankingPosHtml = "<h4 class=\"ranking-position\">" + RankingPosID + "</h4>";
                }
                else
                {
                    rankingPosHtml = "<span class=\"ranking-position\">" + RankingPosID + "</span>";
                }

                return rankingPosHtml;
            }
        }
    }
}
