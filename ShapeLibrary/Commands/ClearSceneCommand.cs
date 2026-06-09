namespace ShapeLibrary.Commands;

public sealed class ClearSceneCommand : SceneCommand
{
    private List<Shape> previousShapes = new();

    public ClearSceneCommand(ShapeScene scene) : base(scene, "Clear scene")
    {
    }

    public override void Execute()
    {
        previousShapes = Scene.Shapes.Select(shape => shape.Clone()).ToList();
        Scene.Clear();
        OnExecuted();
    }

    public override void Undo()
    {
        Scene.ReplaceAll(previousShapes.Select(shape => shape.Clone()));
    }
}
