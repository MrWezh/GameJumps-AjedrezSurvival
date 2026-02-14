using Godot;
using System;
using System.Threading.Tasks;

public partial class CardBaseState : CardState
{
    public override void _Ready()
    {
        state = State.Base;
    }
    public override void Enter()
    {
        card_ui.EmitSignal("ReparentRequest", card_ui);
        card_ui.color.Color = Color.FromHsv(140f/360f, 0.78f, 0.88f);
        card_ui.state.Text = "Base";
        card_ui.PivotOffset = Vector2.Zero;
    }
    public override void OnGuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("left_mouse"))
        {
            card_ui.PivotOffset = card_ui.GetGlobalMousePosition() - card_ui.GlobalPosition;
            EmitSignal("TransitionRequested", this, (int)State.Clicked);
        }
    }
}
