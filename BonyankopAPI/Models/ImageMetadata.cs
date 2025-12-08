namespace BonyankopAPI.Models;

public class ImageMetadata
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Format { get; set; } = string.Empty;
    public int Size { get; set; }
    public DateTime? CapturedAt { get; set; }
}
