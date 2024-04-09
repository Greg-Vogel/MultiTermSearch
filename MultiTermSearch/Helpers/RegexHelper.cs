using MultiTermSearch.Classes;
using System.Text.RegularExpressions;

namespace MultiTermSearch.Helpers;

internal static class RegexHelper
{
    internal static SearchRegex[] CompiledRegex { get; set; } = null!;

    internal static async Task CompileRegexQueries(SearchInputs inputs)
    {
        await Task.Run(() =>
        {
            CompiledRegex = inputs.SearchTerms
                .AsParallel()
                .WithDegreeOfParallelism(10)
                .Select(t => new SearchRegex(searchPattern: inputs.Options_MatchWholeWord ? $@"\b({t})\b" : t
                                            , baseSearchTerm: t
                                            , options: inputs.Options_IgnoreCase ? RegexOptions.Compiled | RegexOptions.IgnoreCase : RegexOptions.Compiled))
                .ToArray();
        });
    }

    internal static void ClearCompiledQueries()
    {
        CompiledRegex = Array.Empty<SearchRegex>();
    }

}
