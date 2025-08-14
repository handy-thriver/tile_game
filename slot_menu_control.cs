using Godot;
using System;

// this script opt to  manage the slot selection and the slots deletion process
public partial class slot_menu_control : VBoxContainer
{
    [Export]
    public Button NextButton;
    [Export]
    public ConfirmationDialog NextDialog;
    [Export]
    public Button backButton;
    [Export]
    public OptionButton slotOptionButton;
    [Export]
    public Button deleteButton;
    [Export]
    public ConfirmationDialog deleteDialog;
    [Export]
    // DeleteSlots is a custom class that manages the slots deletion process
    // its main extra feature is the refresh function whichis neccesary after making change
    public DeleteSlots deleteSlotList;
    [Export]
    //S
    public Slots_initialization slot_options;
    public override void _Ready()
    {
        base._Ready();
        NextButton.Pressed += OnNextButtonPressed;
        backButton.Pressed += OnBackButtonPressed;
        NextDialog.Confirmed += OnNextDialogConfirmed;
        deleteButton.Pressed += OnDeleteButtonPressed;
        deleteDialog.Confirmed += delete_slots;
    }
    private void OnNextButtonPressed()
    {
        if (NextDialog != null)
        {
            NextDialog.PopupCentered();
        }
        else
        {
            GD.Print("NextDialog is not set.");
        }
    }
    private void OnBackButtonPressed()
    {
        // Go back to the main menu
        GetTree().ChangeSceneToFile("res://main_menu.tscn");
    }
    private void OnNextDialogConfirmed()
    {
        //the communication with map_scene and world_configuration is done through the NameState singleton
        int slot_ID = slotOptionButton.GetSelectedId();
        NameState.Instance.slot_ID = slot_ID;
        string slot_name = "slot_" + slot_ID;
        // Check if the directory for the slot exists
        // if it exists, load the scene, which mean load the user data which is stored in the slot
        // if it does not exist, go to the world_configuration scene to create a new slot and save new user data
        if (DirAccess.DirExistsAbsolute("user://slots/" + slot_name))
        {
            NameState.Instance.slot_name = slot_name;
            NameState.Instance.slot_ID = slot_ID;
            NameState.Instance.isLoaded = true;
            GetTree().ChangeSceneToFile("res://map_scene.tscn");
        }
        else
        {
            GetTree().ChangeSceneToFile("res://world_configuration.tscn");
        }
    }
    private void OnDeleteButtonPressed()
    {
        if (deleteDialog != null)
        {
            deleteDialog.PopupCentered();
        }
        else
        {
            GD.Print("DeleteDialog is not set.");
        }
    }
    private void delete_slots()
    {
        // This function is called when the user confirms the deletion of slots
        //one or more slots can be selected for deletion at once
        int[] deleted_slots = deleteSlotList.GetSelectedItems();
        GD.Print("Deleted slots: " + string.Join(", ", deleted_slots));
        Array.Reverse(deleted_slots);
        foreach (int slot_id in deleted_slots)
        {
            // For each selected slot, to get the slot name AKA slot_index, we need to get the text of the item
            string slot_name = deleteSlotList.GetItemText(slot_id).Split(':')[0].Trim();
            string slot_path = ProjectSettings.GlobalizePath("user://slots/" + slot_name);
            if (DirAccess.DirExistsAbsolute(slot_path))
            {
                System.IO.Directory.Delete(slot_path, true);
            }
        }
        // Refresh the slot list and options after deletion
        // so the user can see the changes immediately
        deleteSlotList.refresh();
        slot_options.refresh();
    }
}
