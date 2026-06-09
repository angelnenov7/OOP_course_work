namespace ShapeLibrary.Commands;

public sealed class CommandHistory
{
    private readonly Stack<ISceneCommand> undoStack = new();
    private readonly Stack<ISceneCommand> redoStack = new();

    public event EventHandler? Changed;

    public bool CanUndo => undoStack.Count > 0;
    public bool CanRedo => redoStack.Count > 0;
    public string NextUndoName => CanUndo ? undoStack.Peek().Name : string.Empty;
    public string NextRedoName => CanRedo ? redoStack.Peek().Name : string.Empty;

    public void Execute(ISceneCommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
        OnChanged();
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            return;
        }

        ISceneCommand command = undoStack.Pop();
        command.Undo();
        redoStack.Push(command);
        OnChanged();
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            return;
        }

        ISceneCommand command = redoStack.Pop();
        command.Execute();
        undoStack.Push(command);
        OnChanged();
    }

    public void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
        OnChanged();
    }

    private void OnChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
