namespace MultiTermSearch.Events;

internal class FileListIdentifiedEventArgs : EventArgs
{
    public int FileCount { get; }

    public FileListIdentifiedEventArgs(int fileCount)
    {
        FileCount = fileCount;
    }
}