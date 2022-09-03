using System;

public class InvalidMoveException : Exception
{
    public InvalidMoveException(PlayerMove move, Shape? existingShape)
        : base($"Player move of shape: {move.PlayerShape} " +
            $"at PosY: {move.PosY}, and PosX: {move.PosX} " +
            $"conlficts with already existing {existingShape}.") 
    { }
}
