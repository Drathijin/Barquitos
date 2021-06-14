using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    Transform positions_;

    BattleShipMover btsMover_;
    Fleet fleet_;

    ButtonEnemyField currentAttackButton_;

    private void Start()
    {
        btsMover_ = GetComponent<BattleShipMover>();
        btsMover_.SetPlayer(this);
        fleet_ = GetComponentInChildren<Fleet>();

        fleet_.SetName("Player");                       // BORRAR EN SU MOMENTO

        if (GameManager.Instance())
        {
						Debug.Log("Adding fleet to GM");
            GameManager.Instance().SetPlayerManager(this);
            GameManager.Instance().AddExistingFleet(fleet_);
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
        return fleet_.GetGrid();
    }

    public Fleet GetFleet()
    {
        return fleet_;
    }

    public void OnStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.PREPARING:
                break;
            case GameManager.GameState.SELECTING:
                ManageTurn();
                break;
            case GameManager.GameState.ATTACKING:
                ResolveTurn();
                break;
            case GameManager.GameState.END:
                break;
            default:
                break;
        }
    }

    void ManageTurn()
    {
        if (fleet_.IsDestroyed())
            GameManager.Instance().PlayerLost();
    }

    void ResolveTurn()
    {
        if (!currentAttackButton_)
            return;
        int x = currentAttackButton_.Data().GetX();
        int y = currentAttackButton_.Data().GetY();
        //currentAttackButton_.Fleet().Attack(x, y);
        fleet_.Attack(x, y);

        // LIMPIAR ICONO
        currentAttackButton_ = null;
    }

    public void SetAttackButton(ButtonEnemyField b)
    {
        if (currentAttackButton_)    // LIMPIAR ICONO
            currentAttackButton_.Selected(false);
        currentAttackButton_ = b;
        b.Selected(true);
    }
}
