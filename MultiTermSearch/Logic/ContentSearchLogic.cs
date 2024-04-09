﻿using MultiTermSearch.Classes;
using MultiTermSearch.Helpers;

namespace MultiTermSearch.Logic;

internal class ContentSearchLogic
{
    public static async Task<FileResult?> ScanFileContents(string filePath, SearchInputs inputs, CancellationToken cancelToken)
    {
        return await Task.Run(() =>
        {
            // open the file with a stream reader so we can stream lines out one at a time and not kill memory usage
            using var streamReader = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read));

            // loop though the file line by line
            FileResult? result = null;
            int lineNum = 0;
            while (streamReader.Peek() >= 0 && !cancelToken.IsCancellationRequested)
            {
                lineNum += 1;
                LineResult? lineResult = null;

                // read the line
                //   Since we already checked with Peek above, it should never be null
                var line = streamReader.ReadLine()!;

                if (cancelToken.IsCancellationRequested)
                    return null;

                // check the line against each pre-compiled regex to see if we find a match
                foreach (var regex in RegexHelper.CompiledRegex)
                {
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

                if (cancelToken.IsCancellationRequested)
                    return null;


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
        });
    }
}
