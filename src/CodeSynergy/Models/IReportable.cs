using CodeSynergy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public interface IReportable
    {
        EnumReportType GetReportType();
        string GetReportableItemID();
        string GetReportableItemDescription();
        string GetReportableItemLinkHtml(string returnUrl);
    }
}
