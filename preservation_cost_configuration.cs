using Godot;
using System;
using System.Collections.Generic;

public partial class preservation_cost_configuration : HBoxContainer
{
    [Export]
    public SpinBox max_preservation_cost;
    [Export]
    public SpinBox min_preservation_cost;
    public static Dictionary<string, double> preservation_costs_limits = new Dictionary<string, double>
    {
        { "max_preservation_cost", 100.0 },
        { "min_preservation_cost", 10.0 },
        { "max_default_preservation_cost", 100.0 },
        { "min_default_preservation_cost", 10.0 },
        {"max_preservation_cost_min",50.0 },
        {"step", 0.01 }
    };
    public override void _Ready()
    {
        max_preservation_cost.MaxValue = preservation_costs_limits["max_preservation_cost"];
        min_preservation_cost.MinValue = preservation_costs_limits["min_preservation_cost"];
        max_preservation_cost.MinValue = preservation_costs_limits["max_preservation_cost_min"];
        min_preservation_cost.Step = preservation_costs_limits["step"];
        max_preservation_cost.Step = preservation_costs_limits["step"];
        max_preservation_cost.Value = preservation_costs_limits["max_default_preservation_cost"];
        min_preservation_cost.Value = preservation_costs_limits["min_default_preservation_cost"];
        max_preservation_cost.ValueChanged += UpdateMaxPreservationCost;
        min_preservation_cost.ValueChanged += UpdateMinPreservationCost;
    }
    public void UpdateMaxPreservationCost(double value)
    {
        if (value < min_preservation_cost.Value)
        {
            max_preservation_cost.Value = min_preservation_cost.Value + preservation_costs_limits["step"];
        }
    }
    public void UpdateMinPreservationCost(double value)
    {
        if (value > max_preservation_cost.Value)
        {
            min_preservation_cost.Value = max_preservation_cost.Value - preservation_costs_limits["step"];
        }
    }
}
