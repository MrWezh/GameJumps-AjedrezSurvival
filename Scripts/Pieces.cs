using Godot;
using System;

public partial class Pieces : Node
{

	public Texture2D ResizeTexture(Texture2D texture)
    {
        // Crear una nueva imagen redimensionada
        Image image = texture.GetImage();
        image.Resize(48, 48, Image.Interpolation.Lanczos);
        
        // Crear nueva textura con la imagen redimensionada
        ImageTexture resizedTexture = ImageTexture.CreateFromImage(image);
        
        // Aplicar la nueva textura
        return resizedTexture;
    }
    public Texture2D mainCharacterTexture = 
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/king/king_00.png");

	public Texture2D blackBishop = 
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/bishop/bishop_01.png");
	
	
	public Texture2D BlackKing =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/king/king_01.png");

	public Texture2D BlackKnight =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/knight/knight_01.png");
	public Texture2D BlackPawn =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/bishop/PeonNegro.png");
	public Texture2D BlackQueen =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/queen/queen_01.png");
	public Texture2D BlackRook =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/rook/rook_01.png");

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

	const int blackPawnID = 1;
	const int blackRookID = 2;
	const int blackKnightID = 3;
	const int blackBishopID = 4;
	const int blackQueenID = 5;
	const int blackKingID = 6;

	public PackedScene TEXTURE_PLACEHOLDER = 
		GD.Load<PackedScene>("res://Scenes/texture_placerholder.tscn");


	   
	public void _ready()
	{
		

	}


}
