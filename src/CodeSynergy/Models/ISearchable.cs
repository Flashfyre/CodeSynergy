using CodeSynergy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public interface ISearchable
    {
        string[] GetSearchText();
    }
}
