using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid
{
    private GridObject[,] grid;

    public Grid(GridObject[] objs, Fleet fleet)
    {
        grid = new GridObject[10, 10];

        int i = 0;
        foreach(GridObject g in objs)
        {
            if(g.GetComponent<Button>())
            {
                g.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance().AudioManager().PlayEffect(AudioManager.Effecs.Click));
            }
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
        if (x >= 10 || y >= 10 || x < 0 || y < 0)
            return null;
        return grid[x, y];
    }
}
