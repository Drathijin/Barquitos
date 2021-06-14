using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet : MonoBehaviour
{
    Grid grid_;

    string name_;

    List<BattleShip> ships_ = new List<BattleShip>();

    private void Awake()
    {
        grid_ = new Grid(GetComponentsInChildren<GridObject>(), this);
    }

    public Grid GetGrid()
    {
        return grid_;
    }

    public bool Attack(int x, int y)    // Recivir el ataque de alguien, devolver resultado
    {
        return grid_.Attack(x, y);
    }

    public void SetName(string name)
    {
        name_ = name;
    }

    public string Name()
    {
        return name_;
    }

    public bool AddBattleShip(BattleShip ship, int x, int y)
    {
        if (!((x <= 9 && x >= 0) && (y <= 9 && y >= 0)))
            return false;

        for (int i = 0; i < ship.GetSize(); i++)
        {
            GridObject p = grid_.GetPos(
                x + i * (ship.horizontal ? 1 : 0),
                y + i * (ship.horizontal ? 0 : 1));
            if (!p || p.Data().Ship() != null)
                return false;
        }

        ships_.Add(ship);
        SetShipPosition(ship, x, y);
        return true;
    }

    public void RemoveBattleShip(BattleShip ship)
    {
        List<BattleShip.Position> pos = ship.PlacedPositions();
        for (int i = 0; i < pos.Count; i++)
        {
            GridObject p = grid_.GetPos(pos[i].x, pos[i].y);
            p.SetShip(null);
        }
        ships_.Remove(ship);
    }

    private void SetShipPosition(BattleShip ship, int x, int y)
    {
        for (int i = 0; i < ship.GetSize(); i++)
        {
            int x_ = x + i * (ship.horizontal ? 1 : 0);
            int y_ = y + i * (ship.horizontal ? 0 : 1);
            GridObject p = grid_.GetPos(x_, y_);
            ship.AddPlacedPosition(x_, y_);
            p.SetShip(ship);
        }
    }

    public bool IsDestroyed()
    {
        bool destroyed = true;

        foreach (BattleShip b in ships_)
            destroyed &= b.Destroyed();

        return destroyed;
    }
}
