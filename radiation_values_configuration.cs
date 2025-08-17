using Godot;
using System;
using System.Collections.Generic;
//this a general note about input configurations
//any update that must be done in here should take into account some limitations
//like general coherance of the values and the fact that Program.cs
//while get most of the limitations from here
//it still use direct values when generating energy values when changing materials
//so always check that the values are coherent with the ones in Program.cs
public partial class radiation_values_configuration : HBoxContainer
{
    [Export]
    public SpinBox MaxRadiationValue;
    [Export]
    public SpinBox MinRadiationValue;
    public static Dictionary<string, double> radiationValuesLimits = new Dictionary<string, double>()

    {
        { "maxRadiationValue_MAX", 20.0 },
        { "maxRadiationValue_MIN", 1.0 },
        { "minRadiationValue_MAX", -10.0},
        { "minRadiationValue_MIN", -20.0 },
        { "step", 0.01 },
        { "defaultMaxRadiationValue", 10.0 },
        { "defaultMinRadiationValue", -10.0 }
    };
    public override void _Ready()
    {
        MaxRadiationValue.MaxValue = radiationValuesLimits["maxRadiationValue_MAX"];
        MaxRadiationValue.MinValue = radiationValuesLimits["maxRadiationValue_MIN"];
        MinRadiationValue.MaxValue = radiationValuesLimits["minRadiationValue_MAX"];
        GD.Print(MinRadiationValue.MaxValue);
        MinRadiationValue.MinValue = radiationValuesLimits["minRadiationValue_MIN"];
        MaxRadiationValue.Step = radiationValuesLimits["step"];
        MinRadiationValue.Step = radiationValuesLimits["step"];
        MaxRadiationValue.ValueChanged += UpdateMaxRadiationValue;
        MinRadiationValue.ValueChanged += UpdateMinRadiationValue;
        MaxRadiationValue.Value = radiationValuesLimits["defaultMaxRadiationValue"];
        MinRadiationValue.Value = radiationValuesLimits["defaultMinRadiationValue"];
    }
    public void UpdateMaxRadiationValue(double value)
    {
        if (value <= MinRadiationValue.Value)
        {
            MaxRadiationValue.Value = MinRadiationValue.Value + MaxRadiationValue.Step;
        }
        else if (value > MaxRadiationValue.MaxValue)
        {
            MaxRadiationValue.Value = MaxRadiationValue.MaxValue;
        }
        else if (value < MaxRadiationValue.MinValue)
        {
            MaxRadiationValue.Value = MaxRadiationValue.MinValue;
        }
    }
    public void UpdateMinRadiationValue(double value)
    {
        if (value >= MaxRadiationValue.Value)
        {
            MinRadiationValue.Value = MaxRadiationValue.Value - MinRadiationValue.Step;
        }
        else if (value < MinRadiationValue.MinValue)
        {
            MinRadiationValue.Value = MinRadiationValue.MinValue;
        }
        if (value > MinRadiationValue.MaxValue)
        {
            GD.PrintErr("MinRadiationValue exceeds its maximum limit.", MinRadiationValue.MaxValue);
            MinRadiationValue.Value = MinRadiationValue.MaxValue;
        }
    }


}
