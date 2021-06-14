using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShipMover : MonoBehaviour
{
    BattleShip selectedBts_;

    PlayerGridPosition currentPosition_;

    PlayerManager plMng;

    private void Update()
    {
        if (selectedBts_)
            selectedBts_.transform.position = Input.mousePosition + new Vector3(-30, 30, 0);
    }

    public bool SelectBattleShip(BattleShip ship)
    {
        if (selectedBts_)
            return false;
        selectedBts_ = ship;
        if (selectedBts_.PlacedPosition().x != -1)
        {
            currentPosition_ = plMng.GetGrid().GetPos(ship.PlacedPosition().x, ship.PlacedPosition().y) as PlayerGridPosition;
            SetShipPosition(false);
        }
        return true;
    }

    public void ReleaseBattleShip(BattleShip ship)
    {
        if (selectedBts_ != ship)
            return;
        if (currentPosition_)
        {
            for (int i = 0; i < ship.GetSize(); i++)
            {
                GridObject p = plMng.GetGrid().GetPos(
                    currentPosition_.Data().GetX() + i * (ship.horizontal ? 1 : 0),
                    currentPosition_.Data().GetY() + i * (ship.horizontal ? 0 : 1));
                if (!p || p.Data().Ship())
                {
                    selectedBts_.ResetPosition();
                    selectedBts_ = null;
                    return;
                }
            }
            ship.SetPlacedPosition(currentPosition_.Data().GetX(), currentPosition_.Data().GetY());
            SetShipPosition(true);
        }
        else
            selectedBts_.ResetPosition();
        selectedBts_ = null;
    }

    private void PrintGrid()
    {
        Grid g = plMng.GetGrid();

        for (int i = 0; i < 10; i++)
        {
            string outGrid = "";
            for (int j = 0; j < 10; j++)
            {
                outGrid += g.GetPos(j, i).Data().Ship() + " | ";
            }

            Debug.Log(outGrid);
        }
    }

    private void SetShipPosition(bool set)
    {
        for (int i = 0; i < selectedBts_.GetSize(); i++)
        {
            GridObject p = plMng.GetGrid().GetPos(
                currentPosition_.Data().GetX() + i * (selectedBts_.horizontal ? 1 : 0),
                currentPosition_.Data().GetY() + i * (selectedBts_.horizontal ? 0 : 1));
            p.SetShip(set);
        }
        selectedBts_.transform.position = currentPosition_.transform.position;
    }

    public void OnGridHover(PlayerGridPosition pos)
    {
        if (currentPosition_ == pos)
            return;
        currentPosition_ = pos;
    }

    public void OnGridHoverExit(PlayerGridPosition pos)
    {
        if (pos != currentPosition_)
            return;
        currentPosition_ = null;
    }

    public void SetPlayer(PlayerManager manager)
    {
        plMng = manager;
    }
}
