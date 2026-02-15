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
    
    private ReyBranco _playerInstance;
    private CardUI _card;
    private bool _MeleaAttackMode = false;
    private bool _RangedAttackMode = false;
    private bool _FireBallAttackMode = false;
    private bool _MovimentMode = false;

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
        _card = new CardUI();

        _enemyScenes[1] = _enemies._peon;
        _enemyScenes[2] = _enemies._caballo;
        _enemyScenes[3] = _enemies._alfil;
        _enemyScenes[4] = _enemies._torre;
        _enemyScenes[5] = _enemies._reinaNegra;
        _enemyScenes[6] = _enemies._reynegro;

        InitializeBoard();
        SpawnEnemyPiece();
        DisplayBoard();
        _playerInstance.setAnimation("idle");

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


    private void HandleAttackMele(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.IsPressed() && mb.ButtonIndex == MouseButton.Left)
        {
            // si estamos en modo ataque, capturar click en una de las casillas resaltadas
            if (moves != null && moves.Count > 0)
            {
                Vector2 local = _pieces.ToLocal(GetGlobalMousePosition());
                int col = (int)(local.X / CELL_WIDTH);
                int row = (int)(local.Y / CELL_WIDTH);

                // validar rango
                if (col >= 0 && col < BOARD_SIZE && row >= 0 && row < BOARD_SIZE)
                {
                    Vector2 target = new Vector2(col, row);
                    if (moves.Contains(target))
                    {
                        // dirección relativa normalizada -1/0/1
                        int px = (int)_playerPosition.X;
                        int py = (int)_playerPosition.Y;
                        int dx = Math.Sign(col - px);
                        int dy = Math.Sign(row - py);
                        Vector2 dir = new Vector2(dx, dy);

                        // reproducir animación de ataque en el jugador
                        if (IsPlayerValid())
                            _playerInstance.PlayMeleAttack(dir);
                        ExecuteMeleAttack(px, py, dx, dy);

                        // limpiar estado de selección y puntos
                        ClearDots();
                        moves = null;
                        _MeleaAttackMode = false;
                        // refrescar vista
                        DisplayBoard();
                    }
                }
            }
        }
    }

    private void ExecuteMeleAttack(int startX, int startY, int dirX, int dirY)
    {
        
        int targetX = startX + dirX;
        int targetY = startY + dirY;

        
        if(!_piecesMovement.isValidPosition(new Vector2(targetX, targetY)))
         return;
        _board[targetY, targetX] = 0; // eliminar pieza enemiga (si existe)
         
        if(dirY != 0)
        {
            if(targetX + 1 < BOARD_SIZE)
            _board[targetY, targetX+1] = 0;
            if(targetX - 1 >= 0)
            _board[targetY, targetX-1] = 0;
        }
        else
        {
            if(targetY + 1 < BOARD_SIZE)
            _board[targetY+1, targetX] = 0;
            if(targetY - 1 >= 0)
            _board[targetY-1, targetX] = 0;
        }
        _piecesMovement.setBoard(_board);
    }

    private void HandleAttackRangeShow()
    {
        // Mostrar opciones de ataque en diagonales
        moves = new List<Vector2>();
        int px = (int)_playerPosition.X;
        int py = (int)_playerPosition.Y;
        
        // Solo diagonales
        List<Vector2> directions = new List<Vector2>
        {
            new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1)
        };

        foreach (var d in directions)
        {
            for (int i = 1; i < 5; i++)
            {
                int tx = px + (int)d.X;
                int ty = py + (int)d.Y;
                if (tx >= 0 && tx < BOARD_SIZE && ty >= 0 && ty < BOARD_SIZE)
                {
                    moves.Add(new Vector2(tx, ty));
                }
                else
                {
                    break;
                }
            }
        }

        if (moves.Count > 0)
        {
            show_dots();
        }
    }

    private void HandleAttackRange(InputEvent @event)
    {
        // Maneja el clic para ataque en diagonales
        if (@event is InputEventMouseButton mb && mb.IsPressed() && mb.ButtonIndex == MouseButton.Left)
        {
            if (moves != null && moves.Count > 0)
            {
                Vector2 local = _pieces.ToLocal(GetGlobalMousePosition());
                int col = (int)(local.X / CELL_WIDTH);
                int row = (int)(local.Y / CELL_WIDTH);

                // validar rango
                if (col >= 0 && col < BOARD_SIZE && row >= 0 && row < BOARD_SIZE)
                {
                    Vector2 target = new Vector2(col, row);
                    if (moves.Contains(target))
                    {
                        // dirección relativa normalizada -1/0/1
                        int px = (int)_playerPosition.X;
                        int py = (int)_playerPosition.Y;
                        int dx = Math.Sign(col - px);
                        int dy = Math.Sign(row - py);
                        Vector2 dir = new Vector2(dx, dy);

                        // reproducir animación de ataque con rotación en el jugador
                        if (IsPlayerValid())
                            _playerInstance.PlayRangedAttack(dir);

                        ExecuteRangeAttack(px, py, dx, dy);

                        // limpiar estado de selección y puntos
                        ClearDots();
                        moves = null;
                        _RangedAttackMode = false;
                        // refrescar vista
                        DisplayBoard();
                    }
                }
            }
        }
    }

    public void ExecuteRangeAttack(int startX, int startY, int dirX, int dirY)
    {
        // Lógica para eliminar piezas en la dirección dada (puede ser más compleja según alcance, obstáculos, etc.)
        for (int i = 1; i < 5; i++)
        {
            int targetX = startX + dirX * i;
            int targetY = startY + dirY * i;

            if (!_piecesMovement.isValidPosition(new Vector2(targetX, targetY)))
                break;

            if (_board[targetY, targetX] > 0 && _board[targetY, targetX] <= 6)
            {
                _board[targetY, targetX] = 0; // eliminar piezas enemigas
            }
        }
        _piecesMovement.setBoard(_board);
    }

    private void HandleAttackFireball()
    {
        
    }

    private void HandleMovement1()
    {
       
    }

    private void HandleMovement2()
    {
    
    }

    private void ExecutePlayerMovement(Vector2 newPosition)
    {
        // Mover al jugador en el tablero
        _board[(int)_playerPosition.Y, (int)_playerPosition.X] = 0;
        _board[(int)newPosition.Y, (int)newPosition.X] = -1;
        _playerPosition = newPosition;
        
        // Actualizar la instancia visual del jugador
        if (IsPlayerValid())
        {
            Vector2 localPos = CellToLocalPosition((int)newPosition.X, (int)newPosition.Y);
            _playerInstance.MoveToLocalPosition(localPos);
        }
        
        // Actualizar el tablero en PiecesMovement
        _piecesMovement.setBoard(_board);
        
        // Refrescar la vista
        DisplayBoard();
    }

  public void DisplayBoard()
    {
        // Liberar hijos previos, pero NO eliminar la instancia del jugador si ya está añadida
        foreach (Node child in _pieces.GetChildren())
        {
            // evita eliminar al jugador (si existe y es hijo)
            if (_playerInstance != null && child == _playerInstance)
                continue;
            child.QueueFree();
        }

        int size = BOARD_SIZE;
        int cell = CELL_WIDTH;

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                int piece = _board[row, col];
                if (piece == 0) continue;

                int basePiece = piece > 10 ? piece - 10 : piece;
                Vector2 localPos = CellToLocalPosition(col, row);

                // jugador: instanciar solo una vez y moverla las siguientes veces
                if (basePiece == -1)
                {
                    if (!IsPlayerValid())
                    {
                        if (_jugador != null)
                        {
                            var jugadorInst = _jugador.Instantiate();
                            if (jugadorInst is ReyBranco rb)
                            {
                                _playerInstance = rb;
                                _playerInstance.Position = localPos;
                                _pieces.AddChild(_playerInstance);
                                _card.setPlayer(_playerInstance); // pasar referencia del jugador a la carta
                            }
                            else if (jugadorInst is Node2D nd)
                            {
                                nd.Position = localPos;
                                _pieces.AddChild(nd);
                            }
                        }
                    }
                    else
                    {
                        // mover solo si sigue siendo válido
                        _playerInstance.MoveToLocalPosition(localPos);
                    }

                    _playerPosition = new Vector2(col, row);
                    continue;
                }

                // enemigos (tipos 1..6) -> seguimos instanciando como antes
                if (basePiece >= 1 && basePiece <= 6)
                {
                    if (basePiece < _enemyScenes.Length && _enemyScenes[basePiece] != null)
                    {
                        var inst = _enemyScenes[basePiece].Instantiate();
                        if (inst is Node2D nd)
                        {
                            AddEnemyInstanceToBoard(nd, col, row);
                            continue;
                        }
                    }
                }

                 // para cualquier otro valor (por seguridad), se puede instanciar un sprite genérico
                 Sprite2D holder = (Sprite2D)_piecesTexture.TEXTURE_PLACEHOLDER.Instantiate();
                 holder.ZAsRelative = false;
                 holder.ZIndex = 1;
                 holder.Position = localPos;
                 AssignTexture(holder, basePiece);
                 _pieces.AddChild(holder);
            }
        }
        
    }

      private bool IsPlayerValid()
    {
        return _playerInstance != null && IsInstanceValid(_playerInstance) && _playerInstance.IsInsideTree();
    }

