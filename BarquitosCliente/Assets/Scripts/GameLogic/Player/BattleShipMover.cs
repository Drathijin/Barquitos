using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShipMover : MonoBehaviour
{
    BattleShip selectedBts_;

    PlayerGridPosition currentPosition_;

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
            selectedBts_.transform.position = currentPosition_.transform.position;
        else
            selectedBts_.ResetPosition();
        selectedBts_ = null;
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
}
