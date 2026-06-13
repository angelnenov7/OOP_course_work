using OOP_course_work.Drawing;
using ShapeLibrary;
using ShapeLibrary.Commands;

namespace OOP_course_work
{
    public partial class Form1 : Form
    {
        private readonly ShapeScene scene = new();
        private readonly CommandHistory history = new();
        private readonly DoubleBufferedPanel canvas = new();
        private readonly ListBox shapeList = new();
        private readonly Label statusLabel = new();
        private readonly Label statsLabel = new();
        private readonly ToolStripButton undoButton = new("Undo");
        private readonly ToolStripButton redoButton = new("Redo");
        private readonly ToolStripButton deleteButton = new("Delete");
        private readonly ToolStripButton editButton = new("Edit");
        private Guid? selectedShapeId;
        private Point lastMousePoint;
        private Point dragStartPoint;
        private bool dragging;
        private bool refreshingShapeList;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            WireEvents();
            RefreshUi();
        }

        private void BuildInterface()
        {
            Text = "OOP Shapes Editor";
            MinimumSize = new Size(980, 620);

            ToolStrip toolStrip = new() { GripStyle = ToolStripGripStyle.Hidden, Dock = DockStyle.Top };
            toolStrip.Items.Add(new ToolStripButton("Rectangle", null, (_, _) => AddShape(ShapeKind.Rectangle)));
            toolStrip.Items.Add(new ToolStripButton("Ellipse", null, (_, _) => AddShape(ShapeKind.Ellipse)));
            toolStrip.Items.Add(new ToolStripButton("Triangle", null, (_, _) => AddShape(ShapeKind.Triangle)));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(editButton);
            toolStrip.Items.Add(deleteButton);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(undoButton);
            toolStrip.Items.Add(redoButton);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Save", null, (_, _) => SaveScene()));
            toolStrip.Items.Add(new ToolStripButton("Load", null, (_, _) => LoadScene()));

            canvas.BackColor = Color.White;
            canvas.Dock = DockStyle.Fill;
            canvas.BorderStyle = BorderStyle.FixedSingle;

            Panel sidePanel = new() { Dock = DockStyle.Right, Width = 270, Padding = new Padding(8) };
            Label listTitle = new() { Text = "Shapes", Dock = DockStyle.Top, Height = 24 };
            shapeList.Dock = DockStyle.Top;
            shapeList.Height = 250;
            statsLabel.Dock = DockStyle.Fill;
            statsLabel.Padding = new Padding(0, 12, 0, 0);
            sidePanel.Controls.Add(statsLabel);
            sidePanel.Controls.Add(shapeList);
            sidePanel.Controls.Add(listTitle);

            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Height = 26;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            statusLabel.Padding = new Padding(8, 0, 0, 0);

            Controls.Add(canvas);
            Controls.Add(sidePanel);
            Controls.Add(statusLabel);
            Controls.Add(toolStrip);
        }

        private void WireEvents()
        {
            scene.Changed += (_, _) => RefreshUi();
            history.Changed += (_, _) => RefreshHistoryButtons();
            canvas.Paint += (_, e) => DrawScene(e.Graphics);
            canvas.MouseDown += CanvasMouseDown;
            canvas.MouseMove += CanvasMouseMove;
            canvas.MouseUp += CanvasMouseUp;
            canvas.MouseDoubleClick += (_, _) => EditSelectedShape();
            shapeList.SelectedIndexChanged += (_, _) => SelectShapeFromList();
            editButton.Click += (_, _) => EditSelectedShape();
            deleteButton.Click += (_, _) => DeleteSelectedShape();
            undoButton.Click += (_, _) => history.Undo();
            redoButton.Click += (_, _) => history.Redo();
        }

        private void AddShape(ShapeKind kind)
        {
            int offset = scene.Shapes.Count * 12;
            Shape shape = kind switch
            {
                ShapeKind.Rectangle => new RectangleShape(60 + offset, 60 + offset, 140, 90),
                ShapeKind.Ellipse => new EllipseShape(80 + offset, 80 + offset, 130, 100),
                ShapeKind.Triangle => new TriangleShape(90 + offset, 70 + offset, 140, 120),
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };

            shape.FillColorArgb = kind switch
            {
                ShapeKind.Rectangle => Color.FromArgb(255, 144, 202, 249).ToArgb(),
                ShapeKind.Ellipse => Color.FromArgb(255, 165, 214, 167).ToArgb(),
                ShapeKind.Triangle => Color.FromArgb(255, 255, 213, 79).ToArgb(),
                _ => shape.FillColorArgb
            };

            history.Execute(new AddShapeCommand(scene, shape));
            selectedShapeId = shape.Id;
            RefreshUi();
        }

        private void DrawScene(Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (Shape shape in scene.Shapes)
            {
                ShapeRenderer.Draw(graphics, shape, selectedShapeId == shape.Id);
            }
        }

        private void CanvasMouseDown(object? sender, MouseEventArgs e)
        {
            Shape? hit = scene.HitTest(new Point2D(e.X, e.Y));
            selectedShapeId = hit?.Id;
            dragging = hit is not null;
            lastMousePoint = e.Location;
            dragStartPoint = e.Location;
            RefreshUi();
        }

