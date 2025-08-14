using Godot;
using System;
using System.Collections.Generic;

public partial class radiation_diameter_configuration : HBoxContainer
{
    [Export]
    public SpinBox min_range;
    [Export]
    public SpinBox max_range;
    public static Dictionary<string, int> SizeLimits = new Dictionary<string, int>()
    {
        {"maxDiameter", 20},
        {"minDiameter", 5},
        {"step", 1},
        { "defaultDiameterMin", 5},
        {"defaultDiameterMax", 20}
    };
    public override void _Ready()
    {
        min_range.Value = SizeLimits["defaultDiameterMin"];
        max_range.Value = SizeLimits["defaultDiameterMax"];
        min_range.MaxValue = SizeLimits["maxDiameter"];
        max_range.MaxValue = SizeLimits["maxDiameter"];
        min_range.MinValue = SizeLimits["minDiameter"];
        max_range.MinValue = SizeLimits["minDiameter"];
        min_range.Step = SizeLimits["step"];
        max_range.Step = SizeLimits["step"];
        min_range.ValueChanged += update_min_range;
        max_range.ValueChanged += update_max_range;
    }

    public void update_min_range(double value)
    {
        if (value >= max_range.Value)
        {
            min_range.Value = max_range.Value - min_range.Step;
        }
        else if (value < min_range.MinValue)
        {
            min_range.Value = min_range.MinValue;
        }
    }
    public void update_max_range(double value)
    {
        if (value <= min_range.Value)
        {
            max_range.Value = min_range.Value + max_range.Step;
        }
        else if (value > max_range.MaxValue)
        {
            max_range.Value = max_range.MaxValue;
        }
    }
        
    
}
