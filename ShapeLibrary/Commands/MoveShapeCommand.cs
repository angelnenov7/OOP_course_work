namespace ShapeLibrary.Commands;

public sealed class MoveShapeCommand : SceneCommand
{
    private readonly Guid id;
    private readonly double dx;
    private readonly double dy;

    public MoveShapeCommand(ShapeScene scene, Guid id, double dx, double dy) : base(scene, "Move shape")
    {
        this.id = id;
        this.dx = dx;
        this.dy = dy;
    }

    public override void Execute()
    {
        Shape? shape = Scene.FindById(id);
        if (shape is null)
        {
            return;
        }

        shape.MoveBy(dx, dy);
        Scene.NotifyChanged();
        OnExecuted();
    }

    public override void Undo()
    {
        Shape? shape = Scene.FindById(id);
        if (shape is null)
        {
            return;
        }

        shape.MoveBy(-dx, -dy);
        Scene.NotifyChanged();
    }
}
