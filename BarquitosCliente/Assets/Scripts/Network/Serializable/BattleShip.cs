using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

public class BattleShip
{
    int size = 2;

    public bool horizontal = true;

    public struct Position
    {
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x;
        public int y;
    }

    List<Position> placedPositions_;

    public BattleShip(int size)
    {
        placedPositions_ = new List<Position>();
        this.size = size;
    }

    public List<Position> PlacedPositions()
    {
        return placedPositions_;
    }

    public void AddPlacedPosition(int x, int y)
    {
        placedPositions_.Add(new Position(x, y));
    }

    public bool CheckAttack(int x, int y)
    {
        for (int i = 0; i < placedPositions_.Count; i++)
        {
            if (placedPositions_[i].x == x && placedPositions_[i].y == y)
            {
                placedPositions_.RemoveAt(i);
                break;
            }
        }
        return Destroyed();
    }

    public bool Destroyed()
    {
        return placedPositions_.Count <= 0;
    }

    public int GetSize()
    {
        return size;
    }
}
