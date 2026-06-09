namespace ShapeLibrary;

public sealed class EllipseShape : Shape
{
    public EllipseShape(double x, double y, double width, double height) : base(x, y, width, height)
    {
        Name = "Ellipse";
    }

    public override ShapeKind Kind => ShapeKind.Ellipse;
    public override double Area => Math.PI * (Width / 2.0) * (Height / 2.0);

    public override bool Contains(Point2D point)
    {
        double radiusX = Width / 2.0;
        double radiusY = Height / 2.0;
        double centerX = X + radiusX;
        double centerY = Y + radiusY;
        double normalizedX = (point.X - centerX) / radiusX;
        double normalizedY = (point.Y - centerY) / radiusY;

        return normalizedX * normalizedX + normalizedY * normalizedY <= 1.0;
    }
}
