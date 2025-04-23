using System.Buffers;

namespace MultiTermSearch.Helpers
{
    internal static class SearchValueHelper
    {
        public static SearchValues<string>? SearchVals { get; private set; }

        /// <summary>
        /// Saves a highly optimized set of SearchValues that can be used for finding string/character matches of those values with minimal memory allocation
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <param name="ignoreCase"></param>
        public static void CompileSearchValues(string[] searchTerms, bool ignoreCase)
        {
            SearchVals = SearchValues.Create(searchTerms.AsSpan(), ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        public static void ClearSearchValues()
        {
            SearchVals = null;
        }
    }
}
