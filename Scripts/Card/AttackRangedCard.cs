using Godot;
using System;

/// <summary>
/// Carta de ataque a distancia (arco).
/// </summary>
public partial class AttackRangedCard : CardUI
{
    public override void _Ready()
    {
        base._Ready();
    }

    public void OnCardPlayed(Area2D target)
    {
        GD.Print("AttackRangedCard played!");
        
        var player = GetTree().Root.GetNode<ReyBranco>("ReyBlanco");
        if (player == null)
        {
            GD.PrintErr("ReyBranco not found!");
            return;
        }

        if (target != null)
        {
            Vector2 direction = (target.GlobalPosition - player.GlobalPosition).Normalized();
            player.PlayAttack(direction);
        }
        else
        {
            player.PlayAttack(Vector2.Up);
        }
    }
}
