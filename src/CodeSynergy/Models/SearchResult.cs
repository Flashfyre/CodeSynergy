using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeSynergy.Models
{
    public class SearchResult<T> where T : ISearchable
    {
        public SearchResult(T Object, string Query) {
            this.Object = Object;
            this.Query = Query;

            if (Query != null)
            {
                string query = WebUtility.HtmlEncode(Query.ToLower());

                Occurences = 0;

                SearchTextHtml = new string[Object.GetSearchText().Count()];

                for (int s = 0; s < SearchTextHtml.Count(); s++)
                {
                    int cursor, i;
                    string searchTextHtml, searchText = (searchTextHtml = Object.GetSearchText()[s]).ToLower();
                    List<Tuple<string, int, int>> tagList = new List<Tuple<string, int, int>>();
                    List<Tuple<int, int>> posList = new List<Tuple<int, int>>();
                    Regex tagPattern = new Regex("</?\\w+((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[\\^'\">\\s]+))?)+\\s*|\\s*)/?>", RegexOptions.IgnoreCase), charPattern = new Regex(".");
                    MatchCollection matches = tagPattern.Matches(searchText);

                    foreach (Match m in matches)
                    {
                        cursor = m.Index;
                        searchText = (cursor != 0 ? searchText.Substring(0, cursor) : "") + charPattern.Replace(m.Value, " ") + ((cursor = cursor + m.Length) <= searchText.Length ? searchText.Substring(cursor) : "");
                    }

                    cursor = 0;

                    while ((cursor = searchText.IndexOf(query, cursor)) > -1)
                    {
                        int startPos = cursor, endPos = cursor = (cursor + query.Length);
                        posList.Add(new Tuple<int, int>(startPos, endPos));
                    }

                    foreach (Match m in matches)
                    {
                        cursor = m.Index;
                        searchText = (cursor != 0 ? searchText.Substring(0, cursor) : "") + m.Value + ((cursor = cursor + m.Length) <= searchText.Length ? searchText.Substring(cursor) : "");
                    }

                    for (i = 0; i < posList.Count(); i++)
                    {
                        int startPos = posList[i].Item1 + (i * 31), endPos = posList[i].Item2 + (i * 31);
                        searchTextHtml = (startPos != 0 ? searchTextHtml.Substring(0, startPos) : "") + "<span class=\"highlight\">" + searchTextHtml.Substring(startPos, endPos - startPos) + "</span>" + (endPos <= searchTextHtml.Length ? searchTextHtml.Substring(endPos) : "");
                    }

                    SearchTextHtml[s] = searchTextHtml;
                    Occurences += i;
                }
            }
            else
                SearchTextHtml = Object.GetSearchText();
        }

        public T Object { get; set; }
        public string Query { get; set; }
        public int Occurences { get; set; }
        public string[] SearchTextHtml { get; set; }

        public static IEnumerable<SearchResult<T>> GetSearchResults(IEnumerable<T> Objects, string Query)
        {
            List<SearchResult<T>> results = new List<SearchResult<T>>();

            foreach (T o in Objects)
            {
                SearchResult<T> result = new SearchResult<T>(o, Query);
                if (Query == null || result.Occurences > 0)
                    results.Add(result);
            }

            return results.OrderByDescending(r => r.Occurences);
        }
    }
}
