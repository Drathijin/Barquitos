using System.Collections.Generic;

public class Grid
{
    private GridObject[,] grid;

    public Grid(GridObject[] objs, Fleet fleet)
    {
        grid = new GridObject[10, 10];

        int i = 0;
        foreach(GridObject g in objs)
        {
            int x = i % 10;
            int y = i / 10;
            g.SetPosition(x, y);
            g.SetFleet(fleet);
            grid[x, y] = g;
            i++;
        }
    }

    public bool Attack(int x, int y)
    {
        return grid[x, y].Attack();
    }

    public GridObject GetPos(int x , int y)
    {
        if (x >= 10 || y >= 10)
            return null;
        return grid[x, y];
    }
}
