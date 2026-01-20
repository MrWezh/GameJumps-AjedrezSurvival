using Godot;
using System;

public partial class Tablero : Node2D
{
	//private const string[,] _tablero;

	private Vector2 _TamañoCasilla = new Vector2(90, 90);
	private Texture2D _texturaCasillaBlanca;
	private Texture2D _texturaCasillaNegra;

	public override void _Ready()
	{
		_texturaCasillaBlanca = GD.Load<Texture2D>("res://Assets/Textures/CasillaBlanca.png");
		_texturaCasillaNegra = GD.Load<Texture2D>("res://Assets/Textures/CasillaNegra.png");
	}

	
	public override void _Process(double delta)
	{
	} 

public override void _Draw()
    {

        for (int i = 0; i < 8; i++)
        {
           for (int j = 0; j < 8; j++)
            {
                var pos = new Vector2(i * _TamañoCasilla.X, j * _TamañoCasilla.Y);
                var rect = new Rect2(pos, _TamañoCasilla);

                if ((i + j) % 2 == 0)
                    DrawTextureRect(_texturaCasillaBlanca, rect, false);
                else
                    DrawTextureRect(_texturaCasillaNegra, rect, false);
            }
        }
    }
}
