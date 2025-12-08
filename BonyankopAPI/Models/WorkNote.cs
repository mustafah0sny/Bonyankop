namespace BonyankopAPI.Models;

public class WorkNote
{
    public DateTime Timestamp { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
}
