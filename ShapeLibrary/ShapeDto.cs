namespace ShapeLibrary;

public sealed class ShapeDto
{
    public Guid Id { get; set; }
    public ShapeKind Kind { get; set; }
    public string Name { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public int FillColorArgb { get; set; }
    public int BorderColorArgb { get; set; }
    public bool IsVisible { get; set; } = true;
}
