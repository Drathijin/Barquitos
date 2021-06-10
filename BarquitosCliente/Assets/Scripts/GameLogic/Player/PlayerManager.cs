using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    Transform positions_;

    BattleShipMover btsMover_;
    Grid grid_;

    private void Start()
    {
        btsMover_ = GetComponent<BattleShipMover>();
        btsMover_.SetPlayer(this);

        grid_ = new Grid(positions_.GetComponentsInChildren<GridObject>());

        if (GameManager.Instance())
        {
            GameManager.Instance().SetPlayerManager(this);
            if (GameManager.Instance().State() != GameManager.GameState.PREPARING)
                btsMover_.enabled = false;
        }
    }

    public bool SelectBattleShip(BattleShip ship)
    {
        if (GameManager.Instance().State() != GameManager.GameState.PREPARING)
            return false;
        return btsMover_.SelectBattleShip(ship);
    }

    public void ReleaseBattleShip(BattleShip ship)
    {
        if (GameManager.Instance().State() != GameManager.GameState.PREPARING)
            return;
        btsMover_.ReleaseBattleShip(ship);
    }

    public void OnGridHover(PlayerGridPosition pos)
    {
        if (GameManager.Instance().State() != GameManager.GameState.PREPARING)
            return;
        btsMover_.OnGridHover(pos);
    }

    public void OnGridHoverExit(PlayerGridPosition pos)
    {
        if (GameManager.Instance().State() != GameManager.GameState.PREPARING)
            return;
        btsMover_.OnGridHoverExit(pos);
    }

    public Grid GetGrid()
    {
        return grid_;
    }
}
