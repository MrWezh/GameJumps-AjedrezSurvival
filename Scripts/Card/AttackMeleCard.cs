using Godot;
using System;

/// <summary>
/// Carta de ataque cuerpo a cuerpo (espada).
/// Hereda de CardUI para mantener la misma máquina de estados y lógica de drag.
/// </summary>
public partial class AttackMeleCard : CardUI
{
    public override void _Ready()
    {
        base._Ready();
    }

    /// <summary>
    /// Llamado desde Hand cuando esta carta es jugada.
    /// </summary>
    public void OnCardPlayed(Area2D target)
    {
        GD.Print("AttackMeleCard played!");
        
        // Obtener una referencia al ReyBranco (está en root)
        ReyBranco player = (ReyBranco)GetTree().Root.GetNode<CharacterBody2D>("ReyBlanco");
        if (player == null)
        {
            GD.PrintErr("ReyBranco not found!");
            return;
        }

        // Calcular dirección hacia el target y ejecutar el ataque
        if (target != null)
        {
            Vector2 direction = (target.GlobalPosition - player.GlobalPosition).Normalized();
            player.PlayAttack(direction);
        }
        else
        {
            // Si no hay target, atacar hacia adelante (arriba)
            player.PlayAttack(Vector2.Up);
        }
    }
}
