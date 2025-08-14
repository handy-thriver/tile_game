using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
public class slotData
{
    public string name { get; set; }
    public int width { get; set; }
    public int length { get; set; }
}
public partial class DeleteSlots : ItemList
{
    public override void _Ready()
    {
        refresh();
    }
    public void refresh()
    {
        // Clear the current items in the ItemList
        // Necessary to avoid duplicates and ensure we only show current slots available for deletion
        Clear();
        string slot_path = ProjectSettings.GlobalizePath("user://slots/");
        //to  get the access to the slots directory
        // we use DirAccess to list the directories and know the available slots
        DirAccess slot_directory = DirAccess.Open(slot_path);
        if (slot_directory == null)
        {
            GD.PrintErr("Failed to open slot directory: " + slot_path);
            return;
        }
        else
        {
            // Get the list of directories in the slot path
            // this is not a list of global paths, but rather the names of the directories only
            string[] slot_names = slot_directory.GetDirectories();
            for (int i = 0; i < slot_names.Length; i++)
            {
                string slot_name = slot_names[i];
                if (slot_name != "." && slot_name != "..")
                {
                    //we need to get the full path
                    string full_path = System.IO.Path.Combine(slot_path, slot_name);
                    //the data needed for basic slot text is stored in slot_data.json
                    //as a slotData object which have the following properties:
                    // name, width, length of types string, int, int respectively
                    string slot_data_path = System.IO.Path.Combine(full_path, "slot_data.json");
                    string slot_data_json = System.IO.File.ReadAllText(slot_data_path);
                    slotData slot_data = JsonSerializer.Deserialize<slotData>(slot_data_json);
                    string slot_display_name = slot_data.name;
                    int width = slot_data.width;
                    int length = slot_data.length;
                    // a very important note is that the slot name is not the same as the display name
                    // the slot name is the directory name needed to access the slot for deletion afterwords
                    // the display name is the name of the slot that the user have chosen
                    AddItem($"{slot_name}: name{slot_display_name} size: {width}x{length}", null);
                }
            }
        }
    }
}
