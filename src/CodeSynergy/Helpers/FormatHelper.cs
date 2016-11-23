using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Helpers
{
    public static class FormatHelper
    {
        public static string GetTimeSinceDate(DateTime? dateTime)
        {
            if (dateTime == null)
                return "Never";

            DateTime dt = (DateTime) dateTime;
            string output;
            DateTime now = DateTime.Now;
            TimeSpan timeSpan = now.Subtract(dt);
            const int daysInYear = 365;
            const float avgDaysInMonth = 30.4166666667f;

            if (timeSpan.Days < daysInYear)
            {
                if (timeSpan.Days < DateTime.DaysInMonth(dt.Year, dt.Month))
                {
                    if (timeSpan.Days < 1)
                    {
                        if (timeSpan.Hours < 1)
                        {
                            if (timeSpan.Minutes < 1)
                            {
                                output = "Just now";
                            }
                            else
                            {
                                output = string.Format(timeSpan.Minutes + " minute{0} ago", timeSpan.Minutes == 1 ? "" : "s");
                            }
                        }
                        else
                        {
                            output = string.Format(timeSpan.Hours + " hour{0} ago", timeSpan.Hours == 1 ? "" : "s");
                        }
                    }
                    else
                    {
                        output = string.Format(timeSpan.Days + " day{0} ago", timeSpan.Days == 1 ? "" : "s");
                    }
                }
                else
                {
                    int months = Math.Max((int)Math.Floor(timeSpan.TotalDays / avgDaysInMonth), 1);
                    output = string.Format(months + " month{0} ago", months == 1 ? "" : "s");
                }
            }
            else
            {
                int years = (int)Math.Floor(timeSpan.TotalDays / daysInYear);
                output = string.Format(years + " years{0} ago", years == 1 ? "" : "s");
            }

            return output;
        }
    }
}
