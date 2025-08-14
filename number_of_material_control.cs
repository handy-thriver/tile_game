using Godot;
using System;
using System.Collections.Generic;

public partial class number_of_material_control : HBoxContainer
{
    [Export]
    public SpinBox number_of_materials;
    public static Dictionary<string, int> NumberOfMaterialsLimits = new Dictionary<string, int>()
    {
        { "min", 2},
        { "max", 100 },
        { "default", 50 }
    };
    public override void _Ready()
    {
        number_of_materials.Value = NumberOfMaterialsLimits["default"];
        number_of_materials.MaxValue = NumberOfMaterialsLimits["max"];
        number_of_materials.MinValue = NumberOfMaterialsLimits["min"];
    }
}
