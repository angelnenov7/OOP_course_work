namespace ShapeLibrary;

public abstract class Shape
{
    private double width;
    private double height;

    protected Shape(double x, double y, double width, double height)
    {
        Id = Guid.NewGuid();
        X = x;
        Y = y;
        Width = width;
        Height = height;
        FillColorArgb = unchecked((int)0xFF90CAF9);
        BorderColorArgb = unchecked((int)0xFF263238);
        IsVisible = true;
    }

    public Guid Id { get; private set; }
    public abstract ShapeKind Kind { get; }
    public double X { get; private set; }
    public double Y { get; private set; }

    public double Width
    {
        get => width;
        set => width = Math.Max(1, value);
    }

    public double Height
    {
        get => height;
        set => height = Math.Max(1, value);
    }

    public int FillColorArgb { get; set; }
    public int BorderColorArgb { get; set; }
    public bool IsVisible { get; set; }

    public abstract double Area { get; }

    public virtual void MoveBy(double dx, double dy)
    {
        X += dx;
        Y += dy;
    }

    public virtual void SetPosition(double x, double y)
    {
        X = x;
        Y = y;
    }

    public virtual void Resize(double newWidth, double newHeight)
    {
        Width = newWidth;
        Height = newHeight;
    }

    public virtual bool Contains(Point2D point)
    {
        return point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
    }

    public Shape Clone()
    {
        return FromDto(ToDto());
    }

    public ShapeDto ToDto()
    {
        return new ShapeDto
        {
            Id = Id,
            Kind = Kind,
            X = X,
            Y = Y,
            Width = Width,
            Height = Height,
            FillColorArgb = FillColorArgb,
            BorderColorArgb = BorderColorArgb,
            IsVisible = IsVisible
        };
    }

    public void Apply(ShapeDto dto)
    {
        Id = dto.Id == Guid.Empty ? Id : dto.Id;
        X = dto.X;
        Y = dto.Y;
        Width = dto.Width;
        Height = dto.Height;
        FillColorArgb = dto.FillColorArgb;
        BorderColorArgb = dto.BorderColorArgb;
        IsVisible = dto.IsVisible;
    }

    public static Shape FromDto(ShapeDto dto)
    {
        Shape shape = dto.Kind switch
        {
            ShapeKind.Rectangle => new RectangleShape(dto.X, dto.Y, dto.Width, dto.Height),
            ShapeKind.Ellipse => new EllipseShape(dto.X, dto.Y, dto.Width, dto.Height),
            ShapeKind.Triangle => new TriangleShape(dto.X, dto.Y, dto.Width, dto.Height),
            _ => throw new ArgumentOutOfRangeException(nameof(dto), "Unknown shape kind.")
        };

        shape.Apply(dto);
        return shape;
    }
}
