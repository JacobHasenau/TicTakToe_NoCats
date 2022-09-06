using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        return _board[yPos, xPos] ?? null;
    }

    public uint GetShapeCount(Shape shape)
    {
        uint count = 0;

        for(uint yPos = 0; yPos < YSize; yPos++)
        {
            for (uint xPos = 0; xPos < XSize; xPos++)
                if (_board[yPos, xPos] == shape)
                    count++;
        }

        return count;
    }

    public ulong GetTotalSquares()
    {
        return XSize * YSize;
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

    public void DoubleBoardSize()
    {
        var newBoard = new Shape?[(YSize * 2) - 1, (XSize * 2) - 1];

        for (uint yPos = 0; yPos < YSize; yPos++)
        {
            for (uint xPos = 0; xPos < XSize; xPos++)
            {
                var shape = GetShapeAtPosition(yPos, xPos);
                if (shape != null)
                    newBoard[yPos * 2, xPos * 2] = shape;
            }
        }

        _board = newBoard;
    }

    public bool OutOfBounds(uint posY, uint posX)
    {
        return posX < 0 || posY < 0 || posX >= XSize || posY >= YSize;
    }
}
