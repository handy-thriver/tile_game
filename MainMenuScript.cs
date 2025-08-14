using Godot;
using System;

public partial class MainMenuScript : Control
{
    [Export]
    public Button play_button;
    public override void _Ready()
    {
        base._Ready();
        play_button.Pressed += Button_perssed;
    }

    private void Button_perssed()
    {
        // This function is called when the play button is pressed
        // It will start the game by changing the scene to the slot selection scene
        string slot_path = "res://slot_menu.tscn";
        GetTree().ChangeSceneToFile(slot_path);
    }
}
 