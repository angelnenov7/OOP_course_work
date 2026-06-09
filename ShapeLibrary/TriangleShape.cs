namespace ShapeLibrary;

public sealed class TriangleShape : Shape
{
    public TriangleShape(double x, double y, double width, double height) : base(x, y, width, height)
    {
        Name = "Triangle";
    }

    public override ShapeKind Kind => ShapeKind.Triangle;
    public override double Area => Width * Height / 2.0;

    public override bool Contains(Point2D point)
    {
        Point2D a = new(X + Width / 2.0, Y);
        Point2D b = new(X, Y + Height);
        Point2D c = new(X + Width, Y + Height);

        double area = TriangleArea(a, b, c);
        double area1 = TriangleArea(point, b, c);
        double area2 = TriangleArea(a, point, c);
        double area3 = TriangleArea(a, b, point);

        return Math.Abs(area - (area1 + area2 + area3)) < 0.5;
    }

    private static double TriangleArea(Point2D a, Point2D b, Point2D c)
    {
        return Math.Abs((a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y)) / 2.0);
    }
}
