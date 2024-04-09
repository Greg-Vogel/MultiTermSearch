namespace MultiTermSearch.Events;
using MultiTermSearch.Classes;

internal class ItemAddedEventArgs : EventArgs
{
    public FileResult? Item { get; }

    public ItemAddedEventArgs(FileResult? item)
    {
        Item = item;
    }
}
