using MultiTermSearch.Classes;
using MultiTermSearch.Events;
using System.ComponentModel;
using System.Collections.Concurrent;
using MultiTermSearch.Helpers;
using System.Diagnostics;

namespace MultiTermSearch.Logic;

internal class SearchDriver
{
    private BackgroundWorker _driver = null!;

    private bool _searchInProgress = false;
    internal bool SearchInProgress { get { return _searchInProgress; } }
    private CancellationTokenSource _cancellationTokenSource = null!;
    public Stopwatch SearchTimer { get; private set; } = new Stopwatch();


    internal event EventHandler<FileListIdentifiedEventArgs>? FileListIdentifiedEvent;
    internal event EventHandler<ItemAddedEventArgs>? FileSearchedEvent;
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

        // start the actual search
        ClearHelpers();
        _searchInProgress = true;
        SearchTimer.Restart();
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
        FileQueueHelper.ClearFileQueue();
        RegexHelper.ClearCompiledQueries();
    }

    /// <summary>
    /// Performs the actual search logic of the files
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <param name="compiledRegex"></param>
    /// <returns></returns>
    private async Task<FileResult?> ScanObjectForMatch(string filePath, SearchInputs inputs, CancellationToken cancelToken)
    {
        // 1. Check if we should exclude this file because of the directory it is in
        if (FileMetaDataSearchLogic.ExcludeDirectory(filePath, inputs))
            return null;

        // 2. Check file type
        if (!FileMetaDataSearchLogic.IsValidFileType(filePath, inputs))
            return null;

        if (cancelToken.IsCancellationRequested)
            return null;

        // 3. Check the file contents
        if (inputs.Target == SearchInputs.ESearchTargets.FileContents || inputs.Target == SearchInputs.ESearchTargets.Both)
        {
            var result = await ContentSearchLogic.ScanFileContents(filePath, inputs, cancelToken);

            // If we got a result here... no need to check the file name
            if (result != null)
                return result;
        }

        if (cancelToken.IsCancellationRequested)
            return null;

        // 4. Last resort... check if the file name matches
        if (inputs.Target == SearchInputs.ESearchTargets.FileNames || inputs.Target == SearchInputs.ESearchTargets.Both)
            return FileMetaDataSearchLogic.ScanFileName(filePath, cancelToken);

        // If we made it here... no scenario that was checked found anything
        return null;
    }



    private void SearchDriver_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (e.Argument is not SearchInputs inputs)
            return;


        // Pre-compile all of the regex queries we will need to hopefully more efficiently search through files
        //   The helper stores them in a static collection easily accessible by all threads 
        RegexHelper.CompileRegexQueries(inputs).Wait();


        // Get the queue of files we need to scan
        //    once we have that list, we can report the count back to the parent
        FileQueueHelper.LoadFileQueue(inputs).Wait();
        FileListIdentifiedEvent?.Invoke(this, new FileListIdentifiedEventArgs(FileQueueHelper.FileQueue.Count));


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
                        while (FileQueueHelper.FileQueue.TryDequeue(out var file))
                        {
                            var result = await ScanObjectForMatch(file, inputs, cancelToken);

                            if (cancelToken.IsCancellationRequested)
                                return;

                            _driver.ReportProgress(1, result);
                        }
                    }
                ).Wait();
        }
        catch (Exception) 
        { 
            // dont give a crap what error it is
        }

        // Once the above ForEachAsync returns... the queue should be exhausted and all threads returned/completed.
        //   We can just let the method return
    }
    private void SearchDriver_Report(object? sender, ProgressChangedEventArgs e)
    {
        switch (e.ProgressPercentage)
        {
            case 0:
                FileListIdentifiedEvent?.Invoke(this, new FileListIdentifiedEventArgs(e.UserState is null ? 0 : (int)e.UserState));
                break;

            case 1:
                FileSearchedEvent?.Invoke(this, new ItemAddedEventArgs(e.UserState is null ? null : (FileResult)e.UserState));
                break;
        }
    }
    private void SearchDriver_Completed(object? sender, RunWorkerCompletedEventArgs e)
    {
        ClearHelpers();
        SearchTimer.Stop();
        _searchInProgress = false;
        SearchCompleteEvent?.Invoke(this, e);
    }
}