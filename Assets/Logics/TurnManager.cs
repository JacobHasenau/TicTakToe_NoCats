using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void PlayerTurnUpdate(object sender, Shape newPlayersTurn);
public delegate void PlayerHasWon(object sender, WinningInformation winningPlayer);

public class TurnManager
{
    private TicTacToeBoard _board;
    private ushort _shapesInRowToWin;
    private IList<Shape> _playerShapes;
    private short _currentTurn;
    private PlayerTurnUpdate _onPlayerTurnUpdated;
    private PlayerHasWon _onPlayerWin;


    private Dictionary<Direction, Direction> _directionPairs
        = new Dictionary<Direction, Direction>{ { Direction.Up, Direction.Down },
                                                { Direction.UpRight, Direction.DownLeft },
                                                { Direction.Right, Direction.Left },
                                                { Direction.DownRight, Direction.UpLeft }
        };

    public TurnManager(TicTacToeBoard board, ushort shapesInRowToWin, IReadOnlyCollection<Shape> playerShapes)
    {
        _board = board;
        _shapesInRowToWin = shapesInRowToWin;
        SetActivePlayers(playerShapes);
        _currentTurn = 0;
    }

    public void ResetManager(TicTacToeBoard board, ushort shapesInRowToWin, IReadOnlyCollection<Shape> playerShapes)
    {
        _board = board;
        _shapesInRowToWin = shapesInRowToWin;
        SetActivePlayers(playerShapes);
        _currentTurn = 0;
    }

    public void SetActivePlayers(IReadOnlyCollection<Shape> playerShapes)
    {
        _playerShapes = playerShapes.ToList();
    }

    public void PlayerMakesTurn(PlayerMove move)
    {
        if (!_board.AcceptPlayerMove(move))
            throw new Exception("Replace me with a delegate at somepoint.");

        if (PlayerWonOnMove(move))
        {
            Debug.Log($"Player {move.PlayerShape} has won! Throwing an exception until you make win/loss screen.");
            _onPlayerWin(this, new WinningInformation(_playerShapes.ToList(), move.PlayerShape, _currentTurn));
            return;
        }

        if (!MovesLeftOnBoard())
        {
            _shapesInRowToWin++;
            _board.DoubleBoardSize();
        }

        _currentTurn++;
        if (_onPlayerTurnUpdated != null)
            _onPlayerTurnUpdated(this, GetCurrentPlayersTurn());
    }

    public bool PlayerWonOnMove(PlayerMove move)
    {
        if (_board.GetShapeCount(move.PlayerShape) < _shapesInRowToWin)
            return false;

        foreach (var directions in _directionPairs)
        {
            ushort inARowFound = 0;

            var playerWon = CheckDirectionPairForVictory(directions, move.PosY, move.PosX, move.PlayerShape, ref inARowFound);

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

    public Shape GetCurrentPlayersTurn()
    {
        var turnCount = _currentTurn % _playerShapes.Count;
        var newShape = _playerShapes[turnCount];
        return newShape;
    }

    public void SubscribeToOnPlayerTurnUpdate(PlayerTurnUpdate action)
    {
        _onPlayerTurnUpdated += action;
    }

    public void UnsubscribeFromOnPlayerTurnUpdate(PlayerTurnUpdate action)
    {
        _onPlayerTurnUpdated -= action;
    }


    public void SubscribeToOnPlayerWin(PlayerHasWon action)
    {
        _onPlayerWin += action;
    }

    public void UnsubscribeFromOnPlayerWin(PlayerHasWon action)
    {
        _onPlayerWin -= action;
    }

    private bool CheckDirectionPairForVictory(KeyValuePair<Direction, Direction> directionPair, uint startingY,
                                              uint startingX, Shape shape, ref ushort inARowCount)
    {
        if (_board.OutOfBounds(startingY, startingX))
            return false;

        if (shape != _board.GetShapeAtPosition(startingY, startingX))
            return false;

        inARowCount++;

        if (inARowCount >= _shapesInRowToWin)
            return true;

        switch (directionPair.Key)
        {
            case Direction.Up:
                {
                    return CheckDirectionForVictory(directionPair.Key, startingY + 1, startingX, shape, ref inARowCount)
                        || CheckDirectionForVictory(directionPair.Value, startingY - 1, startingX, shape, ref inARowCount);
                }
            case Direction.UpRight:
                {
                    return CheckDirectionForVictory(directionPair.Key, startingY + 1, startingX + 1, shape, ref inARowCount)
                        || CheckDirectionForVictory(directionPair.Value, startingY - 1, startingX - 1, shape, ref inARowCount);
                }
            case Direction.Right:
                return CheckDirectionForVictory(directionPair.Key, startingY, startingX + 1, shape, ref inARowCount)
                    || CheckDirectionForVictory(directionPair.Value, startingY, startingX - 1, shape, ref inARowCount);
            case Direction.DownRight:
                return CheckDirectionForVictory(directionPair.Key, startingY - 1, startingX + 1, shape, ref inARowCount)
                    || CheckDirectionForVictory(directionPair.Value, startingY + 1, startingX - 1, shape, ref inARowCount); ;
            default:
                throw new Exception("What. Don't be here.");
        }
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