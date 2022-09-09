using System.Collections.Generic;

public class WinningInformation
{
    public IReadOnlyList<Shape> PlayingShapes;
    public Shape WinningShape;
    public int TurnsTaken;

    public WinningInformation(IReadOnlyList<Shape> playingShapes, Shape winningShape, int turnsTaken)
    {
        PlayingShapes = playingShapes;
        WinningShape = winningShape;
        TurnsTaken = turnsTaken;
    }
}
