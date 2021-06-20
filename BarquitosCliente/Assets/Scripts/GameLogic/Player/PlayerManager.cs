using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    Transform positions_;

    BattleShipMover btsMover_;
    Fleet fleet_;

    public ButtonEnemyField currentAttackButton_;


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
        }
    }

    public bool SelectBattleShip(BattleShipViewer ship)
    {
        if (GameManager.Instance().State() != GameManager.GameState.PREPARING
            || GameManager.Instance().FleetsReady[fleet_.Name()])
            return false;
        return btsMover_.SelectBattleShip(ship);
    }

    public void ReleaseBattleShip(BattleShipViewer ship)
    {
        if (GameManager.Instance().State() != GameManager.GameState.PREPARING
             || GameManager.Instance().FleetsReady[fleet_.Name()])
            return;
        btsMover_.ReleaseBattleShip(ship);
    }

    public void SetName(string name)
    {
        fleet_.SetName(name);
    }

    public void OnGridHover(PlayerGridPosition pos)
    {
        if (GameManager.Instance().State() != GameManager.GameState.PREPARING
            || GameManager.Instance().FleetsReady[fleet_.Name()])
            return;
        btsMover_.OnGridHover(pos);
    }

    public void OnGridHoverExit(PlayerGridPosition pos)
    {
        if (GameManager.Instance().State() != GameManager.GameState.PREPARING
            || GameManager.Instance().FleetsReady[fleet_.Name()])
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
        if (GameManager.Instance().GetGameType() == GameManager.GameType.AI &&
            fleet_.IsDestroyed())
            GameManager.Instance().PlayerLost();
    }

    void ResolveTurn()
    {
        if (!currentAttackButton_ || GameManager.Instance().GetGameType() == GameManager.GameType.ONLINE)
            return;
        int x = currentAttackButton_.Data().GetX();
        int y = currentAttackButton_.Data().GetY();
        currentAttackButton_.Fleet().Attack(x, y);

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
