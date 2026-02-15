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
        if (dx == -1 && dy == -1)      angleDeg = -45f;    // arriba izquierda (diagonal)
        else if (dx == 1 && dy == -1) angleDeg = 45f;   // arriba derecha (diagonal)
        else if (dx == 1 && dy == 1)  angleDeg = 135f;   // abajo derecha (diagonal)
        else if (dx == -1 && dy == 1) angleDeg = -135f;  // abajo izquierda (diagonal)
        else
            angleDeg = 0f; // fallback para diagonales u otros casos

        // aplicar rotación al sprite/elemento de "Habilidades"
        animatedSprite.RotationDegrees = angleDeg;
        // reproducir animación de ataque (asumiendo que se llama "Attack")
        setAnimation("shoot");
    }

    public void PlayFireball(Vector2 targetLocal, double duration = 0.5)
    {
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

        // reproducir animación de lanzamiento si existe
        setAnimation("fireBall");
    }
}
