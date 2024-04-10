using MultiTermSearch.Classes;
using System.Collections.Concurrent;
using System.Security.AccessControl;

namespace MultiTermSearch.Helpers;

internal static class FileSystemHelper
{
    internal static ConcurrentQueue<string> FileQueue { get; set; } = new ConcurrentQueue<string>();

    internal static async Task LoadFileQueue(SearchInputs inputs)
    {
        await Task.Run(() =>
        {
            // Get the list of files we need to scan and add them into a thread safe queue so our worker threads can pull them out one by one
            //    we will not filter out the list of file types here... we can do that later
            var filesToScan = Directory.GetFiles(inputs.Path
                , "*.*"
                , new EnumerationOptions()
                {
                    IgnoreInaccessible = true
                    , RecurseSubdirectories = inputs.IncludeSubDir
                });
            foreach (var file in filesToScan)
            {
                FileQueue.Enqueue(file);
            }
        });
    }

    internal static void ClearFileQueue()
    {
        FileQueue.Clear();
    }
}
