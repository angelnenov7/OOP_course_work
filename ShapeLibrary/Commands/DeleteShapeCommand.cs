namespace ShapeLibrary.Commands;

public sealed class DeleteShapeCommand : SceneCommand
{
    private readonly Guid id;
    private Shape? removedShape;

    public DeleteShapeCommand(ShapeScene scene, Guid id) : base(scene, "Delete shape")
    {
        this.id = id;
    }

    public override void Execute()
    {
        Shape? shape = Scene.FindById(id);
        if (shape is null)
        {
            return;
        }

        removedShape = shape.Clone();
        Scene.Remove(id);
        OnExecuted();
    }

    public override void Undo()
    {
        if (removedShape is not null)
        {
            Scene.Add(removedShape.Clone());
        }
    }
}
