using Godot;
using System;

public partial class Camera2d : Camera2D
{
    public float ZoomFactor { get; set; } = 1.0f;
    private bool isDragging = false;
    private Vector2 lastMousePosition;
    int counter = 0;
    public override void _Process(double delta)
    {
        base._Process(delta);
        counter++;
        //this is for the camera movement control using the arrow keys
        if (Input.IsActionPressed("ui_left"))
        {
            Position += new Vector2(-200 * (float)delta, 0);
        }
        if (Input.IsActionPressed("ui_right"))
        {
            Position += new Vector2(200 * (float)delta, 0);
        }
        if (Input.IsActionPressed("ui_up"))
        {
            Position += new Vector2(0, -200 * (float)delta);
        }
        if (Input.IsActionPressed("ui_down"))
        {
            Position += new Vector2(0, 200 * (float)delta);
        }
        //this is for the camera mvement control using the mouse
        //it allows the user to drag the camera around
        if (isDragging)
        {
            Vector2 currentMousePosition = GetViewport().GetMousePosition();
            Vector2 mouseDelta = currentMousePosition - lastMousePosition;
            Position -= mouseDelta;
            if (counter % 3 == 0)
            {
                lastMousePosition = currentMousePosition;
            }
        }
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        // Handle input events for zooming or panning
        //using the mouse wheel to zoom in and out
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                ZoomFactor *= 1.1f; // Zoom in
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                ZoomFactor /= 1.1f; // Zoom out
            }
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed)
                {
                    isDragging = true;
                    lastMousePosition = mouseEvent.Position;
                }
                else
                {
                    isDragging = false;
                }
            }
        }
        this.Zoom = new Vector2(ZoomFactor, ZoomFactor);
    }


}
