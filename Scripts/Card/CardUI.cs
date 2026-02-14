using Godot;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public partial class CardUI : Control
{
    [Signal] public delegate void ReparentRequestEventHandler(CardUI whichCardUi);
    public ColorRect color => GetNode<ColorRect>("Borde");
    public Label state => GetNode<Label>("State");
    public Area2D DropPointDetector => GetNode<Area2D>("DropPointDetector");
    public CardStateMachine stateMachine => GetNode<CardStateMachine>("CardStateMachine");
    public Godot.Collections.Array<Node> targets = [];
    public override void _Ready()
    {
        stateMachine.Init(this);
    }
    public override void _Input(InputEvent @event)
    {
        stateMachine.OnInput(@event);
    }
    public override void _GuiInput(InputEvent @event)
    {
        stateMachine.OnGuiInput(@event);
    }
    public void _MouseEntered()
    {
        stateMachine.OnMouseEntered();
    }
    public void _MouseExited()
    {
        stateMachine.OnMouseExited();
    }
    public void _on_drop_point_entered(Area2D area)
    {
        if (!targets.Contains(area))
            targets.Add(area);
    }
    public void _on_drop_point_exited(Area2D area)
    {
        if (area.IsInGroup("drop_point"))
        {
            targets.Remove(area);
        }
    }
}
