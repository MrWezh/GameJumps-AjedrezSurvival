using Godot;
using System;
using System.Data;

public partial class Pieces : Node
{

		public void _ready()
	{
	}


	// funcion para redimensionar texturas
	public Texture2D ResizeTexture(Texture2D texture)
    {
        // Crear una nueva imagen redimensionada
        Image image = texture.GetImage();
        image.Resize(18, 18, Image.Interpolation.Lanczos);
        
        // Crear nueva textura con la imagen redimensionada
        ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
        
        // Aplicar la nueva textura
        return resizedTexture;
    }
	// cargar texturas de las piezas

    public Texture2D mainCharacterTexture = 
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/ReyBlanco.png");

	public Texture2D BlackBishop = 
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/AlfilNegro.png");
	public Texture2D BlackKing =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/ReyNegro.png");
	public Texture2D BlackKnight =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/CaballoNegro.png");
	public Texture2D BlackPawn =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/PeonNegro.png");
	public Texture2D BlackQueen =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/ReinaNegraNegro.png");
	public Texture2D BlackRook =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/TorreNegra.png");

	public Texture2D PIECES_MOVES =
	ResourceLoader.Load<Texture2D>("res://Assets/Piece_move.png");

	/*
	pieces
	[max: 6]
	pawn = 1
	rook = 2
	knight = 3
	bishop = 4
	queen = 5
	king = 6
	*/

	public PackedScene TEXTURE_PLACEHOLDER = 
		GD.Load<PackedScene>("res://Scenes/texture_placerholder.tscn");

	
	

}
