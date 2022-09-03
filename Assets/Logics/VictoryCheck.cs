using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCheck
{
    public Shape? CheckBoardForVictoryAchived(TicTacToeBoard gameBoard, ushort neededInARow)
    {
        for (uint x = 0; x < gameBoard.XSize; x++)
        {
            for (uint y = 0; y < gameBoard.YSize; y++)
            {
                var shapeToCheck = gameBoard.GetShapeAtPosition(x, y);
                if(shapeToCheck != null)
                {
                    var shapeWon = CheckPositionForVictory(gameBoard, x, y, neededInARow, shapeToCheck.Value);
                    if (shapeWon)
                        return shapeToCheck;
                }
            }
        }

        return null;
    }

    public bool CheckPositionForVictory(TicTacToeBoard board, uint posX, uint posY, ushort neededInARow, Shape player)
    {
        return false;
    }
}
