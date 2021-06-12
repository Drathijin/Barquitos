using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MENU,
        PREPARING,
        SELECTING,
        ATTACKING
    }

    private static GameManager instace_;

    private GameState state_;

    private PlayerManager playerMng_;

    private List<EnemyFleet> enemyFleets_;

    private EnemyFleet currentEnemyFleet_;

    private void Awake()
    {
        if (instace_)
        {
            Destroy(gameObject);
            return;
        }
        instace_ = this;
        DontDestroyOnLoad(gameObject);
        state_ = GameState.PREPARING;
    }

    public static GameManager Instance()
    {
        return instace_;
    }

    public GameState State()
    {
        return state_;
    }

    public void ChangeState(GameState state)
    {
        state_ = state;
    } 

    public void SetPlayerManager(PlayerManager mng)
    {
        playerMng_ = mng;
    }

    public void AddEnemyFleet(EnemyFleet mng)
    {
				Debug.Log("AAAAAAAAAAHHHHHH");
        enemyFleets_.Add(mng);
				currentEnemyFleet_ = mng;
    }

    public EnemyFleet CurrentEnemyFleet()
    {
        return currentEnemyFleet_;
    }

    public PlayerManager PlayerManager()
    {
        return playerMng_;
    }
}
