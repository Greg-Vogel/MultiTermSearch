using MultiTermSearch.Classes;
using MultiTermSearch.Helpers;

namespace MultiTermSearch.SearchLogic;

internal static class FileContentSearchLogic
{
    private const int _fileLineBatchSize = 100;

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
                foreach (var bt in batchTasks)
                {
                    var bResult = bt.Result;
                    if (bt.IsCompleted && bResult is not null)
                    {
                        if (bResult.LineResults.Any())
                            result.LineResults.AddRange(bResult.LineResults);
                        if (!string.IsNullOrWhiteSpace(bResult.Error))
                            result.Error = bResult.Error;
                        continue;
                    }
                    if (bt.IsFaulted)
                    {
                        result.Error = bt.Exception.Message;
                    }
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


    private static BatchResult? SearchBatchOfLines(BatchLine[] lines, SearchInputs inputs, CancellationToken cancelToken)
    {
        var batchResult = new BatchResult();

        LineResult? lineResult;
        foreach (var lineRecord in lines)
        {
            lineResult = null;

            // check the line against each pre-compiled regex to see if we find a match
            foreach (var regex in RegexHelper.CompiledRegex)
            {
                if (cancelToken.IsCancellationRequested)
                    return null;

                // get the matches in this line of text for this search term
                var matches = regex.Matches(lineRecord.Line!);

                if (cancelToken.IsCancellationRequested)
                    return null;

                // if we didnt find any matches for this search term, move on
                if (!matches.Any())
                    continue;

                // if this is the first matching term for this line, create a new result obj for it
                if (lineResult is null)
                    lineResult = new LineResult() { LineNumber = lineRecord.LineNum, Line = lineRecord.Line };

                // Add the matches for this term to the line
                lineResult.AddTermResult(new TermResult()
                {
                    Term = regex.SearchTerm,
                    IndexOfMatches = matches.Select(m => m.Index).ToArray()
                });
            }

            if (lineResult is null)
                continue;

            // If the user specified that a line must contain all search terms
            //    make sure the line has a found term count equal to the compiled regex list passed in
            if (inputs.Filters_LineContainsAll && lineResult.FoundTerms.Count != RegexHelper.CompiledRegex.Length)
                continue;

            if (lineResult is not null)
                batchResult.LineResults.Add(lineResult);
        }

        return batchResult;
    }
}
