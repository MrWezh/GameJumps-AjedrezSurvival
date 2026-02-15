using Godot;
using System.Collections.Generic;
using System;



public partial class PiecesMovement : Node2D
{

	public int[,] board;
	public int BOARD_SIZE;
	private Vector2 _playerPosition;
	private Random _random = new Random();

	public PiecesMovement()
	{
		this.BOARD_SIZE = 8;
		this.board = new int[BOARD_SIZE, BOARD_SIZE];
	}	

	public void setBoard(int[,] setboard)
	{
		this.board = setboard;
	}


	public void setPlayerPosition(Vector2 x)
	{
		_playerPosition = x; 
	}

	public double GetSquaredDistance(Vector2 enemiesNextPosition)
	{
		// c^2 = dx^2 + dy^2; usamos la distancia al cuadrado para comparar sin calcular la raíz
		double dx = _playerPosition.X - enemiesNextPosition.X;
		double dy = _playerPosition.Y - enemiesNextPosition.Y;

		double dist2 = dx * dx + dy * dy;

		return dist2;
	}

     public Vector2 get_pawn_moves(int x, int y)
    {
		Vector2 moves = new Vector2(x, y);		  
        double oldDistance = GetSquaredDistance(new Vector2(x, y));
        Vector2 start = new Vector2(x, y); // X = col, Y = row
		
        Vector2[] directions = new Vector2[] 
                { 
                new Vector2(1,1), new Vector2(1,-1), 
		        new Vector2(-1,1), new Vector2(-1,-1),
                new Vector2(0, 1), new Vector2(1, 0), 
                new Vector2(0, -1), new Vector2(-1, 0)
                
                };

        Vector2 dir;
        for (int i = 0; i < directions.Length; i++)
        {
            dir = directions[i];
            Vector2 nextPos = start + dir;
             if (isValidPosition(nextPos)) {
             if (board[(int)nextPos.Y,(int)nextPos.X]==-1 && i<4)
            {
                return nextPos;
            }
                if (i>3&&is_empty(nextPos))
                {
                    double newDistance = GetSquaredDistance(nextPos);
                    if (newDistance < oldDistance)
                    {
                        oldDistance = newDistance;
                        moves = nextPos;
                    }
                }
             }
             
        }

     return moves;
    }


