using Godot;
using Godot.NativeInterop;
using System;

public partial class Board : Sprite2D
{
    private const int BOARD_SIZE = 8;
    private const int CELL_WIDTH = 60;

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
    private Node _pieces;
    private Node _dots;
    private Sprite2D _turn;


	public override void _Ready()
    {
        _piecesTexture = new Pieces();

        _pieces = GetNode<Node>("Pieces");
        _dots = GetNode<Node>("Dots");
        _turn = GetNode<Sprite2D>("Turn");
        _state = StateMachine.None;
        _selectedPiece = new Vector2I(0, 0);

        InitializeBoard();
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
            { 0, 0, 0, 0, 0, 0, 0, 0 }
        };
    }

    public void display_board()
    {
        

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                Node2D holder = (Node2D)_piecesTexture.TEXTURE_PLACEHOLDER.Instantiate();

                _pieces.AddChild(holder);

                int piece = _board[row, col];
                if (piece != 0)
                {
                    Sprite2D pieceSprite = new Sprite2D();
                    switch (piece)
                    {
                        case 1:
                            pieceSprite.Texture = _piecesTexture.blackBishop;
                            break;
                        case 2:
                            pieceSprite.Texture = _piecesTexture.BlackRook;
                            break;
                        case 3:
                            pieceSprite.Texture = _piecesTexture.BlackKnight;
                            break;
                        case 4:
                            pieceSprite.Texture = _piecesTexture.BlackPawn;
                            break;
                        case 5:
                            pieceSprite.Texture = _piecesTexture.BlackQueen;
                            break;
                        case 6:
                            pieceSprite.Texture = _piecesTexture.BlackKing;
                            break;
                    }
                    pieceSprite.Position = new Vector2(col * CELL_WIDTH, row * CELL_WIDTH);
                    _pieces.AddChild(pieceSprite);
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
