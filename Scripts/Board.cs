using System;
using System.Collections.Generic;
using Godot;


public partial class Board : Node2D
{
    Random random = new Random();
    private const int BOARD_SIZE = 8;
    private const int CELL_WIDTH = 42;

    private Pieces _piecesTexture;
    private int[,] _board;
    public bool state = false;
    public List<Vector2> moves;
    private Vector2I _selectedPiece = new Vector2I(0, 0);
    private bool _isWhiteTurn = false;
    
    private int _turns = 1;
    private int _maxEnemics = 0;
    private PiecesMovement _piecesMovement;
    private Vector2 _playerPosition; 

    private Enemies _enemies = new Enemies();
    private PackedScene[] _enemyScenes = new PackedScene[7];
    private PackedScene _jugador = GD.Load<PackedScene>("res://Scenes/rey_branco.tscn");
    

    [Export]
    private Node2D _pieces;
    [Export]
    private Node2D _dots;

    private Sprite2D _turn;


    public override void _Ready()
    {
        _piecesTexture = new Pieces();
        _selectedPiece = new Vector2I(-1, -1);
        _playerPosition = new Vector2(4, 3);
        _pieces = GetNode<Node2D>("Pieces");
        _dots = GetNode<Node2D>("Dots");
        _piecesMovement = new PiecesMovement();

        _enemyScenes[1] = _enemies._peon;
        _enemyScenes[2] = _enemies._caballo;
        _enemyScenes[3] = _enemies._alfil;
        _enemyScenes[4] = _enemies._torre;
        _enemyScenes[5] = _enemies._reinaNegra;
        _enemyScenes[6] = _enemies._reynegro;

        InitializeBoard();
        SpawnEnemyPiece();
        DisplayBoard();

    }
    public void InitializeBoard()
    {
        _board = new int[BOARD_SIZE, BOARD_SIZE]
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

        _piecesMovement.setBoard(_board);
    }
      public void RandomPieceSpawn()
    {
        int difficulityLevel = _turns;
        if (_turns > 15)
        {
            difficulityLevel = 12;
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
   /* public void DisplayBoard()
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
    }*/

public void DisplayBoard()
{
    // Liberar hijos previos
    foreach (Node child in _pieces.GetChildren())
        child.QueueFree();

    int size = BOARD_SIZE;
    int cell = CELL_WIDTH;

    for (int row = 0; row < size; row++)
    {
        for (int col = 0; col < size; col++)
        {
            int piece = _board[row, col];
            if (piece == 0) continue;

            // Si la pieza fue marcada como movida (+10), recuperar tipo base
            Vector2 position = new Vector2(col * cell + cell / 2f, row * cell + cell / 2f);
            CharacterBody2D jugador = _jugador.Instantiate<CharacterBody2D>();
            jugador.Position = position;
                //cargar el sprite del jugador
                if(piece == -1)
                    _pieces.AddChild(jugador);

                // cargar los sprites de los enemigos
                if (piece > 0)
                {   
                var inst = _enemyScenes[piece].Instantiate();
                if (inst is CharacterBody2D nd)
                {
                    nd.Position = position;
                    if (nd is CanvasItem ci) ci.ZIndex = 1;

                    _pieces.AddChild(nd);
                    continue;
                }
                }
            
    }
    }
}
        // Asigna la textura correspondiente según el valor entero de la pieza
   private void AssignTexture(Sprite2D holder, int piece)
    {
        switch (piece)
        {
            case -1:
                // jugador (ajusta si tienes textura para el jugador)
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
    // mostrar las opciones de movimiento

    public void enemies_movement()
    {
        // lógica para mover las piezas enemigas automáticamente
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                int piece = _board[row, col];
                if (piece > 0 && piece <= 6)
                {
                    //posición actual del enemigo
                    Vector2 enemyMoves = new Vector2(col, row);
                    // Obtener los movimientos posibles según el tipo de pieza
                    switch (piece)
                    {
                        case 1:
                            if(_turns%piece==0)
                            enemyMoves = _piecesMovement.get_pawn_moves(col, row);
                            break;
                        case 2:
                            if(_turns%piece==0)
                            enemyMoves = _piecesMovement.get_knight_moves(col, row);
                            break;
                        case 3:
                            if(_turns%piece==0)
                            enemyMoves = _piecesMovement.get_bishop_moves(col, row);
                            break;
                        case 4:
                            if(_turns%piece==0)
                            enemyMoves = _piecesMovement.get_rook_moves(col, row);
                            break;
                        case 5:
                            if(_turns%piece==0)
                            enemyMoves = _piecesMovement.get_queen_moves(col, row);
                            break;
                        case 6:
                            if(_turns%piece==0)
                            enemyMoves = _piecesMovement.get_king_moves(col, row);
                            break;
                    }
                    
                   // if(_board[(int)enemyMoves.Y, (int)enemyMoves.X] == -1){
                      ///  gameOver();
                    //}

                    _board[row, col] = 0;
                    _board[(int)enemyMoves.Y, (int)enemyMoves.X] = piece+10;
                    
                    _piecesMovement.setBoard(_board);
                }
            }
        }
        
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

     // limpia los puntos hijos del nodo _dots
    private void ClearDots()
    {
        var children = _dots.GetChildren();
        foreach (Node child in children)
            child.QueueFree();
    }

    public void _on_button_pressed(){
        _piecesMovement.setPlayerPosition(_playerPosition);
        enemies_movement();
        newTurn();
        DisplayBoard();
        _turns++;
        SpawnEnemyPiece();
    }

    public void newTurn()
    {
        int rows = _board.GetLength(0);
        int cols = _board.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (_board[row, col] > 10)
                    _board[row, col] -= 10;
            }
        }
    }

    public override void _Input(InputEvent @event)
    {

      
    }
        public override void _Process(double delta)
    {

    }
    public bool is_mouse_out()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        if (mousePos.X < 0 || mousePos.X >= CELL_WIDTH*BOARD_SIZE || mousePos.Y < 0 || mousePos.Y >= CELL_WIDTH*BOARD_SIZE)
            return true;
        return false;
    }

    public void gameOver(){
        GetTree().ChangeSceneToFile("res://Scenes/GameOver.tscn"); 
    }
}