// convierte celda (col,row) -> posición local centrada dentro del nodo _pieces
private Vector2 CellToLocalPosition(int col, int row)
{
    return new Vector2(col * CELL_WIDTH + CELL_WIDTH / 2f, row * CELL_WIDTH + CELL_WIDTH / 2f);
}

// añade la instancia al contenedor _pieces y deja su posición en coordenadas locales
private void AddEnemyInstanceToBoard(Node2D inst, int col, int row)
{
    inst.Position = CellToLocalPosition(col, row);
    if (inst is CanvasItem ci) ci.ZIndex = 1;
    _pieces.AddChild(inst);
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
        ClearDots();
        if (moves == null) return;

        foreach (Vector2 i in moves)
        {
            // inicializa la textura del punto de movimiento
            Sprite2D holder = (Sprite2D)_piecesTexture.TEXTURE_PLACEHOLDER.Instantiate();
            holder.Texture = _piecesTexture.PIECES_MOVES;
            // posición local dentro del nodo _dots (asumiendo que _dots está alineado con el tablero)
            holder.Position = new Vector2(
                i.X * CELL_WIDTH + (CELL_WIDTH / 2f), // X = columna
                i.Y * CELL_WIDTH + (CELL_WIDTH / 2f)  // Y = fila
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


    public override void _Input(InputEvent @event)
    {
        if(_MeleaAttackMode)
        {
            HandleAttackMele(@event);
            // El flag se resetea dentro de HandleAttackMele cuando se completa la acción
        }
        if(_RangedAttackMode)
        {
            HandleAttackRange(@event);
            // El flag se resetea dentro de HandleAttackRange cuando se completa la acción
        }
        if(_FireBallAttackMode)        
        {
            HandleAttackFireball();
            _FireBallAttackMode = false;
        }
        if(_MovimentMode)
        {
            HandleMovement1();
            _MovimentMode = false;
        }
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

        public override void _Process(double delta)
    {

    }
    public bool is_mouse_out()
    {
        // convertir la posición global del ratón a la coordenada local del contenedor de piezas
        Vector2 local = _pieces.ToLocal(GetGlobalMousePosition());
        if (local.X < 0 || local.X >= CELL_WIDTH * BOARD_SIZE || local.Y < 0 || local.Y >= CELL_WIDTH * BOARD_SIZE)
            return true;
        return false;
    }

    public void _on_mele_attack_pressed()
    {
        _RangedAttackMode = false;
        _FireBallAttackMode = false;
        _MovimentMode = false;
           // Entrar en modo ataque: calcular casillas adyacentes y mostrar puntos
        if (!IsPlayerValid()) return;

        List<Vector2> directions;
            directions = new List<Vector2> {
                new Vector2(0,-1), new Vector2(1,0),
                new Vector2(0,1), new Vector2(-1,0)
            };

        // construir lista de objetivos válidos (coordenadas de celda)
        moves = new List<Vector2>();
        int px = (int)_playerPosition.X;
        int py = (int)_playerPosition.Y;

        foreach (var d in directions)
        {
            int tx = px + (int)d.X;
            int ty = py + (int)d.Y;
            if (tx >= 0 && tx < BOARD_SIZE && ty >= 0 && ty < BOARD_SIZE)
            {
                moves.Add(new Vector2(tx, ty));
            }
        }

        if (moves.Count > 0)
        {
            _MeleaAttackMode = true;
            show_dots(); // usa moves para dibujar puntos con PIECES_MOVES
        }

        
    }

    public void _on_ranged_attack_pressed()
    {
        _MeleaAttackMode = false;
        _FireBallAttackMode = false;
        _MovimentMode = false;
        // Entrar en modo ataque a rango: mostrar diagonales
        if (!IsPlayerValid()) return;

        HandleAttackRangeShow();

        if (moves != null && moves.Count > 0)
        {
            _RangedAttackMode = true;
        }
    }

    public void _on_fire_ball_attack_pressed()
    {
        _MeleaAttackMode = false;
        _RangedAttackMode = false;
        _MovimentMode = false;
        // Entrar en modo ataque de bola de fuego: mostrar área de efecto (ejemplo: 3x3 alrededor del jugador)
        if (!IsPlayerValid()) return;

        // Aquí podrías calcular las casillas dentro de un área de efecto y mostrarlas con puntos
        // Luego, al hacer clic, ejecutar la lógica para eliminar piezas dentro de esa área
        

        _FireBallAttackMode = true;
    }



    public void gameOver(){
        GetTree().ChangeSceneToFile("res://Scenes/GameOver.tscn"); 
    }
}