    // movimientos del caballo
      public Vector2 get_knight_moves(int x, int y)
    {
        // movimientos posibles del caballo
        Vector2 _moves = new Vector2(x, y);
        Vector2[] directions = new Vector2[] {
                new Vector2(2, 1), new Vector2(1, 2),
                new Vector2(-1, 2), new Vector2(-2, 1), 
                new Vector2(-2, -1), new Vector2(-1, -2), 
                new Vector2(1, -2), new Vector2(2, -1) };

        Vector2 start = new Vector2(x, y); // X = col, Y = row
		double oldDistance = GetSquaredDistance(start);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            if (isValidPosition(nextPos))
            {
                int cell = board[(int)nextPos.Y, (int)nextPos.X];
                if (cell == 0 || cell == -1)
                {
						double newDistance = GetSquaredDistance(nextPos);
						if(newDistance < oldDistance){
							oldDistance = newDistance;
							_moves = nextPos;
						}
                }
            }
        }
        return _moves;
    }

      public Vector2 get_bishop_moves(int x, int y)
    {
        Vector2 _moves = new Vector2(x, y);
        Vector2[] directions = new Vector2[] 
		{ new Vector2(1,1), new Vector2(1,-1), 
		new Vector2(-1,1), new Vector2(-1,-1) 
		};

        Vector2 start = new Vector2(x, y); // X = col, Y = row
		double oldDistance = GetSquaredDistance(start);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            while (isValidPosition(nextPos))
            {
                int cell = board[(int)nextPos.Y, (int)nextPos.X];
                if (cell != 0 && cell != -1)
                    break; // bloqueado por otra pieza

                double newDistance = GetSquaredDistance(nextPos);
                if (newDistance < oldDistance)
                {
                    oldDistance = newDistance;
                    _moves = nextPos;
                }
                if (cell == -1)
                    return _moves; // capturar jugador

                nextPos += dir;
            }
        }
        return _moves;
    }

    // movimientos de la torre
   public Vector2 get_rook_moves(int x, int y)
    {
        Vector2 _moves = new Vector2(x, y);
        Vector2[] directions = new Vector2[] { 
            new Vector2(0,1), new Vector2(1,0), 
            new Vector2(0,-1), new Vector2(-1,0)};

        Vector2 start = new Vector2(x, y); // X = col, Y = row
		double oldDistance = GetSquaredDistance(start);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            while (isValidPosition(nextPos))
            {
                int cell = board[(int)nextPos.Y, (int)nextPos.X];
                if (cell != 0 && cell != -1)
                    break; // bloqueado por otra pieza

                double newDistance = GetSquaredDistance(nextPos);
                if (newDistance < oldDistance)
                {
                    oldDistance = newDistance;
                    _moves = nextPos;
                }
                if (cell == -1)
                    return _moves; // capturar jugador

                nextPos += dir;
            }
        }
        return _moves;
    }

      public Vector2 get_queen_moves(int x, int y)
    {
        Vector2 _moves = new Vector2(x, y);
        Vector2[] directions = new Vector2[] { 
            new Vector2(1,1), new Vector2(1,-1), 
            new Vector2(-1,1), new Vector2(-1,-1), 
            new Vector2(0,1), new Vector2(1,0), 
            new Vector2(0,-1), new Vector2(-1,0)};

        Vector2 start = new Vector2(x, y); // X = col, Y = row
		double oldDistance = GetSquaredDistance(start);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            while (isValidPosition(nextPos))
            {
               double newDistance = GetSquaredDistance(nextPos);
					if(newDistance < oldDistance && is_empty(nextPos)){
						oldDistance = newDistance;
						_moves = nextPos;
					}	
                nextPos += dir;
            }
        }
        return _moves;
    }

      public Vector2 get_king_moves(int x, int y)
    {
        Vector2 _moves = new Vector2(0, 0);
        Vector2[] directions = new Vector2[] { 
            new Vector2(1,1), new Vector2(1,-1), 
            new Vector2(-1,1), new Vector2(-1,-1), 
            new Vector2(0,1), new Vector2(1,0), 
            new Vector2(0,-1), new Vector2(-1,0)};

        Vector2 start = new Vector2(x, y); // X = col, Y = row
		double oldDistance = GetSquaredDistance(start);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            if (isValidPosition(nextPos))
            {
                double newDistance = GetSquaredDistance(nextPos);
					if(newDistance < oldDistance && is_empty(nextPos)){
						oldDistance = newDistance;
						_moves = nextPos;
					}	
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
        if (board[row, col] == 0)
            return true;
        return false;
    }

    /// <summary>
    /// Obtiene una lista de todos los movimientos posibles para un peón.
    /// </summary>
    public List<Vector2> get_all_pawn_moves(int x, int y)
    {
        List<Vector2> moves = new List<Vector2>();
        Vector2[] directions = new Vector2[] 
        { 
            new Vector2(1,1), new Vector2(1,-1), 
            new Vector2(-1,1), new Vector2(-1,-1),
            new Vector2(0, 1), new Vector2(1, 0), 
            new Vector2(0, -1), new Vector2(-1, 0)
        };

        Vector2 start = new Vector2(x, y);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            if (isValidPosition(nextPos) && is_empty(nextPos))
            {
                moves.Add(nextPos);
            }
        }
        return moves;
    }

    public List<Vector2> get_all_knight_moves(int x, int y)
    {
        List<Vector2> moves = new List<Vector2>();
        Vector2[] directions = new Vector2[] {
            new Vector2(2, 1), new Vector2(1, 2),
            new Vector2(-1, 2), new Vector2(-2, 1), 
            new Vector2(-2, -1), new Vector2(-1, -2), 
            new Vector2(1, -2), new Vector2(2, -1) };

        Vector2 start = new Vector2(x, y);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            if (isValidPosition(nextPos) && is_empty(nextPos))
            {
                moves.Add(nextPos);
            }
        }
        return moves;
    }

    public List<Vector2> get_all_bishop_moves(int x, int y)
    {
        List<Vector2> moves = new List<Vector2>();
        Vector2[] directions = new Vector2[] { new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,1), new Vector2(-1,-1) };
        Vector2 start = new Vector2(x, y);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            while (isValidPosition(nextPos))
            {
                if (is_empty(nextPos)) moves.Add(nextPos);
                else break;
                nextPos += dir;
            }
        }
        return moves;
    }

    public List<Vector2> get_all_rook_moves(int x, int y)
    {
        List<Vector2> moves = new List<Vector2>();
        Vector2[] directions = new Vector2[] { new Vector2(0,1), new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0)};
        Vector2 start = new Vector2(x, y);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            while (isValidPosition(nextPos))
            {
                if (is_empty(nextPos)) moves.Add(nextPos);
                else break;
                nextPos += dir;
            }
        }
        return moves;
    }

    public List<Vector2> get_all_queen_moves(int x, int y)
    {
        List<Vector2> moves = new List<Vector2>();
        Vector2[] directions = new Vector2[] { new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,1), new Vector2(-1,-1), new Vector2(0,1), new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0)};
        Vector2 start = new Vector2(x, y);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            while (isValidPosition(nextPos))
            {
                if (is_empty(nextPos)) moves.Add(nextPos);
                else break;
                nextPos += dir;
            }
        }
        return moves;
    }

    public List<Vector2> get_all_king_moves(int x, int y)
    {
        List<Vector2> moves = new List<Vector2>();
        Vector2[] directions = new Vector2[] { new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,1), new Vector2(-1,-1), new Vector2(0,1), new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0)};
        Vector2 start = new Vector2(x, y);
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
            if (isValidPosition(nextPos) && is_empty(nextPos)) moves.Add(nextPos);
        }
        return moves;
    }

    public List<Vector2> get_all_possible_player_moves(int x, int y)
    {
        List<Vector2> all = new List<Vector2>();
        all.AddRange(get_all_pawn_moves(x,y));
        all.AddRange(get_all_knight_moves(x,y));
        all.AddRange(get_all_bishop_moves(x,y));
        all.AddRange(get_all_rook_moves(x,y));
        all.AddRange(get_all_queen_moves(x,y));
        all.AddRange(get_all_king_moves(x,y));
        var set = new HashSet<Vector2>(all);
        return new List<Vector2>(set);
    }

    public Vector2 get_random_move(List<Vector2> moves)
    {
        if (moves == null || moves.Count == 0) return Vector2.Zero;
        int idx = _random.Next(0, moves.Count);
        return moves[idx];
    }



}
