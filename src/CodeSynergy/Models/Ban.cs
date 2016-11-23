using CodeSynergy.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    // Class representing the Bans table
    public class Ban
    {
        [Key]
        [Required]
        [Column(TypeName = "int")]
        public int BanID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string BannedUserID { get; set; }
        [ForeignKey("BannedUserID")]
        public ApplicationUser BannedUser { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string BanningUserID { get; set; }
        [ForeignKey("BanningUserID")]
        public ApplicationUser BanningUser { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string BanReason { get; set; }
        [Required]
        [Column(TypeName ="tinyint")]
        public byte BanTerm { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime BanDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? BanLiftDate { get; set; }
        [NotMapped]
        public bool Active
        {
            get
            {
                return DateTime.Now < BanLiftDate || BanDate == BanLiftDate;
            }
        }
    }

    public enum EnumBanTerm
    {
        OneDay = 0,
        ThreeDays = 1,
        OneWeek = 2,
        OneMonth = 3,
        Perm = 4
    };

    // Extension for EnumBanTerm to help with display and access of all values
    public static class BanTermHelper
    {
        public static DateTime DateTimeOffset(this EnumBanTerm banTerm, DateTime dateTime)
        {
            switch (banTerm)
            {
                case EnumBanTerm.OneDay:
                    dateTime = dateTime.AddDays(1);
                    break;
                case EnumBanTerm.ThreeDays:
                    dateTime = dateTime.AddDays(3);
                    break;
                case EnumBanTerm.OneWeek:
                    dateTime = dateTime.AddDays(7);
                    break;
                case EnumBanTerm.OneMonth:
                    dateTime = dateTime.AddMonths(1);
                    break;
            }

            return dateTime;
        }

        public static string DisplayName(this EnumBanTerm banTerm)
        {
            string displayName;
            switch (banTerm)
            {
                case EnumBanTerm.OneDay:
                    displayName = "1 Day";
                    break;
                case EnumBanTerm.ThreeDays:
                    displayName = "3 Days";
                    break;
                case EnumBanTerm.OneWeek:
                    displayName = "1 Week";
                    break;
                case EnumBanTerm.OneMonth:
                    displayName = "1 Month";
                    break;
                default:
                    displayName = "Permanent";
                    break;
            }

            return displayName;
        }

        public static List<EnumBanTerm> Values()
        {
            return new List<EnumBanTerm>() { EnumBanTerm.OneDay, EnumBanTerm.ThreeDays, EnumBanTerm.OneWeek, EnumBanTerm.OneMonth, EnumBanTerm.Perm };
        }
    }
}
