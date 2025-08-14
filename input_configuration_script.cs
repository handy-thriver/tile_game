using Godot;
using System;
using System.Collections.Generic;
using simulation_tools;
public partial class input_configuration_script : VBoxContainer
{
    [Export]
    public SpinBox world_width;
    [Export]
    public SpinBox world_length;
    [Export]
    public SpinBox radiation_diameter_min;
    [Export]
    public SpinBox radiation_diameter_max;
    [Export]
    public SpinBox radiation_values_min;
    [Export]
    public SpinBox radiation_values_max;
    [Export]
    public SpinBox preservation_cost_min;
    [Export]
    public SpinBox preservation_cost_max;
    [Export]
    public SpinBox materials_number;
    [Export]
    public Button back_button;
    [Export]
    public ConfirmationDialog back_confirmation_dialog;
    [Export]
    public Button next_button;
    [Export]
    public ConfirmationDialog next_confirmation_dialog;
    [Export]
    public LineEdit slot_name_input;
    Dictionary<string, object> WorldConfigurationVariables = new Dictionary<string, object>();
    public override void _Ready()
    {
        WorldConfigurationVariables = new Dictionary<string, object>()
        {
            { "worldWidth", world_width.Value},
            { "worldLength", world_length.Value},
            { "radiationDiameterMin", radiation_diameter_min.Value},
            { "radiationDiameterMax", radiation_diameter_max.Value},
            { "radiationValuesMin", radiation_values_min.Value},
            { "radiationValuesMax", radiation_values_max.Value},
            { "preservationCostMin", preservation_cost_min.Value},
            { "preservationCostMax", preservation_cost_max.Value},
            { "materialsNumber", materials_number.Value },
            { "slotName", slot_name_input.Text }
        };
        slot_name_input.Text = "Slot " + NameState.Instance.slot_ID.ToString();
        world_width.ValueChanged += update_world_width;
        world_length.ValueChanged += update_world_length;
        radiation_diameter_min.ValueChanged += update_radiation_diameter_min;
        radiation_diameter_max.ValueChanged += update_radiation_diameter_max;
        radiation_values_min.ValueChanged += update_radiation_values_min;
        radiation_values_max.ValueChanged += update_radiation_values_max;
        preservation_cost_min.ValueChanged += update_preservation_cost_min;
        preservation_cost_max.ValueChanged += update_preservation_cost_max;
        materials_number.ValueChanged += update_materials_number;
        slot_name_input.TextChanged += update_slot_name;
        back_button.Pressed += back_button_pressed;
        back_confirmation_dialog.Confirmed += on_back_confirmation_dialog_confirmed;
        next_button.Pressed += next_button_pressed;
        next_confirmation_dialog.Confirmed += on_next_confirmation_dialog_confirmed;
    }
    public void on_back_confirmation_dialog_confirmed()
    {
        GetTree().ChangeSceneToFile("res://slot_menu.tscn");
    }
    public void back_button_pressed()
    {
        back_confirmation_dialog.PopupCentered();
    }
    public void update_materials_number(double value)
    {
        WorldConfigurationVariables["materialsNumber"] = value;
    }
    public void update_world_width(double value)
    {
        WorldConfigurationVariables["worldWidth"] = value;
    }
    public void update_world_length(double value)
    {
        WorldConfigurationVariables["worldLength"] = value;
    }
    public void update_radiation_diameter_min(double value)
    {
        WorldConfigurationVariables["radiationDiameterMin"] = value;
    }
    public void update_radiation_diameter_max(double value)
    {
        WorldConfigurationVariables["radiationDiameterMax"] = value;
    }
    public void update_radiation_values_min(double value)
    {
        WorldConfigurationVariables["radiationValuesMin"] = value;
    }
    public void update_radiation_values_max(double value)
    {
        WorldConfigurationVariables["radiationValuesMax"] = value;
    }
    public void update_preservation_cost_min(double value)
    {
        WorldConfigurationVariables["preservationCostMin"] = value;
    }
    public void update_preservation_cost_max(double value)
    {
        WorldConfigurationVariables["preservationCostMax"] = value;
    }
    public void update_slot_name(string value)
    {
        WorldConfigurationVariables["slotName"] = value;
    }
    public void globlize_change()
    {
        NameState.Instance.materialLimits = new material_limits(
            (int)(double)WorldConfigurationVariables["radiationDiameterMin"],
            (int)(double)WorldConfigurationVariables["radiationDiameterMax"],
            (double)WorldConfigurationVariables["preservationCostMin"],
            (double)WorldConfigurationVariables["preservationCostMax"],
            (double)WorldConfigurationVariables["radiationValuesMin"],
            (double)WorldConfigurationVariables["radiationValuesMax"]
        );
        NameState.Instance.width = (int)(double)WorldConfigurationVariables["worldWidth"];
        NameState.Instance.length = (int)(double)WorldConfigurationVariables["worldLength"];
        NameState.Instance.number_of_materials = (int)(double)WorldConfigurationVariables["materialsNumber"];
        NameState.Instance.slot_name = (string)WorldConfigurationVariables["slotName"];
    }
    public void next_button_pressed()
    {
        globlize_change();
        Dictionary<string, object> slotData = new Dictionary<string, object>
        {
            { "name", NameState.Instance.slot_name },
            { "width", NameState.Instance.width },
            { "length", NameState.Instance.length }
        };
        string slotPath = System.IO.Path.Combine(NameState.Instance.global_path,
         "Slot_" + NameState.Instance.slot_ID.ToString(), "slot_data.json");
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(slotPath));
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(slotData);
        System.IO.File.WriteAllText(slotPath, jsonContent);
        next_confirmation_dialog.PopupCentered();
    }
    public void on_next_confirmation_dialog_confirmed()
    {
        GetTree().ChangeSceneToFile("res://map_scene.tscn");
    }
}

