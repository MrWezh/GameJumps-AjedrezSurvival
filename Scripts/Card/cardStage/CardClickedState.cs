using Godot;
using System;

public partial class CardClickedState : CardState
{
    public override void _Ready()
    {
        state = State.Clicked;
    }
    public override void Enter()
    {
        card_ui.color.Color = Color.FromHsv(30f/360f, 1f, 1f);
        card_ui.state.Text = "Clicked";
        card_ui.DropPointDetector.Monitoring = true;
    }

    public override void OnInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
            EmitSignal("TransitionRequested", this, (int)State.Dragging);
    }
}
