using MultiTermSearch.Classes;
using MultiTermSearch.Helpers;
using System.Buffers;

namespace MultiTermSearch.SearchLogic;

internal static class FileContentSearchLogic
{
    private const int _fileLineBatchSize = 1_000;

    /// <summary>
    /// Scans a files contents by reading it via a stream line by line analyzing if each line meets the criteria.
    /// </summary>
    /// <param name="inputs"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    public static async Task<FileResult> ScanFileContents(FileResult result, SearchInputs inputs, string filePath, CancellationToken cancelToken)
    {
        // if another search was run on the file we might already have a FileResult obj created
        //    if not... initialize it now
        if (result is null)
            result = new FileResult(filePath);

        return await Task.Run(async () => 
        {
            try
            {
                // open the file with a stream reader so we can stream lines out one at a time and not kill memory usage
                using var streamReader = new StreamReader(File.OpenRead(filePath), bufferSize: 4096);

                // loop though the file line by line
                //    add each line to a batch of lines
                //    trigger an async process to check each line in that batch
                //    wait for all batches to complete
                //    finally aggegate all of the batch results back into a single result
                string? line;

                List<BatchLine> batch = new();
                List<Task<BatchResult?>> batchTasks = new();
                for (int i = 1; !streamReader.EndOfStream; i++)
                {
                    line = await streamReader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (cancelToken.IsCancellationRequested)
                        return result;

                    // if we have a full batch, send it off for scanning
                    batch.Add(new BatchLine() { LineNum = i, Line = line });
                    if (batch.Count % _fileLineBatchSize == 0)
                    {
                        var procBatch = batch.ToArray();
                        batchTasks.Add(Task.Run(() => SearchBatchOfLines(procBatch, inputs, cancelToken)));
                        batch = new();
                        continue;
                    }
                }

                // we reached the end of the file and just have a partial batch...
                //   send that partial off for scanning
                if (batch.Any())
                {
                    var procBatch = batch.ToArray();
                    batchTasks.Add(Task.Run(() => SearchBatchOfLines(procBatch, inputs, cancelToken)));
                    batch = new();
                }

                if (cancelToken.IsCancellationRequested)
                    return result;

                // we sent all of the line batches off for scanning... wait for them all to be done
                Task.WaitAll(batchTasks, cancelToken);

                if (cancelToken.IsCancellationRequested)
                    return result;

                // loop through all completed tasks and aggregate the results back together
                var tempLines = new List<LineResult>();
                foreach (var bt in batchTasks)
                {
                    var bResult = bt.Result;
                    if (bt.IsCompleted && bResult is not null)
                    {
                        if (bResult.LineResults.Any())
                            result.AddLineResults(bResult.LineResults);
                        if (!string.IsNullOrWhiteSpace(bResult.Error))
                            result.ErrorMessage = bResult.Error;
                        continue;
                    }
                    if (bt.IsFaulted)
                    {
                        result.ErrorMessage = bt.Exception.Message;
                    }
                }

                // we did not find any lines... just stop here
                if (!result.HasResult)
                    return result;


                // If the user specified that the file itself must contain all search terms to be included....
                //    make sure our FileResult has at least one match for each term
                //    If not... just return the empty result
                if (inputs.Filters_FileContainsAll && result.FoundTerms.Count != inputs.SearchTerms.Count())
                {
                    result.LineResults.Clear();
                    return result;
                }


                // We made it to the end with a valid file result... return it to the caller
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                return result;
            }
        });
    }


    private static BatchResult? SearchBatchOfLines(BatchLine[] lines, SearchInputs inputs, CancellationToken cancelToken)
    {
        var batchResult = new BatchResult();


        // Loop through each line of the batch
        //    check it against each search term individually to pull out all matching indexes
        //    once you have the list of indexes, add them to the result line result and move on to the next search term
        LineResult? lineResult;
        List<int> matchIndexes = new List<int>();
        var comparisonOption = inputs.Options_IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        int trimmedLength = 0;
        int idx;
        List<int> wordMatches;
        foreach (var lineRecord in lines)
        {
            var fullLine = lineRecord.Line.AsSpan();

            // do a quick and dirty check to see if a line contains any of the values we are looking for...
            //   if it does, then we will do further work to identify the positions of all matches
            //   I know scanning more than once seems not ideal...
            //      but using the SearchValues class, it cuts down scanning time by nearly 75% in our benchmarking
            //  this doesnt handle 'WholeWord' searches but it is still faster than nothing
            if (!fullLine.ContainsAny(SearchValueHelper.SearchVals!))
                continue;

            // now loop through each search term and find all of its locations in this line
            //    we will do this by:
            //     1. finding the first instance, recording it
            //     2. slicing that first part of the string containing the first character of our searchterm out
            //     3. search for the next instance, and repeat until we find no more instances
            //  doing all of this using spans so we are not allocating strings for all the slicing
            lineResult = null;
            foreach (var searchTerm in inputs.SearchTerms)
            {
                var term = searchTerm.AsSpan();
                var line = fullLine;
                trimmedLength = 0;
                wordMatches = new List<int>();

                idx = line.IndexOf(term, comparisonOption);
                while (idx != -1)
                {
                    // if the user chose to do a wholeword match
                    //   if the leading or trailing characters are a number or letter we need to just move on
                    //   slice the stirng and move on
                    if (inputs.Options_MatchWholeWord
                        && 
                        (
                            (idx > 0 && char.IsLetterOrDigit(line[idx-1])) // leading char check
                            || (idx + term.Length < line.Length && char.IsLetterOrDigit(line[idx + term.Length]))) // trailling char check
                        )
                    {
                        line = line.Slice(idx + 1);
                        trimmedLength += idx + 1;
                        idx = line.IndexOf(term, comparisonOption);
                        continue;
                    }

                    // we made it here means we got a valid match!
                    //   record the index and move on
                    wordMatches.Add(idx + trimmedLength);
                    line = line.Slice(idx + 1);
                    trimmedLength += idx + 1;
                    idx = line.IndexOf(term, comparisonOption);
                }


                // we finished checking the line for this term...
                //   if we found anything, add it to our result now
                if (wordMatches.Any())
                {
                    if (lineResult is null)
                        lineResult = new LineResult() { LineNumber = lineRecord.LineNum, Line = lineRecord.Line! };

                    lineResult.AddTermResults(searchTerm, wordMatches);
                }
            }

            if (lineResult is null)
                continue;

            // If the user specified that a line must contain all search terms
            //    make sure the line has a found term count equal to the compiled regex list passed in
            if (inputs.Filters_LineContainsAll && lineResult.FoundTerms.Count != inputs.SearchTerms.Length)
                continue;

            if (lineResult is not null)
                batchResult.LineResults.Add(lineResult);
        }

        return batchResult;
    }
}