        private void CanvasMouseMove(object? sender, MouseEventArgs e)
        {
            if (!dragging || selectedShapeId is not Guid id)
            {
                return;
            }

            Shape? shape = scene.FindById(id);
            if (shape is null)
            {
                return;
            }

            shape.MoveBy(e.X - lastMousePoint.X, e.Y - lastMousePoint.Y);
            lastMousePoint = e.Location;
            scene.NotifyChanged();
        }

        private void CanvasMouseUp(object? sender, MouseEventArgs e)
        {
            if (!dragging || selectedShapeId is not Guid id)
            {
                dragging = false;
                return;
            }

            dragging = false;
            double dx = e.X - dragStartPoint.X;
            double dy = e.Y - dragStartPoint.Y;
            if (Math.Abs(dx) < 0.1 && Math.Abs(dy) < 0.1)
            {
                return;
            }

            Shape? shape = scene.FindById(id);
            if (shape is null)
            {
                return;
            }

            shape.MoveBy(-dx, -dy);
            history.Execute(new MoveShapeCommand(scene, id, dx, dy));
        }

        private void SelectShapeFromList()
        {
            if (refreshingShapeList)
            {
                return;
            }

            if (shapeList.SelectedItem is ShapeListItem item)
            {
                selectedShapeId = item.Id;
                canvas.Invalidate();
                RefreshStatus();
            }
        }

        private void EditSelectedShape()
        {
            if (selectedShapeId is not Guid id)
            {
                return;
            }

            Shape? shape = scene.FindById(id);
            if (shape is null)
            {
                return;
            }

            ShapeDto before = shape.ToDto();
            using ShapePropertiesForm dialog = new(shape);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            ShapeDto after = dialog.CreateDto(shape.Id, shape.Kind);
            history.Execute(new UpdateShapeCommand(scene, before, after));
        }

        private void DeleteSelectedShape()
        {
            if (selectedShapeId is not Guid id)
            {
                return;
            }

            history.Execute(new DeleteShapeCommand(scene, id));
            selectedShapeId = null;
            RefreshUi();
        }

        private void SaveScene()
        {
            using SaveFileDialog dialog = new()
            {
                Filter = "Shapes scene (*.json)|*.json|All files (*.*)|*.*",
                FileName = "scene.json"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SceneSerializer.Save(dialog.FileName, scene);
                statusLabel.Text = $"Saved {scene.Shapes.Count} shape(s) to {Path.GetFileName(dialog.FileName)}";
            }
        }

        private void LoadScene()
        {
            using OpenFileDialog dialog = new()
            {
                Filter = "Shapes scene (*.json)|*.json|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            ShapeScene loaded = SceneSerializer.Load(dialog.FileName);
            scene.ReplaceAll(loaded.Shapes.Select(shape => shape.Clone()));
            selectedShapeId = scene.Shapes.FirstOrDefault()?.Id;
            history.Clear();
            statusLabel.Text = $"Loaded {scene.Shapes.Count} shape(s) from {Path.GetFileName(dialog.FileName)}";
        }

        private void RefreshUi()
        {
            RefreshShapeList();
            RefreshStats();
            RefreshStatus();
            RefreshHistoryButtons();
            canvas.Invalidate();
        }

        private void RefreshShapeList()
        {
            refreshingShapeList = true;
            shapeList.Items.Clear();

            foreach (Shape shape in scene.Shapes)
            {
                ShapeListItem item = new(shape.Id, $"{shape.Kind}  area: {shape.Area:F1}");
                int index = shapeList.Items.Add(item);
                if (shape.Id == selectedShapeId)
                {
                    shapeList.SelectedIndex = index;
                }
            }

            refreshingShapeList = false;
        }

        private void RefreshStats()
        {
            string byKind = string.Join(Environment.NewLine, scene.CountByKind().Select(pair => $"{pair.Key}: {pair.Value}"));
            string largest = string.Join(Environment.NewLine, scene.LargestShapes(3).Select(shape => $"{shape.Kind}: {shape.Area:F1}"));
            statsLabel.Text =
                $"Visible: {scene.CountVisible()}{Environment.NewLine}" +
                $"Total area: {scene.TotalArea():F1}{Environment.NewLine}{Environment.NewLine}" +
                $"By type:{Environment.NewLine}{byKind}{Environment.NewLine}{Environment.NewLine}" +
                $"Largest:{Environment.NewLine}{largest}";
        }

        private void RefreshStatus()
        {
            Shape? selected = selectedShapeId is Guid id ? scene.FindById(id) : null;
            statusLabel.Text = selected is null
                ? "Ready"
                : $"{selected.Kind}: X={selected.X:F1}, Y={selected.Y:F1}, W={selected.Width:F1}, H={selected.Height:F1}, Area={selected.Area:F1}";
            editButton.Enabled = selected is not null;
            deleteButton.Enabled = selected is not null;
        }

        private void RefreshHistoryButtons()
        {
            undoButton.Enabled = history.CanUndo;
            redoButton.Enabled = history.CanRedo;
            undoButton.ToolTipText = history.CanUndo ? history.NextUndoName : "Undo";
            redoButton.ToolTipText = history.CanRedo ? history.NextRedoName : "Redo";
        }

        private sealed class ShapeListItem
        {
            public ShapeListItem(Guid id, string text)
            {
                Id = id;
                Text = text;
            }

            public Guid Id { get; }
            private string Text { get; }

            public override string ToString()
            {
                return Text;
            }
        }

        private sealed class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
            }
        }
    }
}
