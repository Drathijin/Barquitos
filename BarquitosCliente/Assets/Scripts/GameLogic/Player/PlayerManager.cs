using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
  [SerializeField]
  Transform positions_;

  BattleShipMover btsMover_;
  Fleet fleet_;

  [SerializeField]
  List<BattleShipViewer> bShips_;

  bool alreadyAttacked = false;

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
    alreadyAttacked = false;
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

  public void CleanButton()
  {
    alreadyAttacked = true;
    if (currentAttackButton_)
      currentAttackButton_.Disable();
    currentAttackButton_ = null;
  }

  public void SetAttackButton(ButtonEnemyField b)
  {
    if (alreadyAttacked)
      return;
    if (currentAttackButton_)    // LIMPIAR ICONO
      currentAttackButton_.Selected(false);
    currentAttackButton_ = b;
    b.Selected(true);
  }


  public void CheckFleet()
  {
    if (fleet_.ships.Count == 5)
      return;

    int lastX = -1, lastY = -1;
    List<int> ships = new List<int>();
    ships.Add(2);
    ships.Add(3);
    ships.Add(3);
    ships.Add(4);
    ships.Add(5);
    foreach (BattleShip sh in fleet_.ships)
      ships.Remove(sh.GetSize());

    bool center, horizontal, close;
    foreach (var shipLength in ships)
    {
      center = Random.Range(0, 100f) <= 50;
      horizontal = Random.Range(0, 100f) <= 50;
      close = Random.Range(0, 100f) <= 50;
      lookForPosition(ref lastX, ref lastY, shipLength, fleet_, close, horizontal, center);

      BattleShipViewer bs = null;

      foreach (BattleShipViewer b in bShips_)
      {
        if (b.BattleShip().GetSize() == shipLength && b.BattleShip().PlacedPositions().Count == 0)
          bs = b;
      }
      if (!bs)
        continue;
      if (bs.BattleShip().horizontal != horizontal)
        bs.RotateShip();
      btsMover_.ReleaseBattleShipOnPosition(bs, fleet_.GetGrid().GetPos(lastX, lastY));
    }
  }

  private bool checkPos(Fleet myFleet, int x, int y, BattleShip bs)
  {
    return myFleet.IsFree(bs, x, y);
  }
  private void lookForPosition(ref int x, ref int y, int length, Fleet myFleet, bool close, bool horizontal, bool center)
  {
    int targetX, targetY;
    if (center)
    {
      targetX = 4;
      targetY = 4;
    }
    else if (close && y >= 0 && x >= 0)
    {
      targetX = x;
      targetY = y;
    }
    else
    {
      targetX = Random.Range(0, 10);
      targetY = Random.Range(0, 10);
    }
    int bestX = 4, bestY = 4, actualBest = 1000;
    for (int i = 0; i < 10; i++)
      for (int j = 0; j < 10; j++)
      {
        int actual = Mathf.Abs(targetX - j) + Mathf.Abs(targetY - i);
        BattleShip bs = new BattleShip(length);
        bs.horizontal = horizontal;
        if (checkPos(myFleet, j, i, bs) && actual < actualBest)
        {
          bestX = j;
          bestY = i;
          actualBest = actual;
        }
      }
    x = bestX;
    y = bestY;
  }


}
