using System.Collections.Generic;

public enum Difficulty
{
    EASY,
    MEDIUM,
    HARD
}

public class AIData
{
    public AIData()
    {
        centerPriority = 0;
        closerPriority = 0;
        horizontalPriority = 0.5;
        diff = Difficulty.MEDIUM;
    }

    public double centerPriority;
    public double closerPriority;
    public double horizontalPriority;
    public Difficulty diff;
}
