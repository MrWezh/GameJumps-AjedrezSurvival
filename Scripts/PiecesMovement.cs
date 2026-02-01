using Godot;
using System.Collections.Generic;
using System;



public partial class PiecesMovement : Node2D
{

	public int[,] board;
	public int BOARD_SIZE;
	private Vector2 _playerPosition;

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
		// c^2 = a^2 + b*2, para carcular la posición má proximo al jugador
		// donde a = (_playerPosition.X - enemiesPosition.X) y a = (_playerPosition.Y - enemiesPosition.Y)
		// comparamos la c, que será la distandia entre el enemigo y el player, de todas las posiciones posibles

		double a = _playerPosition.X - enemiesNextPosition.X;
		double b = _playerPosition.Y - enemiesNextPosition.Y;

		double distans = a*a + b*b;

		return distans;
	}

     public Vector2 get_pawn_moves(int x, int y)
    {

				  Vector2 _moves = new Vector2(0, 0);
				  
        Vector2 start = new Vector2(x, y); // X = col, Y = row
		List<Vector2> enemyPositions = new List<Vector2>
            {
                new Vector2(start.X + 1, start.Y + 1), // Diagonal derecha adelante
                new Vector2(start.X - 1, start.Y + 1), // Diagonal izquierda adelante
                new Vector2(start.X + 1, start.Y - 1), // Diagonal derecha atrás
                new Vector2(start.X - 1, start.Y - 1) // Diagonal izquierda atrás
            };

                    
			 foreach (Vector2 enemyPos in enemyPositions)
			{
			if(isValidPosition(enemyPos)){
				if(board[(int)enemyPos.Y, (int)enemyPos.X] == -1 ){
					return _moves; 
				}
			}
						
		}
		
        Vector2[] directions = new Vector2[] { 
                new Vector2(0, 1), new Vector2(1, 0), 
                new Vector2(0, -1), new Vector2(-1, 0) };

		double oldDistance = GetSquaredDistance(new Vector2(x, y));
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = start + dir;
			double newDistance = GetSquaredDistance(nextPos);
            if (isValidPosition(nextPos) && is_empty(nextPos))
            {
                if (newDistance < oldDistance)
                {
					oldDistance = newDistance;
                    _moves = nextPos;
                }
            }

        }
        return _moves;
    }
    // movimientos del caballo
      public Vector2 get_knight_moves(int x, int y)
    {
        // movimientos posibles del caballo
        Vector2 _moves = new Vector2(0, 0);
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
					double newDistance = GetSquaredDistance(nextPos);
					if(newDistance < oldDistance){
						oldDistance = newDistance;
						_moves = nextPos;
					}	
            }
        }
        return _moves;
    }

      public Vector2 get_bishop_moves(int x, int y)
    {
        Vector2 _moves = new Vector2(0, 0);
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
                double newDistance = GetSquaredDistance(nextPos);
					if(newDistance < oldDistance){
						oldDistance = newDistance;
						_moves = nextPos;
					}	
                nextPos += dir;
            }
        }
        return _moves;
    }

    // movimientos de la torre
   public Vector2 get_rook_moves(int x, int y)
    {
        Vector2 _moves = new Vector2(0, 0);
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
                double newDistance = GetSquaredDistance(nextPos);
					if(newDistance < oldDistance){
						oldDistance = newDistance;
						_moves = nextPos;
					}	
                nextPos += dir;
            }
        }
        return _moves;
    }

      public Vector2 get_queen_moves(int x, int y)
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
            while (isValidPosition(nextPos))
            {
               double newDistance = GetSquaredDistance(nextPos);
					if(newDistance < oldDistance){
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
					if(newDistance < oldDistance){
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
}
