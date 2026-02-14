using Godot;
using System;

public partial class CardReleaseState : CardState
{
    bool played;
    public override void _Ready()
    {
        state = State.Released;
    }
    
    public override void Enter()
    {
        card_ui.color.Color = Color.FromHsv(270f / 360f, 0.85f, 0.35f);
        card_ui.state.Text = "Released";
        card_ui.DropPointDetector.Monitoring = false;
        
        played = false;

        if (card_ui.targets.Count > 0)
        {
            GD.Print("Played card on target: " + card_ui.targets[0].Name);
            played = true;
            // emitir señal para notificar que la carta fue jugada
            if (card_ui.targets[0] is Area2D area)
                card_ui.EmitSignal("CardPlayed", card_ui, area);
        }
    }
    public override void OnInput(InputEvent @event)
    {
        if (played)
            return;
        EmitSignal("TransitionRequested", this, (int)State.Base);
    }
}