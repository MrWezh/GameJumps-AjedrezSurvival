using System;
using System.Collections.Generic;
using Godot;
using Godot.Bridge;


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
    // selección de movimientos aleatorios
    private List<Vector2> _randomCandidates = new List<Vector2>();
    private int _pendingMoveSelections = 0;
    private bool _MoveSelectionMode = false;
    // patrones de movimiento elegidos aleatoriamente (1..6 -> peon, caballo, alfil, torre, reina, rey)
    private int _move1Pattern = -1; // patrón para Move 1
    private int _move2Pattern = -1; // patrón para Move 2
    private int _currentRandomPattern = -1; // patrón actualmente en uso

    private int _energia = 6;
    [Export]
    private Node2D _pieces;
    [Export]
    private Node2D _dots;
    [Export]
    private Node2D _areaPreview; // Nodo para mostrar el preview del área de ataque
    private Sprite2D _turn;
    private Label _energyCountLabel;
    public override void _Ready()
    {
        _piecesTexture = new Pieces();
        _selectedPiece = new Vector2I(-1, -1);
        _playerPosition = new Vector2(4, 3);
        _pieces = GetNode<Node2D>("Pieces");
        _dots = GetNode<Node2D>("Dots");
        _areaPreview = GetNode<Node2D>("AreaPreview");
        _piecesMovement = new PiecesMovement();
        _card = new CardUI();
        _energyCountLabel = GetNode<Label>("Energia");

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
                    
                   if(_board[(int)enemyMoves.Y, (int)enemyMoves.X] == -1){
                        gameOver();
                    }

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

    // limpia el preview del área de ataque
    private void ClearAreaPreview()
    {
        if (_areaPreview == null) return;
        var children = _areaPreview.GetChildren();
        foreach (Node child in children)
            child.QueueFree();
    }

    // actualiza el preview del área de ataque según la posición del mouse
    private void UpdateAreaPreview()
    {
        ClearAreaPreview();
        if (_areaPreview == null) return;

        // Obtener la celda sobre la que está el mouse
        Vector2 local = _pieces.ToLocal(GetGlobalMousePosition());
        int mouseCol = (int)(local.X / CELL_WIDTH);
        int mouseRow = (int)(local.Y / CELL_WIDTH);

        // Validar que el mouse esté dentro del tablero
        if (mouseCol < 0 || mouseCol >= BOARD_SIZE || mouseRow < 0 || mouseRow >= BOARD_SIZE)
            return;

        List<Vector2> affectedCells = new List<Vector2>();

        if (_MeleaAttackMode)
        {
            // Preview para ataque melee: 3x1 en la dirección del click
            int px = (int)_playerPosition.X;
            int py = (int)_playerPosition.Y;
            
            int dx = Math.Sign(mouseCol - px);
            int dy = Math.Sign(mouseRow - py);

            // Si está en diagonal, no mostrar preview (melee solo funciona en cardinales)
            if (dx != 0 && dy != 0) return;
            if (dx == 0 && dy == 0) return;

            // Celda principal en la dirección del click
            int targetX = px + dx;
            int targetY = py + dy;
            
            if (targetX >= 0 && targetX < BOARD_SIZE && targetY >= 0 && targetY < BOARD_SIZE)
            {
                affectedCells.Add(new Vector2(targetX, targetY));

                // Celdas perpendiculares
                if (dy != 0) // Si ataca verticalmente, mostrar horizontales
                {
                    if (targetX + 1 < BOARD_SIZE)
                        affectedCells.Add(new Vector2(targetX + 1, targetY));
                    if (targetX - 1 >= 0)
                        affectedCells.Add(new Vector2(targetX - 1, targetY));
                }
                else // Si ataca horizontalmente, mostrar verticales
                {
                    if (targetY + 1 < BOARD_SIZE)
                        affectedCells.Add(new Vector2(targetX, targetY + 1));
                    if (targetY - 1 >= 0)
                        affectedCells.Add(new Vector2(targetX, targetY - 1));
                }
            }
        }
        else if (_RangedAttackMode)
        {
            // Preview para arco: línea recta en una de las 8 direcciones hasta 4 casillas
            int px = (int)_playerPosition.X;
            int py = (int)_playerPosition.Y;
            
            int dx = mouseCol - px;
            int dy = mouseRow - py;
            
            // Normalizar la dirección
            int dirX = Math.Sign(dx);
            int dirY = Math.Sign(dy);

            if (dirX == 0 && dirY == 0) return;

            // Mostrar hasta 4 casillas en esa dirección
            for (int i = 1; i < 5; i++)
            {
                int targetX = px + dirX * i;
                int targetY = py + dirY * i;
                
                if (targetX >= 0 && targetX < BOARD_SIZE && targetY >= 0 && targetY < BOARD_SIZE)
                {
                    affectedCells.Add(new Vector2(targetX, targetY));
                }
                else
                {
                    break;
                }
            }
        }
        else if (_FireBallAttackMode)
        {
            // Preview para fireball: 3x3 centrado en la celda del mouse
            for (int y = mouseRow - 1; y <= mouseRow + 1; y++)
            {
                for (int x = mouseCol - 1; x <= mouseCol + 1; x++)
                {
                    if (x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE)
                    {
                        affectedCells.Add(new Vector2(x, y));
                    }
                }
            }
        }

        // Dibujar las celdas afectadas con un color semitransparente
        foreach (Vector2 cell in affectedCells)
        {
            ColorRect preview = new ColorRect();
            preview.Size = new Vector2(CELL_WIDTH, CELL_WIDTH);
            preview.Position = new Vector2(cell.X * CELL_WIDTH, cell.Y * CELL_WIDTH);
            preview.Color = new Color(1, 0, 0, 0.3f); // Rojo semitransparente
            preview.ZIndex = 5; // Encima de las piezas
            _areaPreview.AddChild(preview);
        }
    }

    public void _on_button_pressed(){
        _piecesMovement.setPlayerPosition(_playerPosition);
        enemies_movement();
        newTurn();
        SpawnEnemyPiece();
        DisplayBoard();
        _turns++;
        _energia = 6;
        _energyCountLabel.Text = "Energia: " + _energia.ToString();
        
        // IMPORTANTE: Resetear ambos patrones de movimiento al pasar turno
        _move1Pattern = -1;
        _move2Pattern = -1;
        _currentRandomPattern = -1;
    }


    public override void _Input(InputEvent @event)
    {
        if(_MeleaAttackMode && _energia != 0 && (_energia -1) >= 0)
        {
            HandleAttackMele(@event);
            // El flag se resetea dentro de HandleAttackMele cuando se completa la acción
        }
        if(_RangedAttackMode && _energia != 0 && (_energia -2) >= 0)
        {
            HandleAttackRange(@event);
            // El flag se resetea dentro de HandleAttackRange cuando se completa la acción
        }
        if(_FireBallAttackMode && _energia != 0 && (_energia -3) >= 0)        
        {
            HandleAttackFireball(@event);
        }
        if (_MoveSelectionMode && _energia != 0 && (_energia -1) >= 0)
        {
            HandleMoveSelection(@event);
            return; // Priorizar selección sobre otras entradas
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
        // Actualizar preview del área de ataque basándose en la posición del mouse
        if (_MeleaAttackMode || _RangedAttackMode || _FireBallAttackMode)
        {
            UpdateAreaPreview();
        }
        else
        {
            ClearAreaPreview();
        }
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
                        ClearAreaPreview();
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
        _energia--;
        _energyCountLabel.Text = "Energia: " + _energia.ToString();
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
    


    private void HandleAttackRangeShow()
    {
        // Mostrar opciones de ataque en las 8 direcciones
        moves = new List<Vector2>();
        int px = (int)_playerPosition.X;
        int py = (int)_playerPosition.Y;
        
        // Las 8 direcciones: cardinales + diagonales
        List<Vector2> directions = new List<Vector2>
        {
            // Cardinales
            new Vector2(0, -1),  // Arriba
            new Vector2(1, 0),   // Derecha
            new Vector2(0, 1),   // Abajo
            new Vector2(-1, 0),  // Izquierda
            // Diagonales
            new Vector2(1, -1),  // Arriba-Derecha
            new Vector2(1, 1),   // Abajo-Derecha
            new Vector2(-1, 1),  // Abajo-Izquierda
            new Vector2(-1, -1)  // Arriba-Izquierda
        };

        foreach (var d in directions)
        {
            for (int i = 1; i < 5; i++)
            {
                int tx = px + (int)d.X * i;
                int ty = py + (int)d.Y * i;
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
        // Maneja el clic para ataque en las 8 direcciones
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
                        // Calcular dirección normalizada hacia el objetivo
                        int px = (int)_playerPosition.X;
                        int py = (int)_playerPosition.Y;
                        int dx = col - px;
                        int dy = row - py;
                        
                        // Normalizar la dirección (-1, 0, o 1)
                        int dirX = Math.Sign(dx);
                        int dirY = Math.Sign(dy);
                        Vector2 dir = new Vector2(dirX, dirY);

                        // reproducir animación de ataque con rotación en el jugador
                        if (IsPlayerValid())
                            _playerInstance.PlayRangedAttack(dir);

                        ExecuteRangeAttack(px, py, dirX, dirY);

                        // limpiar estado de selección y puntos
                        ClearDots();
                        ClearAreaPreview();
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
        _energia -= 2; // Cuesta 2 energía
        _energyCountLabel.Text = "Energia: " + _energia.ToString();
    }

    public void _on_fire_ball_attack_pressed()
    {
        _MeleaAttackMode = false;
        _RangedAttackMode = false;
        _MovimentMode = false;
        // Entrar en modo ataque de bola de fuego: mostrar área de efecto (alcance global)
        if (!IsPlayerValid()) return;

        // Mostrar todos los tiles del tablero como posibles objetivos (excepto la casilla del jugador)
        moves = new List<Vector2>();
        for (int r = 0; r < BOARD_SIZE; r++)
        {
            for (int c = 0; c < BOARD_SIZE; c++)
            {
                if (c == (int)_playerPosition.X && r == (int)_playerPosition.Y) continue;
                moves.Add(new Vector2(c, r));
            }
        }

        if (moves != null && moves.Count > 0)
        {
            _FireBallAttackMode = true;
            show_dots();
        }
    }
     private void HandleAttackFireball(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.IsPressed() && mb.ButtonIndex == MouseButton.Left)
        {
            if (moves != null && moves.Count > 0)
            {
                Vector2 local = _pieces.ToLocal(GetGlobalMousePosition());
                int col = (int)(local.X / CELL_WIDTH);
                int row = (int)(local.Y / CELL_WIDTH);

                if (col >= 0 && col < BOARD_SIZE && row >= 0 && row < BOARD_SIZE)
                {
                    Vector2 target = new Vector2(col, row);
                    if (moves.Contains(target))
                    {
                        // Dirección relativa para orientar animación
                        int px = (int)_playerPosition.X;
                        int py = (int)_playerPosition.Y;
                        int dx = Math.Sign(col - px);
                        int dy = Math.Sign(row - py);

                        if (IsPlayerValid())
                            _playerInstance.PlayFireball(CellToLocalPosition(col, row));

                        ExecuteFireballAttack(col, row);

                        ClearDots();
                        ClearAreaPreview();
                        moves = null;
                        _FireBallAttackMode = false;
                        DisplayBoard();
                    }
                }
            }
        }
    }
    public void ExecuteFireballAttack(int targetCol, int targetRow)
    {
        // Elimina enemigos en un área 3x3 alrededor del objetivo
        for (int y = targetRow - 1; y <= targetRow + 1; y++)
        {
            for (int x = targetCol - 1; x <= targetCol + 1; x++)
            {
                if (!_piecesMovement.isValidPosition(new Vector2(x, y))) continue;
                if (_board[y, x] > 0 && _board[y, x] <= 6)
                {
                    _board[y, x] = 0;
                }
            }
        }
        _piecesMovement.setBoard(_board);
        _energia -= 3; // Cuesta 3 energía
        _energyCountLabel.Text = "Energia: " + _energia.ToString();
    }

    public void _on_move_1_pressed()
    {
        _MeleaAttackMode = false;
        _RangedAttackMode = false;
        _FireBallAttackMode = false;
        _MovimentMode = false;
        // Iniciar selección de 1 movimiento con el patrón de Move 1
        StartMoveSelection(1, true); // true indica que es Move 1
    }
       public void _on_move_2_pressed()
    {
        _MeleaAttackMode = false;
        _RangedAttackMode = false;
        _FireBallAttackMode = false;
        _MovimentMode = false;
        // Iniciar selección de 2 movimientos con el patrón de Move 2
        StartMoveSelection(2, false); // false indica que es Move 2
    }

    private void MoveRandomSteps(int steps)
    {
        if (!IsPlayerValid()) return;
        _piecesMovement.setBoard(_board);
        for (int s = 0; s < steps; s++)
        {
            List<Vector2> possible = _piecesMovement.get_all_possible_player_moves((int)_playerPosition.X, (int)_playerPosition.Y);
            if (possible == null || possible.Count == 0) break;
            Vector2 chosen = _piecesMovement.get_random_move(possible);
            if (chosen == Vector2.Zero) break;
            ExecutePlayerMovement(chosen);
        }
    }

    // Inicia el flujo de selección: genera candidatos aleatorios y muestra puntos
    private void StartMoveSelection(int selections, bool isMove1)
    {
        if (!IsPlayerValid()) return;
        _pendingMoveSelections = selections;
        _MoveSelectionMode = true;
        
        if (isMove1)
        {
            // Move 1: usar o generar patrón de Move 1
            if (_move1Pattern == -1)
            {
                _move1Pattern = random.Next(1, 7);
                
                // Si Move 2 ya tiene un patrón, asegurarse de que Move 1 sea diferente
                if (_move2Pattern != -1)
                {
                    while (_move1Pattern == _move2Pattern)
                    {
                        _move1Pattern = random.Next(1, 7);
                    }
                }
            }
            _currentRandomPattern = _move1Pattern;
        }
        else
        {
            // Move 2: usar o generar patrón de Move 2
            if (_move2Pattern == -1)
            {
                _move2Pattern = random.Next(1, 7);
                
                // Si Move 1 ya tiene un patrón, asegurarse de que Move 2 sea diferente
                if (_move1Pattern != -1)
                {
                    while (_move2Pattern == _move1Pattern)
                    {
                        _move2Pattern = random.Next(1, 7);
                    }
                }
            }
            _currentRandomPattern = _move2Pattern;
        }
        
        ShowMovesForCurrentPattern();
    }

    // Método auxiliar para mostrar los movimientos del patrón actual
    private void ShowMovesForCurrentPattern()
    {
        var all = GetMovesForPattern(_currentRandomPattern);
        moves = new List<Vector2>(all);
        if (moves != null && moves.Count > 0)
            show_dots();
        else
        {
            // no hay movimientos disponibles
            _MoveSelectionMode = false;
            _pendingMoveSelections = 0;
        }
    }

    // Devuelve todas las casillas válidas y vacías según el patrón aleatorio actual
    private List<Vector2> GetMovesForPattern(int pattern)
    {
        _piecesMovement.setBoard(_board);
        int px = (int)_playerPosition.X;
        int py = (int)_playerPosition.Y;
        List<Vector2> possible = null;

        switch (pattern)
        {
            case 1:
                possible = _piecesMovement.get_all_pawn_moves(px, py);
                break;
            case 2:
                possible = _piecesMovement.get_all_knight_moves(px, py);
                break;
            case 3:
                possible = _piecesMovement.get_all_bishop_moves(px, py);
                break;
            case 4:
                possible = _piecesMovement.get_all_rook_moves(px, py);
                break;
            case 5:
                possible = _piecesMovement.get_all_queen_moves(px, py);
                break;
            case 6:
                possible = _piecesMovement.get_all_king_moves(px, py);
                break;
            default:
                possible = _piecesMovement.get_all_possible_player_moves(px, py);
                break;
        }

        if (possible == null) return new List<Vector2>();

        List<Vector2> filtered = new List<Vector2>();
        foreach (var v in possible)
        {
            int x = (int)v.X;
            int y = (int)v.Y;
            if (_piecesMovement.isValidPosition(v) && _board[y, x] == 0)
                filtered.Add(v);
        }
        return filtered;
    }

    // Maneja el clic del jugador para elegir uno de los candidatos generados
    private void HandleMoveSelection(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.IsPressed() && mb.ButtonIndex == MouseButton.Left)
        {
            if (moves == null || moves.Count == 0) return;

            Vector2 local = _pieces.ToLocal(GetGlobalMousePosition());
            int col = (int)(local.X / CELL_WIDTH);
            int row = (int)(local.Y / CELL_WIDTH);

            if (col < 0 || col >= BOARD_SIZE || row < 0 || row >= BOARD_SIZE) return;

            Vector2 target = new Vector2(col, row);
            if (moves.Contains(target))
            {
                // mover jugador a la casilla seleccionada
                ExecutePlayerMovement(target);

                // decrementar selecciones pendientes
                _pendingMoveSelections--;

                ClearDots();
                moves = null;

                if (_pendingMoveSelections > 0)
                {
                    // MANTENER el mismo patrón, solo actualizar las casillas disponibles
                    ShowMovesForCurrentPattern();
                }
                else
                {
                    // acabado: resetear todo
                    _MoveSelectionMode = false;
                    _pendingMoveSelections = 0;
                    _randomCandidates.Clear();
                    // NO resetear el patrón aquí - se resetea en _on_button_pressed al pasar turno
                }

                DisplayBoard();
                _energia--;
                _energyCountLabel.Text = "Energia: " + _energia.ToString();
            }
        }
    }

    public void gameOver(){
        GetTree().ChangeSceneToFile("res://Scenes/GameOver.tscn"); 
    }
}