using Godot;
using System;
using simulation_tools;

public partial class MapScene : Node2D
{
    [Export]
    world_map_script world_map;
    World mainWorld;
    int counter = 0;
    public override void _Ready()
    {
        base._Ready();
        if (NameState.Instance.isLoaded)
        {
            GD.Print("MapScene is ready and loading existing world.");
            world_data_manipulater loader = new world_data_manipulater(NameState.Instance.global_path);
            mainWorld = loader.load_world(NameState.Instance.slot_name);
            string fileName = System.IO.Path.Combine(NameState.Instance.global_path, "slot_" + NameState.Instance.slot_ID);
            string path = System.IO.Path.Combine(fileName, "world_map_data.json");
            load_world_map(path);
            world_map.create_map(mainWorld.material_matrix);
            save_world();
        }
        else
        {
            int slot_ID = NameState.Instance.slot_ID;
            int width = NameState.Instance.width;
            int length = NameState.Instance.length;
            int number_of_materials = NameState.Instance.number_of_materials;
            string global_path = NameState.Instance.global_path;
            material_limits materialLimits = NameState.Instance.materialLimits;
            mainWorld = new World(width, length, number_of_materials, global_path, slot_ID, materialLimits);
            var colors = mainWorld.get_all_colors();
            world_map.initialize_world(width, length, colors);
            world_map.create_map(mainWorld.material_matrix);
            save_world();
        }
    }
    public void save_world_map()
    {
        world_map_script_file_manager fileManager = new world_map_script_file_manager();
        string fileName = System.IO.Path.Combine(mainWorld.global_path, "slot_" + mainWorld.slot_index);
        string filePath = System.IO.Path.Combine(fileName, "world_map_data.json");
        fileManager.SaveWorldMap(world_map, filePath);
    }
    public void load_world_map(string fileName)
    {
        world_map_script_file_manager fileManager = new world_map_script_file_manager();
        world_map_script loadedWorldMap = fileManager.LoadWorldMap(fileName, world_map);
    }
    public void save_world()
    {
        world_data_manipulater dataManipulator = new world_data_manipulater(mainWorld.global_path);
        string fileName = System.IO.Path.Combine(mainWorld.global_path, "slot_" + mainWorld.slot_index);
        dataManipulator.save_world(mainWorld, fileName);
        save_world_map();

    }
    public override void _Process(double delta)
    {
        counter++;
        if (counter % 10 == 0)
        {
            base._Process(delta);
            mainWorld.simulation();
            world_map.create_map(mainWorld.material_matrix);
            save_world();
        }
    }
    public override void _ExitTree()
    {
        if (this != null)
        {
            GD.Print("MapScene is exiting tree and saving world.");
            save_world();
        }
        base._ExitTree();
    }
}
