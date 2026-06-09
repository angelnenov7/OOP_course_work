namespace ShapeLibrary.Commands;

public abstract class SceneCommand : ISceneCommand
{
    protected SceneCommand(ShapeScene scene, string name)
    {
        Scene = scene;
        Name = name;
    }

    public event EventHandler? Executed;
    public string Name { get; }
    protected ShapeScene Scene { get; }

    public abstract void Execute();
    public abstract void Undo();

    protected void OnExecuted()
    {
        Executed?.Invoke(this, EventArgs.Empty);
    }
}
