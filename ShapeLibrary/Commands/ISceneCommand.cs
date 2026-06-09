namespace ShapeLibrary.Commands;

public interface ISceneCommand
{
    event EventHandler? Executed;
    string Name { get; }
    void Execute();
    void Undo();
}
