using Godot;
using System;

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

		Image image = texture.GetImage();
		image.Resize(18, 18, Image.Interpolation.Lanczos);
		return ImageTexture.CreateFromImage(image);
	}
}
