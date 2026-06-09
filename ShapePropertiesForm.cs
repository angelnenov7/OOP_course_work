using ShapeLibrary;

namespace OOP_course_work;

public sealed class ShapePropertiesForm : Form
{
    private readonly TextBox nameTextBox = new();
    private readonly NumericUpDown xInput = new();
    private readonly NumericUpDown yInput = new();
    private readonly NumericUpDown widthInput = new();
    private readonly NumericUpDown heightInput = new();
    private readonly CheckBox visibleCheckBox = new();
    private readonly Button fillButton = new();
    private readonly Button borderButton = new();
    private int fillArgb;
    private int borderArgb;

    public ShapePropertiesForm(Shape shape)
    {
        Text = "Shape properties";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(320, 360);

        fillArgb = shape.FillColorArgb;
        borderArgb = shape.BorderColorArgb;

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 9,
            Padding = new Padding(12)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));

        ConfigureNumber(xInput, shape.X);
        ConfigureNumber(yInput, shape.Y);
        ConfigureNumber(widthInput, shape.Width);
        ConfigureNumber(heightInput, shape.Height);

        nameTextBox.Text = shape.Name;
        nameTextBox.Dock = DockStyle.Fill;
        visibleCheckBox.Checked = shape.IsVisible;
        visibleCheckBox.Dock = DockStyle.Left;
        fillButton.BackColor = Color.FromArgb(fillArgb);
        borderButton.BackColor = Color.FromArgb(borderArgb);
        fillButton.Dock = DockStyle.Fill;
        borderButton.Dock = DockStyle.Fill;
        fillButton.Click += (_, _) => PickColor(fillButton, ref fillArgb);
        borderButton.Click += (_, _) => PickColor(borderButton, ref borderArgb);

        AddRow(layout, 0, "Name", nameTextBox);
        AddRow(layout, 1, "X", xInput);
        AddRow(layout, 2, "Y", yInput);
        AddRow(layout, 3, "Width", widthInput);
        AddRow(layout, 4, "Height", heightInput);
        AddRow(layout, 5, "Fill", fillButton);
        AddRow(layout, 6, "Border", borderButton);
        AddRow(layout, 7, "Visible", visibleCheckBox);

        FlowLayoutPanel buttons = new() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
        Button okButton = new() { Text = "OK", DialogResult = DialogResult.OK, Width = 88 };
        Button cancelButton = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 88 };
        buttons.Controls.Add(okButton);
        buttons.Controls.Add(cancelButton);
        layout.Controls.Add(buttons, 0, 8);
        layout.SetColumnSpan(buttons, 2);

        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(layout);
    }

    public ShapeDto CreateDto(Guid id, ShapeKind kind)
    {
        return new ShapeDto
        {
            Id = id,
            Kind = kind,
            Name = nameTextBox.Text.Trim(),
            X = (double)xInput.Value,
            Y = (double)yInput.Value,
            Width = (double)widthInput.Value,
            Height = (double)heightInput.Value,
            FillColorArgb = fillArgb,
            BorderColorArgb = borderArgb,
            IsVisible = visibleCheckBox.Checked
        };
    }

    private static void AddRow(TableLayoutPanel layout, int row, string labelText, Control control)
    {
        Label label = new() { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }

    private static void ConfigureNumber(NumericUpDown input, double value)
    {
        input.Minimum = -10000;
        input.Maximum = 10000;
        input.DecimalPlaces = 1;
        input.Increment = 5;
        input.Value = (decimal)value;
        input.Dock = DockStyle.Fill;
    }

    private static void PickColor(Button button, ref int argb)
    {
        using ColorDialog dialog = new() { Color = Color.FromArgb(argb), FullOpen = true };
        if (dialog.ShowDialog(button.FindForm()) == DialogResult.OK)
        {
            argb = dialog.Color.ToArgb();
            button.BackColor = dialog.Color;
        }
    }
}
