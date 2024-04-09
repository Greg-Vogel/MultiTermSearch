using MultiTermSearch.Classes;
using MultiTermSearch.Helpers;

namespace MultiTermSearch.Logic;

internal class ContentSearchLogic
{
    public static async Task<FileResult?> ScanFileContents(string filePath, SearchInputs inputs, CancellationToken cancelToken)
    {
        var fileLines = await File.ReadAllLinesAsync(filePath);

        // For each regex, scans the file and gets a set of matches for that regex
        FileResult? result = null;
        int lineNum = 0;
        foreach (string line in fileLines)
        {
            lineNum += 1;
            LineResult? lineResult = null;

            if (cancelToken.IsCancellationRequested)
                return null;

            foreach (var regex in RegexHelper.CompiledRegex)
            {
                if (cancelToken.IsCancellationRequested)
                    return null;

                // get the matches in this line of text for this search term
                var matches = regex.Matches(line);

                if (cancelToken.IsCancellationRequested)
                    return null;

                // if we didnt find any matches for this search term, move on
                if (!matches.Any())
                    continue;

                // if this is the first matching term for this line, create a new result obj for it
                if (lineResult == null)
                    lineResult = new LineResult() { LineNumber = lineNum, Line = line };

                // Add the matches for this term to the line
                lineResult.AddTermResult(new TermResult()
                {
                    Term = regex.SearchTerm,
                    IndexOfMatches = matches.Select(m => m.Index).ToArray()
                });
            }

            // if we found a result for this line... add it to the results
            if (lineResult != null)
            {
                // If the user specified that a line must contain all search terms
                //    make sure the line has a found term count equal to the compiled regex list passed in
                if (inputs.Filters_LineContainsAll)
                {
                    if (lineResult.FoundTerms.Count != RegexHelper.CompiledRegex.Length)
                        continue;
                }

                // if this is the first line to have a result, instantiate the file result obj to hold it
                if (result == null)
                    result = new FileResult(filePath);

                result.AddLineResult(lineResult);
            }
        }

        // if we have a result but it does not contain all search terms as requested by user... just return null
        if (result != null)
        {
            if (inputs.Filters_FileContainsAll)
            {
                if (result.FoundTerms.Count != RegexHelper.CompiledRegex.Length)
                    return null;
            }
        }

        return result;
    }
}
