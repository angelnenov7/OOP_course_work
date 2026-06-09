using System.Text.Json;

namespace ShapeLibrary;

public static class SceneSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static void Save(string filePath, ShapeScene scene)
    {
        string json = JsonSerializer.Serialize(scene.ToDtos(), Options);
        File.WriteAllText(filePath, json);
    }

    public static ShapeScene Load(string filePath)
    {
        string json = File.ReadAllText(filePath);
        List<ShapeDto>? dtos = JsonSerializer.Deserialize<List<ShapeDto>>(json, Options);
        ShapeScene scene = new();
        scene.ReplaceAll((dtos ?? new List<ShapeDto>()).Select(Shape.FromDto));
        return scene;
    }
}
