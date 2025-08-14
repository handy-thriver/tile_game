using Godot;
using System;
using System.Collections.Generic;

public partial class button_configuration_script : HBoxContainer
{
    [Export]
    public Button NextButton;
    [Export]
    public Button BackButton;
    [Export]
    public ConfirmationDialog NextDialog;
    [Export]
    public ConfirmationDialog BackDialog;
    public override void _Ready()
    {
        NextButton.Pressed += OnNextButtonPressed;
        BackButton.Pressed += OnBackButtonPressed;
    }
    public void OnNextButtonPressed()
    {
        NextDialog.PopupCentered();
    }
    public void OnBackButtonPressed()
    {
        BackDialog.PopupCentered();
    }
}
