namespace ShapeLibrary;

public sealed class RectangleShape : Shape
{
    public RectangleShape(double x, double y, double width, double height) : base(x, y, width, height)
    {
    }

    public override ShapeKind Kind => ShapeKind.Rectangle;
    public override double Area => Width * Height;
}
