using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShipMover : MonoBehaviour
{
    BattleShipViewer selectedBts_;

    PlayerGridPosition currentPosition_;

    PlayerManager plMng;

    private void Update()
    {
        if (selectedBts_)
            selectedBts_.transform.position = Input.mousePosition + new Vector3(-30, 30, 0);
    }

    public bool SelectBattleShip(BattleShipViewer ship)
    {
        if (selectedBts_)
            return false;
        selectedBts_ = ship;
        if (selectedBts_.BattleShip().PlacedPositions().Count != 0)
            plMng.GetFleet().RemoveBattleShip(ship.BattleShip());
        return true;
    }

    public void ReleaseBattleShip(BattleShipViewer ship)
    {
        if (selectedBts_ != ship)
            return;
        if (currentPosition_)
        {
            if (plMng.GetFleet().AddBattleShip(ship.BattleShip(),
                currentPosition_.Data().GetX(),
                currentPosition_.Data().GetY()))
            {
                selectedBts_.transform.position = currentPosition_.transform.position;
            }
            else
            {
                selectedBts_.ResetPosition();
                selectedBts_ = null;
            }
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
