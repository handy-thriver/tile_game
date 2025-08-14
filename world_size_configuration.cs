using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class world_size_configuration : HBoxContainer
{
    [Export]
    public SpinBox world_width_input;
    [Export]
    public SpinBox world_length_input;
    public static Dictionary<string, int> SizeLimits = new Dictionary<string, int>()
    {
        {"maxLength", 1000},
        {"minLength", 100},
        {"lengthDefaultValue", 500},
        { "maxWidth", 1000},
        {"minWidth", 100},
        {"widthDefaultValue", 500}
    };
    public override void _Ready()
    {
        world_length_input.Value = SizeLimits["lengthDefaultValue"];
        world_width_input.Value = SizeLimits["widthDefaultValue"];
        world_length_input.MaxValue = SizeLimits["maxLength"];
        world_length_input.MinValue = SizeLimits["minLength"];
        world_width_input.MaxValue = SizeLimits["maxWidth"];
        world_width_input.MinValue = SizeLimits["minWidth"];
    }
        

}
