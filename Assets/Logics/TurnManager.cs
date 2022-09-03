using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager
{
    private TicTacToeBoard _board;
    private ushort _shapesInRowToWin;
    private IReadOnlyCollection<Shape> _playerShapes;

    public TurnManager(TicTacToeBoard board, ushort shapesInRowToWin, IReadOnlyCollection<Shape> playerShapes)
    {
        _board = board;
        _shapesInRowToWin = shapesInRowToWin;
        _playerShapes = playerShapes;
    }

    public void PlayerMakesTurn(PlayerMove move)
    {
        if (!_board.AcceptPlayerMove(move))
            throw new Exception("Replace me with a delegate at somepoint.");

        if (PlayerWonOnMove(move))
        {
            Debug.Log($"Player {move.PlayerShape} has won! Throwing an exception until you make win/loss screen.");
            throw new EntryPointNotFoundException("Im that cool exception for win/loss");
        }

        if (!MovesLeftOnBoard())
            _board.DoubleBoardSize();
    }

    public bool PlayerWonOnMove(PlayerMove move)
    {
        if (_board.GetShapeCount(move.PlayerShape) < _shapesInRowToWin)
            return false;

        ushort inARowFound = 0;
        foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>())
        {
            var playerWon = CheckDirectionForVictory(direction, move.PosY, move.PosX, move.PlayerShape, ref inARowFound);

            if (playerWon)
                return playerWon;
        }

        return false;
    }

    public bool MovesLeftOnBoard()
    {
        var totalSpaces = _board.GetTotalSquares();
        var usedSpaces = (ulong)_playerShapes.Sum(shape => _board.GetShapeCount(shape));

        return totalSpaces > usedSpaces;
    }

    private bool CheckDirectionForVictory(Direction direction, uint currY, uint currX, Shape shape, ref ushort inARowCount)
    {
        if (_board.OutOfBounds(currY, currX))
            return false;

        if (shape != _board.GetShapeAtPosition(currY, currX))
            return false;

        inARowCount++;

        if (inARowCount >= _shapesInRowToWin)
            return true;

        switch (direction)
        {
            case Direction.Up:
                return CheckDirectionForVictory(direction, currY + 1, currX, shape, ref inARowCount);
            case Direction.UpRight:
                return CheckDirectionForVictory(direction, currY + 1, currX + 1, shape, ref inARowCount);
            case Direction.Right:
                return CheckDirectionForVictory(direction, currY, currX + 1, shape, ref inARowCount);
            case Direction.DownRight:
                return CheckDirectionForVictory(direction, currY - 1, currX + 1, shape, ref inARowCount);
            case Direction.Down:
                return CheckDirectionForVictory(direction, currY - 1, currX, shape, ref inARowCount);
            case Direction.DownLeft:
                return CheckDirectionForVictory(direction, currY - 1, currX - 1, shape, ref inARowCount);
            case Direction.Left:
                return CheckDirectionForVictory(direction, currY, currX - 1, shape, ref inARowCount);
            case Direction.UpLeft:
                return CheckDirectionForVictory(direction, currY + 1, currX - 1, shape, ref inARowCount);
            default:
                throw new Exception("What. Don't be here.");
        }
    }
}