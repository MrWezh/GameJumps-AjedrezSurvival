using Godot;
using System;
using System.Data;

public partial class Pieces : Node
{
	// Cargas estáticas (cached) para evitar múltiples lecturas de recursos
	private static readonly Texture2D sMainCharacter = GD.Load<Texture2D>("res://Assets/pieces/ReyBlanco.png");
	private static readonly Texture2D sBlackBishop = GD.Load<Texture2D>("res://Assets/pieces/AlfilNegro.png");
	private static readonly Texture2D sBlackKing = GD.Load<Texture2D>("res://Assets/pieces/ReyNegro.png");
	private static readonly Texture2D sBlackKnight = GD.Load<Texture2D>("res://Assets/pieces/CaballoNegro.png");
	private static readonly Texture2D sBlackPawn = GD.Load<Texture2D>("res://Assets/pieces/PeonNegro.png");
	private static readonly Texture2D sBlackQueen = GD.Load<Texture2D>("res://Assets/pieces/ReinaNegraNegro.png");
	private static readonly Texture2D sBlackRook = GD.Load<Texture2D>("res://Assets/pieces/TorreNegra.png");

	private static readonly PackedScene sTexturePlaceholder = GD.Load<PackedScene>("res://Scenes/texture_placerholder.tscn");

	// Exponer como propiedades de instancia para mantener compatibilidad con el código existente
	public Texture2D mainCharacterTexture => sMainCharacter;
	public Texture2D BlackBishop => sBlackBishop;
	public Texture2D BlackKing => sBlackKing;
	public Texture2D BlackKnight => sBlackKnight;
	public Texture2D BlackPawn => sBlackPawn;
	public Texture2D BlackQueen => sBlackQueen;
	public Texture2D BlackRook => sBlackRook;

	public PackedScene TEXTURE_PLACEHOLDER => sTexturePlaceholder;

	public override void _Ready()
	{
		// No se necesita inicialización en tiempo de ejecución; recursos ya cargados de forma estática.
	}

	// Función para redimensionar texturas (utilizar con precaución; cachear resultados si se usa frecuentemente)
	public static Texture2D ResizeTexture(Texture2D texture)
	{
		if (texture == null)
			return null;

<<<<<<< HEAD
		Image image = texture.GetImage();
		image.Resize(18, 18, Image.Interpolation.Lanczos);
		return ImageTexture.CreateFromImage(image);
	}
=======
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

	
	

>>>>>>> 96df82c0a06916d586faf820743211dcdd2b8563
}
