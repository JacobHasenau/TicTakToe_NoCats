using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void PlayerTurnUpdate(object sender, Shape newPlayersTurn);
public delegate void PlayerHasWon(object sender, WinningInformation winningPlayer);

public class TurnManager
{
    private TicTacToeBoard _board;
    private VictoryChecker _victoryChecker;
    private ushort _shapesInRowToWin;
    private IList<Shape> _playerShapes;
    private short _currentTurn;
    private PlayerTurnUpdate _onPlayerTurnUpdated;
    private PlayerHasWon _onPlayerWin;

    public TurnManager(TicTacToeBoard board, ushort shapesInRowToWin, IReadOnlyCollection<Shape> playerShapes)
    {
        _board = board;
        _shapesInRowToWin = shapesInRowToWin;
        SetActivePlayers(playerShapes);
        _currentTurn = 0;
        _victoryChecker = new VictoryChecker(_board);
    }

    public void ResetManager(TicTacToeBoard board, ushort shapesInRowToWin, IReadOnlyCollection<Shape> playerShapes)
    {
        _board = board;
        _shapesInRowToWin = shapesInRowToWin;
        SetActivePlayers(playerShapes);
        _currentTurn = 0;
        _victoryChecker = new VictoryChecker(_board);
    }

    public void SetActivePlayers(IReadOnlyCollection<Shape> playerShapes)
    {
        _playerShapes = playerShapes
            .OrderBy(x => (int)x)
            .ToList();
    }

    public void PlayerMakesTurn(PlayerMove move)
    {
        if (!_board.AcceptPlayerMove(move))
            throw new Exception("Replace me with a delegate at somepoint.");

        var victoryResult = _victoryChecker.PlayerWonOnMove(move, _shapesInRowToWin);

        if (victoryResult.VictoryAchived)
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
}