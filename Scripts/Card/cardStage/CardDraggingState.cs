using Godot;
using System;

public partial class CardDraggingState : CardState
{
    public override void _Ready()
    {
        state = State.Dragging;
    }
    public override void Enter()
    {
        if (GetTree().GetFirstNodeInGroup("ui_layer") is CanvasLayer ui_layer)
            card_ui.Reparent(ui_layer);
        card_ui.color.Color = Color.FromHsv(210f / 360f, 0.85f, 0.35f);
        card_ui.state.Text = "Dragging";
    }
    
     public override void OnInput(InputEvent @event)
    {
        bool mouseMotion = @event is InputEventMouseMotion;
        bool cancel = @event.IsActionPressed("right_mouse");
        bool confirm = @event.IsActionReleased("left_mouse") || @event.IsActionPressed("left_mouse");
        if (mouseMotion)
        {
            card_ui.GlobalPosition = card_ui.GetGlobalMousePosition() - card_ui.PivotOffset;
        }
        else if (cancel)
        {
            EmitSignal("TransitionRequested", this, (int)State.Base);
        }
        else if (confirm)
        {
            EmitSignal("TransitionRequested", this, (int)State.Released);
        }
    }
}
