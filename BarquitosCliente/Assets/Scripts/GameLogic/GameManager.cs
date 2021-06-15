using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MENU,
        PREPARING,
        SELECTING,
        ATTACKING,
        END
    }

    public enum GameType
    {
        AI,
        ONLINE
    }

    private static GameManager instace_;

    [SerializeField]
    private GameObject buttonsPrefabs_;

    private Transform enemyWater_;

    private GameState state_ = GameState.PREPARING;

    private PlayerManager playerMng_;

    private NetworkManager netManager_;

    private AIManager aiManager_;

    private ReadyButton button_;

    private Dictionary<string, Fleet> fleets_ = new Dictionary<string, Fleet>();

    private List<Fleet> enemyFleets_ = new List<Fleet>();

    private int currentEnemyFleet_ = -1;

    private GameType gameType_ = GameType.AI;

    private AIData aiSetup_ = new AIData();

    private void Awake()
    {
        if (instace_)
        {
            Destroy(gameObject);
            return;
        }
        instace_ = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static GameManager Instance()
    {
        return instace_;
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void SetGameType(GameType type)
    {
        gameType_ = type;
    }

    public GameType GetGameType()
    {
        return gameType_;
    }

    public void SetGameType(int type)
    {
        if (type >= 3)
        {
            Debug.LogWarning("Wrong index of GameType " + type);
            return;
        }
        SetGameType((GameType)type);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
            ChangeState(GameState.MENU);
        else
        {
            enemyWater_ = GameObject.Find("WaterOponent").transform;
            GameObject manager = new GameObject("EnemyManager");
            switch (gameType_)
            {
                case GameType.AI:
                    {
                        AIManager ai = manager.AddComponent<AIManager>();
                        ai.Setup(aiSetup_);
                        aiManager_ = ai;
                        break;
                    }
                case GameType.ONLINE:
                    {
                        NetworkManager net = manager.AddComponent<NetworkManager>();
                        net.Setup(playerMng_.GetFleet().Name());
                        netManager_ = net;
                        break;
                    }
            }
					ChangeState(GameState.PREPARING);
        }
    }

    public GameState State()
    {
        return state_;
    }

    public void ChangeState(GameState state)
    {
        state_ = state;

				if(playerMng_)
        	playerMng_.OnStateChanged(state);

        if (aiManager_)
            aiManager_.OnStateChanged(state);
        else if (netManager_)
            netManager_.OnStateChanged(state);

        if(button_)
					button_.OnStateChanged(state);

        if (state == GameState.ATTACKING)
            Invoke("DelayBorrar", 0f);
    }

    public void DelayBorrar()
    {
        ChangeState(GameState.SELECTING);
    }

    public void OnReadyClick()
    {
        if (state_ == GameState.PREPARING)
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

    public GameObject AddEnemyFleet(string name, bool ai)
    {
        if (!enemyWater_)
        {
            Debug.LogError("No enemyWater found");
            return null;
        }
        GameObject g = Instantiate(buttonsPrefabs_, enemyWater_);

        Fleet fleet = g.AddComponent<Fleet>();
        fleet.SetName(name);
        fleets_[name] = fleet;
        enemyFleets_.Add(fleet);
        if (currentEnemyFleet_ == -1)
            currentEnemyFleet_ = 0;
        else
            g.SetActive(false);

        return g;
    }

    public Fleet GetFleet(string id)
    {
        return fleets_[id];
    }

    public void NextFleet() {
        enemyFleets_[currentEnemyFleet_].gameObject.SetActive(false);
        currentEnemyFleet_ = ++currentEnemyFleet_ % enemyFleets_.Count;
        enemyFleets_[currentEnemyFleet_].gameObject.SetActive(true);
    }

    public void PreviousFleet()
    {
        enemyFleets_[currentEnemyFleet_].gameObject.SetActive(false);
        currentEnemyFleet_ = --currentEnemyFleet_ == -1 ? enemyFleets_.Count - 1 : currentEnemyFleet_;
        enemyFleets_[currentEnemyFleet_].gameObject.SetActive(true);
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
        return enemyFleets_[currentEnemyFleet_];
    }

    public PlayerManager PlayerManager()
    {
        return playerMng_;
    }

    public void PlayerLost()
    {
        Debug.Log("YOU LOSE");
				FleetLost(playerMng_.GetFleet().Name());
    }

    public void FleetLost(string fleet)
    {
        fleets_.Remove(fleet);
				if(fleets_.Count == 1)
				{
					Debug.Log("GAME END");
        	Debug.Log(fleets_.First().Key+" WINS");
        	ChangeState(GameState.END);
				}
    }

    public void FleetLost(Fleet fleet)
    {
        FleetLost(fleet.Name());
    }

    public void SetAISetup(AIData data)
    {
        aiSetup_ = data;
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
