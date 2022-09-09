using System;
using System.Collections.Generic;

public class VictoryChecker
{
    private Dictionary<Direction, Direction> _directionPairs
        = new Dictionary<Direction, Direction>{ { Direction.Up, Direction.Down },
                                                { Direction.UpRight, Direction.DownLeft },
                                                { Direction.Right, Direction.Left },
                                                { Direction.DownRight, Direction.UpLeft }
        };
    private TicTacToeBoard _board;

    public VictoryChecker(TicTacToeBoard board)
    {
        _board = board;
    }

    public VictoryCheckResult PlayerWonOnMove(PlayerMove move, ushort inARowToWin)
    {
        if (_board.GetShapeCount(move.PlayerShape) < inARowToWin)
            return VictoryCheckResult.False();

        foreach (var directions in _directionPairs)
        {
            ushort inARowFound = 0;

            var playerWon = CheckDirectionPairForVictory(directions, move.PosY, move.PosX, move.PlayerShape, inARowToWin, ref inARowFound);

            if (playerWon)
                return new VictoryCheckResult(true, move.PlayerShape);
        }

        return VictoryCheckResult.False();
    }

    private bool CheckDirectionPairForVictory(KeyValuePair<Direction, Direction> directionPair, uint startingY,
                                              uint startingX, Shape shape, ushort inARowToWin, ref ushort inARowCount)
    {
        if (_board.OutOfBounds(startingY, startingX))
            return false;

        if (shape != _board.GetShapeAtPosition(startingY, startingX))
            return false;

        inARowCount++;

        if (inARowCount >= inARowToWin)
            return true;

        switch (directionPair.Key)
        {
            case Direction.Up:
                {
                    return CheckDirectionForVictory(directionPair.Key, startingY + 1, startingX, shape, inARowToWin, ref inARowCount)
                        || CheckDirectionForVictory(directionPair.Value, startingY - 1, startingX, shape, inARowToWin, ref inARowCount);
                }
            case Direction.UpRight:
                {
                    return CheckDirectionForVictory(directionPair.Key, startingY + 1, startingX + 1, shape, inARowToWin, ref inARowCount)
                        || CheckDirectionForVictory(directionPair.Value, startingY - 1, startingX - 1, shape, inARowToWin, ref inARowCount);
                }
            case Direction.Right:
                return CheckDirectionForVictory(directionPair.Key, startingY, startingX + 1, shape, inARowToWin, ref inARowCount)
                    || CheckDirectionForVictory(directionPair.Value, startingY, startingX - 1, shape, inARowToWin, ref inARowCount);
            case Direction.DownRight:
                return CheckDirectionForVictory(directionPair.Key, startingY - 1, startingX + 1, shape, inARowToWin, ref inARowCount)
                    || CheckDirectionForVictory(directionPair.Value, startingY + 1, startingX - 1, shape, inARowToWin, ref inARowCount); ;
            default:
                throw new Exception("What. Don't be here.");
        }
    }

    private bool CheckDirectionForVictory(Direction direction, uint currY, uint currX, Shape shape, ushort inARowToWin, ref ushort inARowCount)
    {
        if (_board.OutOfBounds(currY, currX))
            return false;

        if (shape != _board.GetShapeAtPosition(currY, currX))
            return false;

        inARowCount++;

        if (inARowCount >= inARowToWin)
            return true;

        switch (direction)
        {
            case Direction.Up:
                return CheckDirectionForVictory(direction, currY + 1, currX, shape, inARowToWin, ref inARowCount);
            case Direction.UpRight:
                return CheckDirectionForVictory(direction, currY + 1, currX + 1, shape, inARowToWin, ref inARowCount);
            case Direction.Right:
                return CheckDirectionForVictory(direction, currY, currX + 1, shape, inARowToWin, ref inARowCount);
            case Direction.DownRight:
                return CheckDirectionForVictory(direction, currY - 1, currX + 1, shape, inARowToWin, ref inARowCount);
            case Direction.Down:
                return CheckDirectionForVictory(direction, currY - 1, currX, shape, inARowToWin, ref inARowCount);
            case Direction.DownLeft:
                return CheckDirectionForVictory(direction, currY - 1, currX - 1, shape, inARowToWin, ref inARowCount);
            case Direction.Left:
                return CheckDirectionForVictory(direction, currY, currX - 1, shape, inARowToWin, ref inARowCount);
            case Direction.UpLeft:
                return CheckDirectionForVictory(direction, currY + 1, currX - 1, shape, inARowToWin, ref inARowCount);
            default:
                throw new Exception("What. Don't be here.");
        }
    }
}
