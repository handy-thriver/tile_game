using Godot;
using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Text.Json;
using System.Runtime.CompilerServices;
public record world_map_data(
    int width,
    int height,
    List<Tuple<float, float, float>> alt_colors,
    List<int> alt_IDS
);
public partial class world_map_script : TileMapLayer
{
    [Export]
    public TileSet tileSet { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    // The ID of the source in your TileSet (usually 0 if you have one source)
    private const int SourceId = 0;
    public List<int> alt_IDS = new List<int>();
    // The coordinate of the tile within the TileSet source
    private readonly Vector2I _atlasCoords = new Vector2I(0, 0);
    public List<Tuple<float, float, float>> alt_colors = new List<Tuple<float, float, float>>();
    public void initialize_world(int width, int height, List<Tuple<float, float, float>> colors)
    {

        this.width = width;
        this.height = height;
        this.alt_colors = colors;
        // Initialization code here
        GD.Print("World Map Script is ready.");
        GD.Print("TileSet: " + (tileSet != null ? tileSet.ResourceName : "No TileSet assigned."));
        for (int i = 0; i < colors.Count; i++)
        {
            GD.Print("Color " + i + ": " + colors[i]);
            alt_IDS.Add(CreateAlternative(colors[i]));

        }
    }
    public int CreateAlternative(Tuple<float, float, float> color)
    {
        GD.Print("Creating alternative tile with color: " + color);
        // Unpack the color
        float red = color.Item1;
        float green = color.Item2;
        float blue = color.Item3;
        Color tileColor = new Color(red, green, blue);
        // Get the TileSetAtlasSource
        TileSetAtlasSource source = tileSet.GetSource(SourceId) as TileSetAtlasSource;
        if (source == null)
        {
            GD.PrintErr("Source is not a TileSetAtlasSource.");
            return -1;
        }

        // Get the tile ID from the atlas coordinates
        int tileId = source.GetNextAlternativeTileId(_atlasCoords);
        if (tileId == -1)
        {
            GD.PrintErr($"No tile found at atlas coords {_atlasCoords}");
            return -1;
        }

        // Create an alternative tile for that tileId
        int altId = source.CreateAlternativeTile(_atlasCoords, tileId);

        // Apply color modulate
        source.GetTileData(_atlasCoords, tileId).Modulate = tileColor;
        return altId;
    }
    public override void _ExitTree()
    {
        if (this != null)
        {
            this.Clear();
            GD.Print("World Map Script is exiting tree and clearing tiles.");
        }
        base._ExitTree();
    }
    public void create_map(Matrix<float> map)
    {
        this.Clear();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Get the value from the map
                int value = (int)Math.Round(map[x, y]);
                if (value < 0 || value >= alt_IDS.Count)
                {
                    GD.PrintErr($"Value {value} at ({x}, {y}) is out of bounds for alt_IDS.");
                    continue;
                }
                else
                {
                    // Set the tile at the specified position
                    Vector2I position = new Vector2I(x, y);
                    int tileId = alt_IDS[value];
                    this.SetCell(position,SourceId,_atlasCoords, tileId);
                }
            }
        }
    }

}
public class world_map_script_file_manager()
{
    public string path =ProjectSettings.GlobalizePath("user://slots");
    public void SaveWorldMap(world_map_script worldMap, string fileName)
    {
        // Create a dictionary to hold the world map data
        string  filePath = System.IO.Path.Combine(path, fileName);
        world_map_data worldData = new world_map_data(
            width: worldMap.width,
            height: worldMap.height,
            alt_colors: worldMap.alt_colors,
            alt_IDS: worldMap.alt_IDS
        );
        // Serialize the dictionary to JSON
        string json = JsonSerializer.Serialize(worldData, new JsonSerializerOptions { WriteIndented = true });
        // Write the JSON to a file
        System.IO.File.WriteAllText(filePath, json);

    }
    public world_map_script LoadWorldMap(string fileName,world_map_script worldMap)
    {
        // Construct the file path
        string filePath = System.IO.Path.Combine(path, fileName);
        // Read the JSON from the file
        string json = System.IO.File.ReadAllText(filePath);
        // Deserialize the JSON to a dictionary
        world_map_data worldData = JsonSerializer.Deserialize<world_map_data>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (worldData == null)
        {
            GD.PrintErr("Failed to deserialize world map data.");
            return null;
        }
        // Set properties from the deserialized data
        worldMap.initialize_world(worldData.width, worldData.height, worldData.alt_colors);

        return worldMap;
    } 

        // Serialize 
}