using MultiTermSearch.Classes;
using System.Collections.Concurrent;

namespace MultiTermSearch.Helpers;

internal static class FileQueueHelper
{
    internal static ConcurrentQueue<string> FileQueue { get; set; } = new ConcurrentQueue<string>();

    internal static async Task LoadFileQueue(SearchInputs inputs)
    {
        await Task.Run(() =>
        {
            // Get the list of files we need to scan and add them into a thread safe queue so our worker threads can pull them out one by one
            var filesToScan = Directory.GetFiles(inputs.Path
                    , "*.*" // dont filter out here... we will use our own logic to determine if names/paths match
                    , inputs.IncludeSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
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
