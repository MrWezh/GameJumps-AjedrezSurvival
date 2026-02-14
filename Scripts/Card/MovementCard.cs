using Godot;
using System;

/// <summary>
/// Carta base para movimiento.
/// Se puede usar para Moviment1 y Moviment2, o crear subclases específicas.
/// </summary>
public partial class MovementCard : CardUI
{
    public override void _Ready()
    {
        base._Ready();
    }

    public void OnCardPlayed(Area2D target)
    {
        GD.Print("MovementCard played: " + Name);
        
        var player = GetTree().Root.GetNode<ReyBranco>("ReyBlanco");
        if (player == null)
        {
            GD.PrintErr("ReyBranco not found!");
            return;
        }

        if (target != null)
        {
            // Mover hacia el target
            player.MoveToLocalPosition(target.GlobalPosition);
        }
    }
}
