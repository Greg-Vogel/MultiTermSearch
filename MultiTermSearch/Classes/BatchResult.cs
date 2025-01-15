
namespace MultiTermSearch.Classes
{
    public class BatchResult
    {
        public string? Error { get; set; }
        public List<LineResult> LineResults { get; set; } = new List<LineResult>();
    }
}
