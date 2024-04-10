using MultiTermSearch.Classes;

namespace MultiTermSearch.Events;

internal class ItemAddedEventArgs : EventArgs
{
    public FileResult? Item { get; } = null;

    public ItemAddedEventArgs() { }
    public ItemAddedEventArgs(FileResult? item)
    {
        Item = item;
    }
}
