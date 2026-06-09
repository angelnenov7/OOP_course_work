using ShapeLibrary;

namespace OOP_course_work.Drawing;

internal static class ShapeRenderer
{
    public static void Draw(Graphics graphics, Shape shape, bool selected)
    {
        if (!shape.IsVisible)
        {
            return;
        }

        RectangleF bounds = new((float)shape.X, (float)shape.Y, (float)shape.Width, (float)shape.Height);
        using SolidBrush fillBrush = new(Color.FromArgb(shape.FillColorArgb));
        using Pen borderPen = new(Color.FromArgb(shape.BorderColorArgb), selected ? 3 : 2);

        switch (shape.Kind)
        {
            case ShapeKind.Rectangle:
                graphics.FillRectangle(fillBrush, bounds);
                graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                break;
            case ShapeKind.Ellipse:
                graphics.FillEllipse(fillBrush, bounds);
                graphics.DrawEllipse(borderPen, bounds);
                break;
            case ShapeKind.Triangle:
                PointF[] points =
                {
                    new(bounds.Left + bounds.Width / 2f, bounds.Top),
                    new(bounds.Left, bounds.Bottom),
                    new(bounds.Right, bounds.Bottom)
                };
                graphics.FillPolygon(fillBrush, points);
                graphics.DrawPolygon(borderPen, points);
                break;
        }

        if (selected)
        {
            using Pen selectionPen = new(Color.Black) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            graphics.DrawRectangle(selectionPen, bounds.X - 4, bounds.Y - 4, bounds.Width + 8, bounds.Height + 8);
        }
    }
}
