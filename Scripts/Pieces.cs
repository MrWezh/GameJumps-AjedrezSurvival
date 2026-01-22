using Godot;
using System;

public partial class Pieces : Node
{
    public Texture2D mainCharacterTexture = 
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/king/king_00.png");

	public Texture2D blackBishop = 
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/bishop/PeonNegro.png");

	public Texture2D BlackKing =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/king/king_01.png");

	public Texture2D BlackKnight =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/knight/knight_01.png");
	public Texture2D BlackPawn =
	ResourceLoader.Load<Texture2D>("res://Assets/pieces/pawn/pawn_01.png");
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

}
