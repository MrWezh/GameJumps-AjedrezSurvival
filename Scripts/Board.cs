using Godot;
using Godot.NativeInterop;
using System;

public partial class Board : Sprite2D
{
    private const int BOARD_SIZE = 8;
    private const int CELL_WIDTH = 62;

    private Pieces _piecesTexture;

      private int[,] _board;

    enum StateMachine
    {
        Moving,
        Moved, 
        None
    }


   
    private StateMachine _state;
    private Vector2I _selectedPiece;
    private bool _isWhiteTurn = false;
        
    [Export]
    private Node2D _pieces;
    private Node2D _dots;
    private Sprite2D _turn;


	public override void _Ready()
    {
        _piecesTexture = new Pieces();

        _pieces = GetNode<Node2D>("Pieces");
        _dots = GetNode<Node2D>("Dots");
        _turn = GetNode<Sprite2D>("Turn");
        _state = StateMachine.None;
        _selectedPiece = new Vector2I(0, 0);

        InitializeBoard();
        display_board();
    }

    public void InitializeBoard()
    {
        _board = new int[8,8]
        {
            { 2, 3, 4, 5, 6, 4, 3, 2 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0 ,0 ,0, 0, 0 ,0, 0, 0 },
            { 0, 0, 0, 0, -1, 0, 0, 0 }
        };
    }

    public void display_board()
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                Sprite2D holder = (Sprite2D)_piecesTexture.TEXTURE_PLACEHOLDER.Instantiate();

                holder.Position = new Vector2(
                    col * CELL_WIDTH + (CELL_WIDTH/2) - (CELL_WIDTH * 5),
                    row * CELL_WIDTH + (CELL_WIDTH/2) - (CELL_WIDTH * 3)
                    );

                int piece = _board[row, col];
                if (piece != 0)
                {
                    //Sprite2D pieceSprite = new Sprite2D();
                    switch (piece)
                    {
                        case 1:
                            holder.Texture = _piecesTexture.ResizeTexture(_piecesTexture.BlackPawn);
                            break;
                        case -1:
                            holder.Texture = _piecesTexture.mainCharacterTexture;
                            break;
                        case 2:
                            holder.Texture = _piecesTexture.BlackRook;
                            break;
                        case 3:
                            holder.Texture = _piecesTexture.BlackKnight;
                            break;
                        case 4:
                            holder.Texture = _piecesTexture.blackBishop;
                            break;
                        case 5:
                            holder.Texture = _piecesTexture.BlackQueen;
                            break;
                        case 6:
                            holder.Texture = _piecesTexture.BlackKing;
                            break;
                    }
                    _pieces.AddChild(holder);
                }
            }
        }
    }

	
	public override void _Process(double delta)
	{
        display_board();
	} 



    public override void _Input(InputEvent @event)
    {
        
    }
}
