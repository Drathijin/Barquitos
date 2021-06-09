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

    private GridSetter playerGrid_, enemyGrid_;

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

    public PlayerManager PlayerManager()
    {
        return playerMng_;
    }

    public void SetGrid(GridSetter grid)
    {
        if (grid.type == GridSetter.GridType.ENEMY)
            enemyGrid_ = grid;
        else
            playerGrid_ = grid;
    }
}
