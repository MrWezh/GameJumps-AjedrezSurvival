// ...existing code...
using Godot;
using System;
using System.Collections.Generic;

public partial class ReyBranco : CharacterBody2D
{

    public AnimatedSprite2D animatedSprite;
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("Habilidades"); 

    }
    // llamado por Board para mover/posicionar al jugador en coordenadas LOCALES del tablero
    public void MoveToLocalPosition(Vector2 localPos)
    {
        Position = localPos;
    }

    // llamado por Board para reproducir la animación de ataque en una dirección (dx,dy) con componentes -1..0..

    public void setAnimation(string animName)
    {
        if (animatedSprite != null)
        {
            animatedSprite.Play(animName);
        }
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

    // NUEVO: devuelve las 4 direcciones relativas alrededor del jugador (orden: arriba, derecha, abajo, izquierda)
    public List<Vector2> GetAttackDirections()
    {
        return new List<Vector2> {
            new Vector2(0, -1),   // arriba
            new Vector2(1, 0),    // derecha
            new Vector2(0, 1),    // abajo
            new Vector2(-1, 0),   // izquierda
        };
    }
}
