using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    [SerializeField]
    private GameObject buttonsPrefabs_;
    [SerializeField]
    private Transform enemyWater_;

    private GameState state_;

    private PlayerManager playerMng_;

    private NetworkManager netManager_;

    private AIManager aiManager_;

    private ReadyButton button_;

    private Dictionary<string, Fleet> fleets_ = new Dictionary<string, Fleet>();

    private Fleet currentEnemyFleet_;

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

    private void Start()
    {
      fleets_ = new Dictionary<string, Fleet>(); 
      AddEnemyFleet("pepepopo", true);
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

        playerMng_.OnStateChanged(state);

        if (aiManager_)
            aiManager_.OnStateChanged(state);
        else if (netManager_)
            netManager_.OnStateChanged(state);

        button_.OnStateChanged(state);

        if (state == GameState.ATTACKING)
            Invoke("DelayBorrar", 2f);
    }

    public void DelayBorrar()
    {
        ChangeState(GameState.SELECTING);
    }

    public void OnReadyClick()
    {
        if(state_ == GameState.PREPARING)
            ChangeState(GameState.SELECTING);
        else if (state_ == GameState.SELECTING)
            ChangeState(GameState.ATTACKING);

    }
    public void SetReadyButton(ReadyButton b)
    {
        button_ = b;
    }

    public void SetPlayerManager(PlayerManager mng)
    {
        playerMng_ = mng;
    }

    public void AddEnemyFleet(string name, bool ai)
    {
        GameObject g = Instantiate(buttonsPrefabs_, enemyWater_);

        Fleet fleet = g.AddComponent<Fleet>();
        fleet.SetName(name);
        fleets_[name] = fleet;
        currentEnemyFleet_ = fleet;

        //Asumimos or ahora easyAI
        if(ai)
        {
          MediumBehaviour eb = g.AddComponent<MediumBehaviour>();
          eb.Setup(fleet);
          if(aiManager_)
            aiManager_.addBehaviour(name, (IABehaviour)eb);
        }
		}

    public Fleet GetFleet(string id)
    {
        return fleets_[id];
    }
    public List<string> GetPlayerList()
    {
        List<string> list = fleets_.Keys.ToList<string>();
        return list;
    }

    public void AddExistingFleet(Fleet fleet)
    {
        fleets_[fleet.Name()] = fleet;
    }

    public void SetAIManager(AIManager ai)
    {
        if (ai)
            aiManager_ = ai;
    }

    public void SetNetworkManager(NetworkManager net)
    {
        if (net)
            netManager_ = net;
    }

    public Fleet CurrentEnemyFleet()
    {
        return currentEnemyFleet_;
    }

    public PlayerManager PlayerManager()
    {
        return playerMng_;
    }
}
