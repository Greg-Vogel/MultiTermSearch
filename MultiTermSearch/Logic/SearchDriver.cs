using MultiTermSearch.Models;
using MultiTermSearch.Events;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace MultiTermSearch.Logic;

internal class SearchDriver
{
    private BackgroundWorker _searchWorker = null!;
    private CancellationTokenSource _cancelToken = new();
    internal bool SearchInProgress { get { return _searchWorker is null ? false : _searchWorker.IsBusy; } }


    internal event EventHandler<ItemAddedEventArgs>? ItemAddedEvent;
    internal event EventHandler<EventArgs>? SearchCompleteEvent;
    internal event EventHandler<FileListIdentifiedEventArgs>? FileListIdentifiedEvent;


    private void InitializeSearchWorker()
    {
        if (_searchWorker != null)
            return;

        _searchWorker = new BackgroundWorker();
        _searchWorker.WorkerReportsProgress = true;
        _searchWorker.WorkerSupportsCancellation = true;
        _searchWorker.DoWork += _searchWorker_DoWork;
        _searchWorker.ProgressChanged += _searchWorker_ReportFileFound;
        _searchWorker.RunWorkerCompleted += _searchWorker_RunWorkerCompleted;
    }



    private void _searchWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (e.Argument is not SearchInputs inputs)
            return;

        // Pre-compile all of the regex queries we will need to hopefully more efficiently search through files
        var compiledRegex = CompileRegexQueries(inputs);


        // Start a parallel ForEach up process multiple files at once
        //    It uses EnumerateFileSystemEntries because it enumerates them as it finds them instead of waiting to find the full list of files upfront
        List<Task> tasks = new List<Task>();
        _cancelToken = new CancellationTokenSource();

