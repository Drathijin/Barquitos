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
        if (!selectedBts_)
            return;

        selectedBts_.transform.position = Input.mousePosition + new Vector3(-30, 30, 0);
    }

    public bool SelectBattleShip(BattleShip ship)
    {
        if (selectedBts_)
            return false;
        selectedBts_ = ship;
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
                if (!p || p.Data().Boat())
                {
                    selectedBts_.ResetPosition();
                    selectedBts_ = null;
                    return;
                }
            }
            SetShipPosition();
        }
        else
            selectedBts_.ResetPosition();
        selectedBts_ = null;
    }

    private void SetShipPosition()
    {
        for (int i = 0; i < selectedBts_.GetSize(); i++)
        {
            GridObject p = plMng.GetGrid().GetPos(
                currentPosition_.Data().GetX() + i * (selectedBts_.horizontal ? 1 : 0),
                currentPosition_.Data().GetY() + i * (selectedBts_.horizontal ? 0 : 1));
            p.SetBoat(true);
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
