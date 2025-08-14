using Godot;
using System;

public partial class MainMenuEnergyPulse : TextureRect
{
    private float _time = 0.0f;
    private ShaderMaterial _shaderMaterial;

    public override void _Ready()
    {
        // Ensure this TextureRect has a ShaderMaterial applied
        _shaderMaterial = Material as ShaderMaterial;

        if (_shaderMaterial == null)
        {
            GD.PrintErr("ShaderMaterial not found on MainMenuEnergyPulse.");
        }
    }

    public override void _Process(double delta)
    {
        if (_shaderMaterial != null)
        {
            _time += (float)delta;
            _shaderMaterial.SetShaderParameter("timer", _time);
        }
    }
}
