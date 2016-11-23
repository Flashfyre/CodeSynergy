using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class RankingCategory
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "tinyint")]
        public byte RankingCategoryID { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string RankingName { get; set; }
    }

    public enum EnumRankingCategory
    {
        Overall_Rank,
        Reputation,
        Question_Score,
        Answer_Score,
        Comment_Score,
        Total_Score,
        Best_Answers,
        Question_Stars,
        Questions_Posted,
        Answers_Posted,
        Comments_Posted,
        Total_Posts
    }

    public static class RankingCategoryHelper
    {
        public static string DisplayName(this EnumRankingCategory rankingCategory)
        {
            return Enum.GetName(typeof(EnumRankingCategory), rankingCategory).Replace("_", " ");
        }

        public static string GetFormattedUserScore(this EnumRankingCategory rankingCategory, ApplicationUser user, Ranking ranking = null)
        {
            string userScore;

            if (ranking != null)
            {
                string rankingPosString = ranking != null ? "<a href=\"/Ranking/" + ((int)(Math.Ceiling(ranking.RankingPosID * 0.1D))) + "\">#" + ranking.RankingPosID + "</a>" : "";
                userScore = rankingCategory != EnumRankingCategory.Overall_Rank ? ranking.Score.ToString() + " (" + rankingPosString + ")" : rankingPosString;
            }
            else
            {
                switch (rankingCategory)
                {
                    case EnumRankingCategory.Reputation:
                        userScore = user.Reputation.ToString();
                        break;
                    case EnumRankingCategory.Question_Score:
                        userScore = user.QuestionScore.ToString();
                        break;
                    case EnumRankingCategory.Answer_Score:
                        userScore = user.AnswerScore.ToString();
                        break;
                    case EnumRankingCategory.Comment_Score:
                        userScore = user.CommentScore.ToString();
                        break;
                    case EnumRankingCategory.Total_Score:
                        userScore = user.TotalScore.ToString();
                        break;
                    case EnumRankingCategory.Best_Answers:
                        userScore = user.BestAnswerCount.ToString();
                        break;
                    case EnumRankingCategory.Question_Stars:
                        userScore = user.StarCount.ToString();
                        break;
                    case EnumRankingCategory.Questions_Posted:
                        userScore = user.QuestionsPosted.ToString();
                        break;
                    case EnumRankingCategory.Answers_Posted:
                        userScore = user.AnswersPosted.ToString();
                        break;
                    case EnumRankingCategory.Comments_Posted:
                        userScore = user.CommentsPosted.ToString();
                        break;
                    case EnumRankingCategory.Total_Posts:
                        userScore = user.TotalPosts.ToString();
                        break;
                    default:
                        userScore = "N/A";
                        break;
                }
            }

            return userScore;
        }

        public static string GetFormattedScore(this EnumRankingCategory rankingCategory, int Score)
        {
            string unit;

            switch (rankingCategory)
            {
                case EnumRankingCategory.Overall_Rank:
                    unit = " #" + Score + " ";
                    break;
                case EnumRankingCategory.Reputation:
                    unit = Score + " Rep";
                    break;
                case EnumRankingCategory.Best_Answers:
                    unit = Score + " Answers";
                    break;
                case EnumRankingCategory.Question_Stars:
                    unit = Score + " Stars";
                    break;
                case EnumRankingCategory.Questions_Posted:
                    unit = Score + " Questions";
                    break;
                case EnumRankingCategory.Answers_Posted:
                    unit = Score + " Answers";
                    break;
                case EnumRankingCategory.Comments_Posted:
                    unit = Score + " Comments";
                    break;
                case EnumRankingCategory.Total_Posts:
                    unit = Score + " Posts";
                    break;
                default:
                    unit = Score + " Points";
                    break;
            }

            return unit;
        }
    }
}
