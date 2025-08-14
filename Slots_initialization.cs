using Godot;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text.Json;

public partial class Slots_initialization : OptionButton
{
    public string slotpath = ProjectSettings.GlobalizePath("user://slots/");
    public override void _Ready()
    {
        //this is necessary to ensure the slots directory exists
        base._Ready();
        //we use refresh because the slots are not initialized at the start
        //and because we will initialize them later more than once depending on the game state
        refresh();
    }
    public  void refresh()
    {
        //clear the current items in the OptionButton
        //necessary to avoid duplicates and ensure we only show current slots
        Clear();
        for (int i = 1; i <= 20; i++)
        {
            string slotName = "Slot_" + i;
            string slotPath = Path.Combine(slotpath, slotName, "slot_data.json");
            //check if the slot file exists
            //if it does, read the content and add it to the OptionButton so that the user can see it
            //and choose if he want to load it
            if (File.Exists(slotPath))
            {
                string fileContent = File.ReadAllText(slotPath);//the content of the file is a dictionary
                Dictionary<string, object> slotData = JsonSerializer.Deserialize<Dictionary<string, object>>(fileContent);
                //slorData is a dictionary with the following keys
                //name, width, lengths of types string, int, int respectively
                //we don't need to convert them back to a specific type snce we will only use them as strings
                //for the text of the OptionButton
                string name = slotData["name"].ToString();
                string width = slotData["width"].ToString();
                string length = slotData["length"].ToString();
                AddItem($"name:{name} size: ({width}x{length})", i);
            }
            else
            {
                AddItem(slotName + " [Empty]", i);
            }
        }
    }
}
