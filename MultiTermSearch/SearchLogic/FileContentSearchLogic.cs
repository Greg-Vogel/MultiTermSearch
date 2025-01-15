using MultiTermSearch.Classes;
using MultiTermSearch.Helpers;

namespace MultiTermSearch.SearchLogic;

internal static class FileContentSearchLogic
{
    /// <summary>
    /// Scans a files contents by reading it via a stream line by line analyzing if each line meets the criteria.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    public static async Task<FileResult> ScanFileContents(FileResult result, string filePath, SearchInputs inputs, CancellationToken cancelToken)
    {
        if (result is null)
            result = new FileResult(filePath);

        return await Task.Run(() => 
        {
            try
            {
                // open the file with a stream reader so we can stream lines out one at a time and not kill memory usage
                using var streamReader = new StreamReader(File.OpenRead(filePath));

                // loop though the file line by line
                int lineNum = 0;
                while (streamReader.Peek() >= 0 && !cancelToken.IsCancellationRequested)
                {
                    lineNum += 1;
                    LineResult? lineResult = null;

                    // read the line
                    //   Since we already checked with Peek above, it should never be null
                    var line = streamReader.ReadLine()!;

                    if (cancelToken.IsCancellationRequested)
                        return result;

                    // check the line against each pre-compiled regex to see if we find a match
                    foreach (var regex in RegexHelper.CompiledRegex)
                    {
                        // get the matches in this line of text for this search term
                        var matches = regex.Matches(line);

                        // if we didnt find any matches for this search term, move on
                        if (!matches.Any())
                            continue;

                        if (cancelToken.IsCancellationRequested)
                            return result;

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

                    if (cancelToken.IsCancellationRequested)
                        return result;


                    // if we did not find anything for this line... move on
                    if (lineResult is null)
                        continue;


                    // If the user specified that a line must contain all search terms
                    //    make sure the line has a found term count equal to the compiled regex list passed in
                    if (inputs.Filters_LineContainsAll && lineResult.FoundTerms.Count != RegexHelper.CompiledRegex.Length)
                        continue;


                    // We made it here... so we have a valid line result to include... so add it to our FileResult
                    result.AddLineResult(lineResult);
                }


                // we did not find any lines... just stop here
                if (!result.HasResult)
                    return result;


                // If the user specified that the file itself must contain all search terms to be included....
                //    make sure our FileResult has at least one match for each term
                //    If not... just return null
                if (inputs.Filters_FileContainsAll && result.FoundTerms.Count != RegexHelper.CompiledRegex.Length)
                    return result;


                // We made it to the end with a valid file result... return it to the caller
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                return result;
            }
        });
    }
}
