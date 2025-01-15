﻿using System.ComponentModel;
using MultiTermSearch.Classes;
using MultiTermSearch.Events;
using MultiTermSearch.Helpers;

namespace MultiTermSearch.SearchLogic;

internal class SearchDriver
{
    private BackgroundWorker _driver = null!;

    private bool _searchInProgress = false;
    internal bool SearchInProgress { get { return _searchInProgress; } }
    private CancellationTokenSource _cancellationTokenSource = null!;

    public string SearchPath { get; private set; } = string.Empty;

    internal event EventHandler<FileListIdentifiedEventArgs>? FileListIdentifiedEvent;
    internal event EventHandler<ItemAddedEventArgs>? FileProcessedEvent;
    internal event EventHandler<EventArgs>? FileSkppedEvent;
    internal event EventHandler<EventArgs>? SearchCompleteEvent;


    private void InitializeSearchDriver()
    {
        if (_driver != null)
            return;

        _driver = new BackgroundWorker();
        _driver.WorkerReportsProgress = true;
        _driver.WorkerSupportsCancellation = true;
        _driver.DoWork += SearchDriver_DoWork;
        _driver.ProgressChanged += SearchDriver_Report;
        _driver.RunWorkerCompleted += SearchDriver_Completed;
    }


    /// <summary>
    /// Start the Search process
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    internal void StartSearchAsync(SearchInputs inputs)
    {
        // Prep the searchDriver if needed
        InitializeSearchDriver();

        // expose the search path that we are posting results for
        SearchPath = inputs.Path;

        // start the actual search
        ClearHelpers();
        _searchInProgress = true;
        _driver.RunWorkerAsync(inputs);
    }

    /// <summary>
    /// Tells the search process to cancel asap
    /// </summary>
    internal void CancelSearchAsync()
    {
        if (!_searchInProgress)
            return;

        _cancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Clears the data from helper classes that can store data or queues while a search is running
    /// </summary>
    private void ClearHelpers()
    {
        FileSystemHelper.ClearFileQueue();
        RegexHelper.ClearCompiledQueries();
    }

    private bool SkipScanningFile(string filePath, SearchInputs inputs)
    {
        // 1. Check if we should exclude this file because of the directory it is in
        if (FileMetaDataSearchLogic.ExcludeDirectory(filePath, inputs))
            return true;

        // 2. Check file type
        if (!FileMetaDataSearchLogic.IsValidFileType(filePath, inputs))
            return true;

        return false;
    }

    /// <summary>
    /// Performs the actual search logic of the files
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <param name="compiledRegex"></param>
    /// <returns></returns>
    private async Task<FileResult?> ScanFileForMatch(string filePath, SearchInputs inputs, CancellationToken cancelToken)
    {
        var result = new FileResult(filePath);

        // 1. Quickly check the file name
        if (inputs.Target == SearchInputs.ESearchTargets.FileNames || inputs.Target == SearchInputs.ESearchTargets.Both)
            FileMetaDataSearchLogic.ScanFileName(ref result, filePath, cancelToken);

        // exit early before possibly reading a giant file if we were told to cancel the search
        if (cancelToken.IsCancellationRequested)
            return null;

        // 2. Check the file contents
        if (inputs.Target == SearchInputs.ESearchTargets.FileContents || inputs.Target == SearchInputs.ESearchTargets.Both)
            result = await FileContentSearchLogic.ScanFileContents(result, filePath, inputs, cancelToken);

        // if no results were found, just return a null up to the caller...
        //   no need to waste its time with file data that doesnt match our criteria
        return result.HasResult ? result : null;
    }



    private void SearchDriver_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (e.Argument is not SearchInputs inputs)
            return;

        // Make sure the result is clear... it will only be set in the case of an global exception
        e.Result = null;


        // Pre-compile all of the regex queries we will need to hopefully more efficiently search through files
        //   The helper stores them in a static collection easily accessible by all threads 
        RegexHelper.CompileRegexQueries(inputs).Wait();


        // Get the queue of files we need to scan
        //    once we have that list, we can report the count back to the parent
        FileSystemHelper.LoadFileQueue(inputs).Wait();
        FileListIdentifiedEvent?.Invoke(this, new FileListIdentifiedEventArgs(FileSystemHelper.FileQueue.Count));


        // Start the desired number of worker threads
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Parallel.ForEachAsync(
                Enumerable.Range(1, inputs.SearcherThreadCount)
                , _cancellationTokenSource.Token
                , async (threadId, cancelToken) =>
                    {
                        if (cancelToken.IsCancellationRequested)
                            return;

                        // continuously loop until we have exhausted our queue
                        //    once no more files are left it will automatically exit
                        while (FileSystemHelper.FileQueue.TryDequeue(out var file))
                        {
                            if (cancelToken.IsCancellationRequested)
                                return;

                            // If the file should be excluded from the scan... report back that we skipped one
                            if (SkipScanningFile(file, inputs))
                            {
                                _driver.ReportProgress(0);
                                continue;
                            }

                            if (cancelToken.IsCancellationRequested)
                                return;

                            // The file should be included... run through the actual scan logic
                            var result = await ScanFileForMatch(file, inputs, cancelToken);

                            if (cancelToken.IsCancellationRequested)
                                return;

                            // Report the result out to any result display/consumer
                            _driver.ReportProgress(1, result);
                        }
                    }
                ).Wait();
        }
        catch (Exception ex) 
        {
            e.Result = ex;
        }
    }

    private static EventArgs _skippedArgs = new EventArgs();

    private static ItemAddedEventArgs _emptyResultArgs = new ItemAddedEventArgs();
    private void SearchDriver_Report(object? sender, ProgressChangedEventArgs e)
    {
        switch (e.ProgressPercentage)
        {
            // A '0' means the file was skipped per the users filter criteria
            case 0:
                FileSkppedEvent?.Invoke(this, _skippedArgs);
                break;

            // A '1' means the file was at least scanned
            case 1:
                FileProcessedEvent?.Invoke(this
                    , e.UserState is null 
                        ? _emptyResultArgs 
                        : new ItemAddedEventArgs((FileResult?)e.UserState));
                break;
        }
    }
    private void SearchDriver_Completed(object? sender, RunWorkerCompletedEventArgs e)
    {
        ClearHelpers();
        _searchInProgress = false;
        SearchCompleteEvent?.Invoke(this, e);
    }
}
