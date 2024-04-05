namespace MultiTermSearch.Events;
using MultiTermSearch.Models;

internal class ItemAddedEventArgs : EventArgs
{
    public FileResult Item { get; }

    public ItemAddedEventArgs(FileResult item)
    {
        Item = item;
    }
}
