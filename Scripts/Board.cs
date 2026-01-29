using Godot;
using System;
using System.Collections.Generic;
public partial class Board : Sprite2D
{
    private const int BOARD_SIZE = 8;
    private const int CELL_WIDTH = 42;

    private Pieces _piecesTexture;

    private int[,] _board;

    public bool black = true;
    public bool state = false;
    public List<Vector2> moves;

    enum StateMachine
    {
        Moving,
        Moved,
        None
    }
    private StateMachine _state;
    private Vector2I _selectedPiece;
    private bool _isWhiteTurn = false;
    
    private int _turns = 20;
    private int _maxEnemics = 0;
    private Random random = new Random();


    [Export]
    private Node2D _pieces;
    [Export]
    private Node2D _dots;
    private Sprite2D _turn;


    public override void _Ready()
    {
        _piecesTexture = new Pieces();
        _selectedPiece = new Vector2I(-1, -1);
        _pieces = GetNode<Node2D>("Pieces");
        _dots = GetNode<Node2D>("Dots");
        _state = StateMachine.None;

        InitializeBoard();
        SpawnEnemyPiece();
        DisplayBoard();
    }

    public void InitializeBoard()
    {
        _board = new int[8, 8]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, -1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0 ,0 ,0, 0, 0 ,0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 }
        };
    }
      public void RandomPieceSpawn()
    {
        int difficulityLevel = _turns;
        if (_turns > 15)
        {
            difficulityLevel = 15;
        }
        // Aqui captura la dificultad para limitar el tipo de piezas
        int pieceType = random.Next(1, 2 + difficulityLevel/3); // Genera un número aleatorio entre 1 y 5
        // Aquí puedes usar pieceType para determinar qué tipo de pieza crear de peon a reina
        bool isInRow = random.Next(0, 2) == 0;
        // Aquí puedes usar isRow para determinar si la pieza se modificara para colocarse en una fila o columna
        int row, col;
        if (isInRow)
        {
            row = random.Next(1, BOARD_SIZE);
            col = random.Next(0, 2);
            if (col != 0)
            {
                col = BOARD_SIZE - 1;
            }
            if (_board[row, col] == 0)
            {
                _board[row, col] = pieceType;
                }
            else
            {
                RandomPieceSpawn();
            }
        }
        else
        {
            col = random.Next(1, BOARD_SIZE);
            row = random.Next(0, 2);
            if (row != 0)
            {
                row = BOARD_SIZE - 1;
            }
            if (_board[row, col] == 0)
            {
                _board[row, col] = pieceType;
            }
            else
            {
                RandomPieceSpawn();
            }
        }
    }

     public void SpawnEnemyPiece()
    {
        if (LeerBordes(_board) < 1)
        {
            return;
        }
        else if (LeerBordes(_board) < 5 + (_turns/5))
        {
            for (int i = 0; i < LeerBordes(_board); i++)
            {
                RandomPieceSpawn();
            }
            return;
        }
        else
        {
            for (int i = 0; i < 5 + (_turns/5); i++)
            {
                RandomPieceSpawn();
            }
        }
    }
    static int LeerBordes(int[,] matriz)
    {
        int filas = matriz.GetLength(0);
        int cols = matriz.GetLength(1);
        int casillasDisponibles = 0;

        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Es borde si está en primera/última fila o primera/última columna
                if (i == 0 || i == filas - 1 || j == 0 || j == cols - 1)
                {
                    if (matriz[i, j] == 0)
                    {
                        casillasDisponibles++;
                    }
                }
            }
        }

        return casillasDisponibles;
    }

    //Funcion para mostrar las piezas en el tablero
    public void DisplayBoard()
    {
        // Liberar hijos previos (si se vuelve a dibujar)
        foreach (Node child in _pieces.GetChildren())
        {
            // QueueFree para liberar correctamente los nodos instanciados
            child.QueueFree();
        }

        // Local copies for faster access
        int size = BOARD_SIZE;
        int cell = CELL_WIDTH;
        var textures = _piecesTexture;

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                int piece = _board[row, col];
                if (piece == 0)
                    continue; // evitar instanciar para celdas vacías

                // Calcular posición del centro de la celda (float)
                Vector2 position = new Vector2(col * cell + cell / 2f, row * cell + cell / 2f);

                // Instanciar y preparar el sprite solo cuando hay pieza
                Sprite2D holder = (Sprite2D)textures.TEXTURE_PLACEHOLDER.Instantiate();
                holder.ZAsRelative = false;
                holder.ZIndex = 1;
                holder.Position = position;

                AssignTexture(holder, piece);

                _pieces.AddChild(holder);
            }
        }
    }

        // Asigna la textura correspondiente según el valor entero de la pieza
    private void AssignTexture(Sprite2D holder, int piece)
    {
        switch (piece)
        {
            case -1:
                holder.Texture = _piecesTexture.mainCharacterTexture;
                break;
            case 1:
                holder.Texture = _piecesTexture.BlackPawn;
                break;
            case 2:
                holder.Texture = _piecesTexture.BlackKnight;
                break;
            case 3:
                holder.Texture = _piecesTexture.BlackBishop;
                break;
            case 4:
                holder.Texture = _piecesTexture.BlackRook;
                break;
            case 5:
                holder.Texture = _piecesTexture.BlackQueen;
                break;
            case 6:
                holder.Texture = _piecesTexture.BlackKing;
                break;
            default:
                holder.Texture = null;
                break;
        }
    }


    public override void _Process(double delta)
    {

    }
    // mostrar las opciones de movimiento
    public void show_options()
    {
        moves = get_moves();

        if (moves.Count == 0)
        {
            state = false;
            return;
        }

        show_dots();
    }
      // mostrar los puntos de movimiento
    public void show_dots()
    {
        foreach (Vector2 i in moves)
        {
            // inicializa la textura del punto de movimiento
            Sprite2D holder = (Sprite2D)_piecesTexture.TEXTURE_PLACEHOLDER.Instantiate();
            holder.Texture = _piecesTexture.PIECES_MOVES;
            holder.Position = new Vector2(
                i.X * CELL_WIDTH + (CELL_WIDTH / 2), // X = columna
                i.Y * CELL_WIDTH + (CELL_WIDTH / 2) // Y = fila
                );

                _dots.AddChild(holder);
        }
    }

    // obtener los movimientos de la pieza seleccionada
    public List<Vector2> get_moves()
    {

        List<Vector2> _moves = new List<Vector2>();
        // identificar la pieza seleccionada
        int row = Math.Abs((int)_selectedPiece.Y);
        int col = Math.Abs((int)_selectedPiece.X);
        switch (_board[row, col])
        {
            case 1:
                GD.Print("Pawn selected");
                break;
            case 2:
                GD.Print("Knight selected");
                break;
            case 3:
                _moves = get_bishop_moves();
                break;
            case 4:
                _moves = get_rook_moves();
                break;
            case 5:
                GD.Print("Queen selected");
                break;
            case 6:
                GD.Print("King selected");
                break;
        }
        return _moves;
    }

  
    // movimientos de la torre
   public List<Vector2> get_rook_moves()
    {
        List<Vector2> _moves = new List<Vector2>();
        Vector2[] directions = new Vector2[] { new Vector2(0,1), new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0) };

        Vector2 start = new Vector2(_selectedPiece.X, _selectedPiece.Y); // X = col, Y = row

        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            while (isValidPosition(nextPos))
            {
                if (is_empty(nextPos))
                {
                    _moves.Add(new Vector2(nextPos.X, nextPos.Y));
                }
                else
                {
                    int col = (int)nextPos.X;
                    int row = (int)nextPos.Y;
                    if (black && _board[row, col] > 0)
                    {
                        _moves.Add(new Vector2(nextPos.X, nextPos.Y));
                    }
                    break;
                }
                nextPos += dir;
            }
        }
        return _moves;
    }

  public List<Vector2> get_bishop_moves()
    {
        List<Vector2> _moves = new List<Vector2>();
        Vector2[] directions = new Vector2[] { new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,1), new Vector2(-1,-1) };

        Vector2 start = new Vector2(_selectedPiece.X, _selectedPiece.Y); // X = col, Y = row

        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            while (isValidPosition(nextPos))
            {
                if (is_empty(nextPos))
                {
                    _moves.Add(new Vector2(nextPos.X, nextPos.Y));
                }
                else
                {
                    int col = (int)nextPos.X;
                    int row = (int)nextPos.Y;
                    if (black && _board[row, col] > 0)
                    {
                        _moves.Add(new Vector2(nextPos.X, nextPos.Y));
                    }
                    break;
                }
                nextPos += dir;
            }
        }
        return _moves;
    }




    public bool isValidPosition(Vector2 pos)
    {
        if (pos.X < 0 || pos.X >= BOARD_SIZE || pos.Y < 0 || pos.Y >= BOARD_SIZE)
            return false;
        return true;
    }
    public bool is_empty(Vector2 pos)
    {
        int col = (int)pos.X;
        int row = (int)pos.Y;
        if (_board[row, col] == 0)
            return true;
        return false;
    }
     // limpia los puntos hijos del nodo _dots
    private void ClearDots()
    {
        var children = _dots.GetChildren();
        foreach (Node child in children)
            child.QueueFree();
    }

    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed())
        {
            // convertir la posición global a local del nodo (Sprite2D)
                Vector2 local = ToLocal(GetGlobalMousePosition());
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                 // fuera del tablero?
                if (local.X < 0 || local.Y < 0 || local.X >= CELL_WIDTH * BOARD_SIZE || local.Y >= CELL_WIDTH * BOARD_SIZE)
                    return;

                int col = (int)local.X / CELL_WIDTH;
                int row = (int)local.Y / CELL_WIDTH;

                GD.Print(row, ",", col);

                if (!state && (black && _board[row, col] > 0))
                {
                    _selectedPiece = new Vector2I(col, row);
                    show_options();
                    state = true;
                }
                else if (state)
                {
                    Vector2 selectedMove = new Vector2(col, row);
                    if (moves.Contains(selectedMove))
                    {
                        // mover la pieza
                        int fromRow = (int)_selectedPiece.Y;
                        int fromCol = (int)_selectedPiece.X;
                        int toRow = (int)selectedMove.Y;
                        int toCol = (int)selectedMove.X;

                        _board[toRow, toCol] = _board[fromRow, fromCol];
                        _board[fromRow, fromCol] = 0;

                        // limpiar los puntos de movimiento
                        ClearDots();
                        state = false;
                        black = !black;
                    }
                    else
                    {
                        // limpiar los puntos de movimiento
                        ClearDots();
                        state = false;
                    }
                }
            }
        }
    }
    public bool is_mouse_out()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        if (mousePos.X < 0 || mousePos.X >= CELL_WIDTH*BOARD_SIZE || mousePos.Y < 0 || mousePos.Y >= CELL_WIDTH*BOARD_SIZE)
            return true;
        return false;
    }
}

