public class VictoryCheckResult
{
    public bool VictoryAchived;
    public Shape? VictoryShape;

    public VictoryCheckResult(bool victoryAchived, Shape victoryShape)
    {
        VictoryAchived = victoryAchived;
        VictoryShape = victoryShape;
    }

    public VictoryCheckResult()
    {
        VictoryAchived = false;
        VictoryShape = null;
    }

    public static VictoryCheckResult False()
    {
        return new VictoryCheckResult();
    }
}
