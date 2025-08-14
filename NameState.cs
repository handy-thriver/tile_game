using Godot;
using System;
using simulation_tools;

public partial class NameState : Node
{
    public int slot_ID=0;
    public material_limits materialLimits{ get; set; } 
    public string slot_name {get; set; } 
    public int width = 0;
    public int length = 0;
    public string global_path = ProjectSettings.GlobalizePath("user://slots/");
    public int number_of_materials = 100;
    public static NameState Instance { get; private set; }
    public bool isLoaded = false;
    public override void _Ready()
    {
        Instance = this;
        // Initialize or load any necessary data here
    }
}