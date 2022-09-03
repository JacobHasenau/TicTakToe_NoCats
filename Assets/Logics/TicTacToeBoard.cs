using System;
using System.Collections.Generic;
using System.Linq;

public class TicTacToeBoard
{
    private Shape?[,] _board;

    public TicTacToeBoard(uint ySize, uint xSize)
    {
        _board = new Shape?[ySize, xSize];
    }
    
    public uint XSize { get { return (uint)_board.GetLength(1); } }
    public uint YSize { get { return (uint)_board.GetLength(0); } }
    
    public Shape? GetShapeAtPosition(uint yPos, uint xPos)
    {
        if (xPos > XSize || yPos > YSize)
            throw new ArgumentOutOfRangeException();

        return _board[yPos, xPos];
    }

    public uint GetShapeCount(Shape shape)
    {
        uint count = 0;

        for(int yPos = 0; yPos < YSize; yPos++)
        {
            for (int xPos = 0; xPos < XSize; xPos++)
                if (_board[yPos, xPos] == shape)
                    count++;
        }

        return count;
    }

    public bool AcceptPlayerMove(PlayerMove move)
    {
        if (GetShapeAtPosition(move.PosY, move.PosX) != null)
            return false;

        _board[move.PosY, move.PosX] = move.PlayerShape;
        return true;
    }

    public void InitializeBoard(IEnumerable<PlayerMove> moves)
    {
        foreach(var move in moves)
        {
            if (!AcceptPlayerMove(move))
            {
                var existingShape = GetShapeAtPosition(move.PosY, move.PosX);
                throw new InvalidMoveException(move, existingShape);
            }
        }
    }

    public bool PlayerWonOnMove(PlayerMove move, ushort neededInARow)
    {
        if (GetShapeCount(move.PlayerShape) < neededInARow)
            return false;

        ushort inARowFound = 0;
        foreach(var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
        {
            var playerWon = CheckDirectionForVictory(direction, move.PosY, move.PosX, move.PlayerShape, ref inARowFound, neededInARow);

            if (playerWon)
                return playerWon;
        }

        return false;
    }

    private bool CheckDirectionForVictory(Direction direction, uint currY, uint currX, Shape shape, ref ushort inARowCount, ushort neededCount)
    {
        if (OutOfBounds(currY, currX))
            return false;

        if (shape != GetShapeAtPosition(currY, currX))
            return false;

        inARowCount++;
        
        if (inARowCount >= neededCount)
            return true;

        switch(direction)
        {
            case Direction.Up:
                return CheckDirectionForVictory(direction, currY + 1, currX, shape, ref inARowCount, neededCount);
            case Direction.UpRight:
                return CheckDirectionForVictory(direction, currY + 1, currX + 1, shape, ref inARowCount, neededCount);
            case Direction.Right:
                return CheckDirectionForVictory(direction, currY, currX + 1, shape, ref inARowCount, neededCount);
            case Direction.DownRight:
                return CheckDirectionForVictory(direction, currY -1, currX + 1, shape, ref inARowCount, neededCount);
            case Direction.Down:
                return CheckDirectionForVictory(direction, currY -1, currX, shape, ref inARowCount, neededCount);
            case Direction.DownLeft:
                return CheckDirectionForVictory(direction, currY - 1, currX - 1, shape, ref inARowCount, neededCount);
            case Direction.Left:
                return CheckDirectionForVictory(direction, currY, currX - 1, shape, ref inARowCount, neededCount);
            case Direction.UpLeft:
                return CheckDirectionForVictory(direction, currY + 1, currX - 1, shape, ref inARowCount, neededCount);
            default:
                throw new Exception("What. Don't be here.");
        }
    }

    private bool OutOfBounds(uint posY, uint posX)
    {
        return posX < 0 || posY < 0 || posX >= XSize || posY >= YSize;
    }
}
