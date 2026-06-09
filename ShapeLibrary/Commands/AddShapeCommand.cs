namespace ShapeLibrary.Commands;

public sealed class AddShapeCommand : SceneCommand
{
    private readonly Shape shape;

    public AddShapeCommand(ShapeScene scene, Shape shape) : base(scene, $"Add {shape.Kind}")
    {
        this.shape = shape;
    }

    public override void Execute()
    {
        Scene.Add(shape);
        OnExecuted();
    }

    public override void Undo()
    {
        Scene.Remove(shape.Id);
    }
}
