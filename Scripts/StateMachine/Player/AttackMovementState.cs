using Godot;
using System;

public partial class AttackMovementState : State
{
	private ReyBranco _player;
	private AnimatableBody2D animatedSprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetParent().GetParent().GetNode<ReyBranco>("ReyBranco");
		animatedSprite = _player.GetNode<AnimatableBody2D>("Habilidades");
		_player.setAnimation("attack");
		
		
	}


	    public void PlayAttack(Vector2 dir)
    {
        if (animatedSprite == null) return;

        // normalizar dirección a componentes enteras -1,0,1
        int dx = Math.Sign(dir.X);
        int dy = Math.Sign(dir.Y);

        // mapear a ángulo en grados (0 = arriba)
        float angleDeg = 0f;
        if (dx == 0 && dy == -1)      angleDeg = 0f;    // arriba
        else if (dx == 1 && dy == 0)  angleDeg = 90f;   // derecha
        else if (dx == 0 && dy == 1)  angleDeg = 180f;  // abajo
        else if (dx == -1 && dy == 0) angleDeg = -90f;  // izquierda
        else
            angleDeg = 0f; // fallback para diagonales u otros casos

        // aplicar rotación al sprite/elemento de "Habilidades"
        animatedSprite.RotationDegrees = angleDeg;

    }
}