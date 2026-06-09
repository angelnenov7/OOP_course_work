using System.Collections.ObjectModel;

namespace ShapeLibrary;

public sealed class ShapeScene
{
    private readonly List<Shape> shapes = new();

    public event EventHandler? Changed;

    public IReadOnlyList<Shape> Shapes => new ReadOnlyCollection<Shape>(shapes);

    public void Add(Shape shape)
    {
        shapes.Add(shape);
        OnChanged();
    }

    public bool Remove(Guid id)
    {
        Shape? shape = FindById(id);
        if (shape is null)
        {
            return false;
        }

        shapes.Remove(shape);
        OnChanged();
        return true;
    }

    public void Clear()
    {
        shapes.Clear();
        OnChanged();
    }

    public Shape? FindById(Guid id)
    {
        return shapes.FirstOrDefault(shape => shape.Id == id);
    }

    public Shape? HitTest(Point2D point)
    {
        return shapes.LastOrDefault(shape => shape.IsVisible && shape.Contains(point));
    }

    public void ReplaceAll(IEnumerable<Shape> newShapes)
    {
        shapes.Clear();
        shapes.AddRange(newShapes);
        OnChanged();
    }

    public int CountVisible()
    {
        return shapes.Count(shape => shape.IsVisible);
    }

    public double TotalArea()
    {
        return shapes.Where(shape => shape.IsVisible).Sum(shape => shape.Area);
    }

    public IReadOnlyDictionary<ShapeKind, int> CountByKind()
    {
        return shapes.GroupBy(shape => shape.Kind).ToDictionary(group => group.Key, group => group.Count());
    }

    public IReadOnlyList<Shape> LargestShapes(int count)
    {
        return shapes.OrderByDescending(shape => shape.Area).Take(count).ToList();
    }

    public IReadOnlyDictionary<ShapeKind, double> AverageAreaByKind()
    {
        return shapes.GroupBy(shape => shape.Kind).ToDictionary(group => group.Key, group => group.Average(shape => shape.Area));
    }

    public IReadOnlyList<ShapeDto> ToDtos()
    {
        return shapes.Select(shape => shape.ToDto()).ToList();
    }

    public void NotifyChanged()
    {
        OnChanged();
    }

    private void OnChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