        try
        {
            var filesToScan = Directory.EnumerateFiles(inputs.Path
                    , "*.*" // dont filter out here... we will use our own logic to determine if names/paths match
                    , inputs.IncludeSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToArray();

            _searchWorker.ReportProgress(0, filesToScan.Length);

            var result = Parallel.ForEach(filesToScan
                , new ParallelOptions() { MaxDegreeOfParallelism = inputs.SearcherThreadCount, CancellationToken = _cancelToken.Token } // limit the number of async threads we have searching files at a time
                , (filePath) =>
                {
                    var task = Task.Run(async () =>
                    {
                        // Scan the current file for matches
                        return await ScanObjectForMatch(filePath, inputs, compiledRegex);
                    });

                    Task.WaitAll(task);

                    // post results
                    _searchWorker.ReportProgress(1, task.Result);
                });
        }
        catch (OperationCanceledException)
        {
            // do nothing with this... its user requested
        }
        finally { _cancelToken.Dispose(); }
    }
    private void _searchWorker_ReportFileFound(object? sender, ProgressChangedEventArgs e)
    {
        if (e.ProgressPercentage == 0)
        {
            FileListIdentifiedEvent?.Invoke(this, new FileListIdentifiedEventArgs((int)e.UserState));
        }
        else
        {
            ItemAddedEvent?.Invoke(this, new ItemAddedEventArgs((FileResult)e.UserState));
        }

    }
    private void _searchWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        SearchCompleteEvent?.Invoke(this, new EventArgs());
    }


    /// <summary>
    /// Start the Search process
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    internal void StartSearchAsync(SearchInputs inputs)
    {
        // Prep the searchWorker if needed
        InitializeSearchWorker();

        // start the actual search
        _searchWorker.RunWorkerAsync(inputs);
    }

    /// <summary>
    /// Tells the search process to cancel asap
    /// </summary>
    internal void CancelSearchAsync()
    {
        _cancelToken.Cancel();
    }


    private SearchRegex[] CompileRegexQueries(SearchInputs inputs)
    {
        var ro = inputs.Options_IgnoreCase ? RegexOptions.Compiled | RegexOptions.IgnoreCase : RegexOptions.Compiled;

        return inputs.SearchTerms.Select(t => new SearchRegex(
            searchPattern: inputs.Options_MatchWholeWord ? $@"\b({t})\b" : t
            , baseSearchTerm: t
            , options: ro
        )).ToArray();
    }

    /// <summary>
    /// Performs the actual search logic of the files
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <param name="compiledRegex"></param>
    /// <returns></returns>
    private async Task<FileResult?> ScanObjectForMatch(string filePath, SearchInputs inputs, SearchRegex[] compiledRegex)
    {
        // 1. Check if we should exclude this file because of the directory it is in
        if (ExcludeDirectory(filePath, inputs))
            return null;

        // 2. Check file type
        if (!IsValidFileType(filePath, inputs))
            return null;

        // 3. Check the file contents
        if (inputs.Target == SearchInputs.ESearchTargets.FileContents || inputs.Target == SearchInputs.ESearchTargets.Both)
        {
            var result = await CheckFileContents(filePath, inputs, compiledRegex);

            // If we got a result here... no need to check the file name
            if (result != null)
                return result;
        }

        // 4. Last resort... check if the file name matches
        if (inputs.Target == SearchInputs.ESearchTargets.FileNames || inputs.Target == SearchInputs.ESearchTargets.Both)
            return await CheckFileNaming(filePath, inputs, compiledRegex);

        // If we made it here... no scenario that was checked found anything
        return null;
    }

    /// <summary>
    /// Checks if we should exclude the current file being searched based on the directory it is in
    ///    We know some folders are massive and pointless to search for our needs
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <returns></returns>
    private bool ExcludeDirectory(string filePath, SearchInputs inputs)
    {
        if (!inputs.Options_ExcludeLargeDirectories)
            return false;

        if (filePath.Contains(@"\.git\") || filePath.Contains(@"\node_modules\"))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if a file should be included based on the defined Include File Types in the SearchInputs
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <returns></returns>
    private bool IsValidFileType(string filePath, SearchInputs inputs)
    {
        // If the file type list included the wildcard, then we need to check all file types for the search vals
        if (inputs.ScanAllFileTypes)
            return true;

        // We do not have the wild card, so we need to see if any of the Include File Types defined are the filetype at the end of the filepath
        return inputs.IncludeTypes.Any(filePath.EndsWith);
    }

    /// <summary>
    /// Actual logic for reading contents inside of files and identifying matches
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <param name="compiledRegex"></param>
    /// <returns></returns>
    private async Task<FileResult?> CheckFileContents(string filePath, SearchInputs inputs, SearchRegex[] compiledRegex)
    {
        var fileLines = await File.ReadAllLinesAsync(filePath);

        // For each regex, scans the file and gets a set of matches for that regex
        FileResult? result = null;
        int lineNum = 0;
        foreach (string line in fileLines)
        {
            lineNum += 1;
            LineResult? lineResult = null;
            foreach (var regex in compiledRegex)
            {
                // get the matches in this line of text for this search term
                var matches = regex.Matches(line);

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
                    if (lineResult.FoundTerms.Count != compiledRegex.Length)
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
                if (result.FoundTerms.Count != compiledRegex.Length)
                    return null;
            }
        }

        return result;
    }

    /// <summary>
    /// Actual logic for reading filenames and identifying matches
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <returns></returns>
    private async Task<FileResult?> CheckFileNaming(string filePath, SearchInputs inputs, SearchRegex[] compiledRegex)
    {
        FileResult? result = null;

        await Task.Run(() =>
        {
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

            LineResult? lineResult = null;
            foreach (var regex in compiledRegex)
            {
                // get the matches in this fileName for this search term
                var matches = regex.Matches(fileName);

                // if we didnt find any matches for this search term, move on
                if (!matches.Any())
                    continue;

                // if this is the first matching term for this fileName, create a new result obj for it
                if (lineResult == null)
                    lineResult = new LineResult() { LineNumber = 0, Line = fileName };

                // Add the matches for this term to the line
                lineResult.AddTermResult(new TermResult()
                {
                    Term = regex.SearchTerm,
                    IndexOfMatches = matches.Select(m => m.Index).ToArray()
                });
            }

            if (lineResult != null)
            {
                if (result == null)
                    result = new FileResult(filePath);

                result.AddLineResult(lineResult);
            }
        });

        return result;
    }
}
