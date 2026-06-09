namespace ShapeLibrary.Commands;

public sealed class UpdateShapeCommand : SceneCommand
{
    private readonly ShapeDto before;
    private readonly ShapeDto after;

    public UpdateShapeCommand(ShapeScene scene, ShapeDto before, ShapeDto after) : base(scene, "Edit shape")
    {
        this.before = before;
        this.after = after;
    }

    public override void Execute()
    {
        Apply(after);
        OnExecuted();
    }

    public override void Undo()
    {
        Apply(before);
    }

    private void Apply(ShapeDto dto)
    {
        Shape? shape = Scene.FindById(dto.Id);
        shape?.Apply(dto);
        Scene.NotifyChanged();
    }
}
