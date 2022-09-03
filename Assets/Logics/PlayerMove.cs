public struct PlayerMove
{
    private readonly Shape _shape;
    private readonly uint _posX;
    private readonly uint _posY;

    public PlayerMove(Shape shape, uint posY, uint posX)
    {
        _shape = shape;
        _posY = posY;
        _posX = posX;
    }

    public Shape PlayerShape { get { return _shape; } }
    public uint PosX { get { return _posX; } }
    public uint PosY { get { return _posY; } }
}
