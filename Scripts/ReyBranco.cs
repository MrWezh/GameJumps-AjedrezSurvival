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

    public void PlayMeleAttack(Vector2 dir)
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
        // reproducir animación de ataque (asumiendo que se llama "Attack")
        setAnimation("attack");

    }

    
    public void PlayRangedAttack(Vector2 dir)
    {
        if (animatedSprite == null) return;

        // normalizar dirección a componentes enteras -1,0,1
        int dx = Math.Sign(dir.X);
        int dy = Math.Sign(dir.Y);

        // mapear a ángulo en grados (0 = arriba)
        float angleDeg = 0f;
        
        // Direcciones cardinales (arriba, derecha, abajo, izquierda)
        if (dx == 0 && dy == -1)      angleDeg = 0f;      // arriba
        else if (dx == 1 && dy == 0)  angleDeg = 90f;     // derecha
        else if (dx == 0 && dy == 1)  angleDeg = 180f;    // abajo
        else if (dx == -1 && dy == 0) angleDeg = -90f;    // izquierda (o 270f)
        
        // Direcciones diagonales
        else if (dx == 1 && dy == -1)  angleDeg = 45f;    // arriba-derecha
        else if (dx == 1 && dy == 1)   angleDeg = 135f;   // abajo-derecha
        else if (dx == -1 && dy == 1)  angleDeg = -135f;  // abajo-izquierda (o 225f)
        else if (dx == -1 && dy == -1) angleDeg = -45f;   // arriba-izquierda (o 315f)
        else
            angleDeg = 0f; // fallback

        // aplicar rotación al sprite/elemento de "Habilidades"
        animatedSprite.RotationDegrees = angleDeg;
        // reproducir animación de ataque (asumiendo que se llama "shoot")
        setAnimation("shoot");
    }

    public void PlayFireball(Vector2 targetLocal, double duration = 0.5)
    {
        // Calcular la dirección hacia el objetivo
        Vector2 direction = (targetLocal - this.Position).Normalized();
        int dx = Math.Sign(direction.X);
        int dy = Math.Sign(direction.Y);
        
        // Calcular el ángulo de rotación hacia el objetivo
        float angleDeg = 0f;
        
        // Direcciones cardinales
        if (dx == 0 && dy == -1)      angleDeg = 0f;      // arriba
        else if (dx == 1 && dy == 0)  angleDeg = 90f;     // derecha
        else if (dx == 0 && dy == 1)  angleDeg = 180f;    // abajo
        else if (dx == -1 && dy == 0) angleDeg = -90f;    // izquierda
        
        // Direcciones diagonales
        else if (dx == 1 && dy == -1)  angleDeg = 45f;    // arriba-derecha
        else if (dx == 1 && dy == 1)   angleDeg = 135f;   // abajo-derecha
        else if (dx == -1 && dy == 1)  angleDeg = -135f;  // abajo-izquierda
        else if (dx == -1 && dy == -1) angleDeg = -45f;   // arriba-izquierda
        else
            angleDeg = 0f; // fallback

        // Rotar el sprite del jugador hacia el objetivo
        if (animatedSprite != null)
        {
            animatedSprite.RotationDegrees = angleDeg - 90f; // Ajuste de rotación para que la bola apunte hacia adelante
        }

        // reproducir animación de lanzamiento si existe
        setAnimation("fireBall");

        // Crea un sprite temporal (bola de fuego) y lo mueve desde la posición actual hasta targetLocal
        if (GetParent() == null) return;

        var fire = new Sprite2D();
        Texture2D tex = null;
        try
        {
            tex = GD.Load<Texture2D>("res://Assets/Habilidades/bolaDiFuego.png");
        }
        catch { tex = null; }
        fire.Texture = tex;
        // usar la posición local del rey (ya en coordenadas del padre)
        fire.Position = this.Position;
        
        GetParent().AddChild(fire);

        var tween = CreateTween();
        tween.TweenProperty(fire, "position", targetLocal, duration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
        tween.TweenCallback(Callable.From(() => { if (IsInstanceValid(fire)) fire.QueueFree(); }));
    }
}