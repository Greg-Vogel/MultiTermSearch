using MultiTermSearch.Classes;

namespace MultiTermSearch.Events;

internal class ItemAddedEventArgs : EventArgs
{
    public FileResult? Item { get; }

    public ItemAddedEventArgs(FileResult? item)
    {
        Item = item;
    }
}
