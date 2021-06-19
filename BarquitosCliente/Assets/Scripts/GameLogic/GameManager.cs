using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MENU,
        WAITINGFORPLAYERS,
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

    public static Object lock_;

    public Transform enemyWater_;

    #region Managers

    private PlayerManager playerMng_;

    private NetworkManager netManager_;

    private AIManager aiManager_;

    #endregion

    #region FleetsData

    private Dictionary<string, Fleet> fleets_ = new Dictionary<string, Fleet>();

    private Dictionary<string, bool> fleetsReady_ = new Dictionary<string, bool>();

    public Dictionary<string, bool> FleetsReady
    {
        get { return fleetsReady_; }
        private set { fleetsReady_ = value; }
    }

    private List<Fleet> enemyFleets_ = new List<Fleet>();

    private int currentEnemyFleet_ = -1;

    #endregion

    #region GameData

    public GameState state_ = GameState.PREPARING;

    private GameType gameType_ = GameType.AI;

    private AIData aiSetup_ = new AIData();

    private NetworkData networkSetup = new NetworkData();

    public string ip = "127.0.0.1";

    public string port = "8080";

    public string playerName = "Player";
    #endregion

    #region UIVariables
    [Header("UI Variables")]
    [SerializeField]
    private GameObject buttonsPrefabs_;

    private WInnerText winText_;

    private ResultText resultText_;

    private ReadyButton button_;

    private NameUI playerNameUI_, enemyNameUI_;

    private WaitingText waitingText_;

    #endregion

    private void Awake()
    {
        if (instace_)
        {
            Destroy(gameObject);
            return;
        }
        instace_ = this;
        lock_ = new Object();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static GameManager Instance()
    {
        return instace_;
    }

    #region SceneManagement
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
            ChangeState(GameState.MENU);
        else
        {
            fleets_ = new Dictionary<string, Fleet>();
            fleetsReady_ = new Dictionary<string, bool>();
            enemyFleets_ = new List<Fleet>();

            enemyWater_ = GameObject.Find("WaterOponent").transform;
            GameObject manager = new GameObject("EnemyManager");
            switch (gameType_)
            {

                case GameType.AI:
                    {
                        AIManager ai = manager.AddComponent<AIManager>();
                        aiManager_ = ai;
                        ai.Setup(aiSetup_);
                        ChangeState(GameState.PREPARING);
                        break;
                    }
                case GameType.ONLINE:
                    {
                        netManager_ = new NetworkManager();
                        playerName = networkSetup.playerName;
                        ChangeState(GameState.WAITINGFORPLAYERS);
                        netManager_.Setup(networkSetup, ip, port);
                        break;
                    }
            }
        }
    }

    #endregion

    #region States
    public void ChangeState(GameState state)
    {
        state_ = state;

        ResetChecks();

        if (button_)
            button_.OnStateChanged(state);

        if (playerMng_)
            playerMng_.OnStateChanged(state);

        if (aiManager_)
            //Invoke("DelayBorrar", 10f);
            aiManager_.OnStateChanged(state);
        else if (netManager_ != null)
            netManager_.OnStateChanged(state);

        if (state == GameState.ATTACKING)
            ChangeState(GameState.SELECTING);

    }

    public void DelayBorrar()
    {
        aiManager_.OnStateChanged(state_);
    }

    public void ReadyCheck(string name, bool set)
    {
        fleetsReady_[name] = set;

        bool ready = true;

        foreach (var b in fleetsReady_)
            ready &= b.Value;

        if (ready && fleetsReady_.Count > 1)
            ReadyChange();
    }

    private void ResetChecks()
    {
        foreach (var b in fleets_)
            fleetsReady_[b.Key] = false;
    }

    public void OnReadyClick()
    {
        if (state_ == GameState.END)
            ReadyChange();
        string plName = playerMng_.GetFleet().Name();
        ReadyCheck(plName, !fleetsReady_[plName]);
    }

    private void ReadyChange()
    {
        switch (state_)
        {
            case GameState.PREPARING:
                ChangeState(GameState.SELECTING);
                break;
            case GameState.SELECTING:
                ChangeState(GameState.ATTACKING);
                break;
            case GameState.END:
                LoadLevel("Menu");
                break;
            default:
                break;
        }
    }

    public void PlayerLost()
    {
        Debug.Log("YOU LOSE");
        FleetLost(playerMng_.GetFleet().Name());
    }

    #endregion

    #region FleetsManagement
    public void FleetLost(string fleet)
    {
        fleets_.Remove(fleet);
        if (fleets_.Count == 1)
        {
            Debug.Log("GAME END");
            Debug.Log(fleets_.First().Key + " WINS");
            winText_.OnEnd(fleets_.First().Key);
            resultText_.OnEnd(fleets_.First().Key == playerMng_.GetFleet().Name());
            ChangeState(GameState.END);
        }
    }

    public void FleetLost(Fleet fleet)
    {
        FleetLost(fleet.Name());
    }

    public void SetEnemyFleet(int enemyFleet)
    {
        if (enemyFleets_.Count == 0)
            return;
        currentEnemyFleet_ = enemyFleet;
        enemyFleets_[currentEnemyFleet_].gameObject.SetActive(true);
        if (enemyNameUI_)
            enemyNameUI_.SetName(enemyFleets_[currentEnemyFleet_].Name());
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
        fleetsReady_[name] = false;
        enemyFleets_.Add(fleet);
        if (currentEnemyFleet_ == -1)
            SetEnemyFleet(0);
        else
            g.SetActive(false);

        return g;
    }

    public void NextFleet()
    {
        if (enemyFleets_.Count == 0)
            return;
        enemyFleets_[currentEnemyFleet_].gameObject.SetActive(false);
        SetEnemyFleet(++currentEnemyFleet_ % enemyFleets_.Count);
    }

    public void PreviousFleet()
    {
        if (enemyFleets_.Count == 0)
            return;
        enemyFleets_[currentEnemyFleet_].gameObject.SetActive(false);
        SetEnemyFleet(--currentEnemyFleet_ == -1 ? enemyFleets_.Count - 1 : currentEnemyFleet_);
    }

    public void AddExistingFleet(Fleet fleet)
    {
        fleets_[fleet.Name()] = fleet;
        fleetsReady_[fleet.Name()] = false;
    }

    #endregion

    #region Getters
    public List<string> GetPlayerList()
    {
        List<string> list = fleets_.Keys.ToList<string>();
        return list;
    }

    public Fleet GetFleet(string id)
    {
        return fleets_[id];
    }

    public Fleet CurrentEnemyFleet()
    {
        return enemyFleets_[currentEnemyFleet_];
    }

    public PlayerManager PlayerManager()
    {
        return playerMng_;
    }

    public GameType GetGameType()
    {
        return gameType_;
    }

    public GameState State()
    {
        return state_;
    }

    public bool IsBr()
    {
        return networkSetup.battleRoyale;
    }

    #endregion

    #region Setters
    public void SetPlayerManager(PlayerManager mng)
    {
        playerMng_ = mng;
        playerMng_.SetName(playerName);
    }

    public void SetAISetup(AIData data)
    {
        aiSetup_ = data;
    }

    public void SetNetworkSetup(NetworkData data)
    {
        networkSetup = data;
    }

    public void SetResultText(ResultText text)
    {
        resultText_ = text;
        text.gameObject.SetActive(false);
    }

    public void SetWinnerText(WInnerText text)
    {
        winText_ = text;
        text.gameObject.SetActive(false);
    }

    public void SetPlayerNameUI(NameUI name)
    {
        playerNameUI_ = name;
        playerNameUI_.SetName(playerName);
    }

    public void SetReadyButton(ReadyButton b)
    {
        button_ = b;
    }

    public void SetEnemyNameUI(NameUI name)
    {
        enemyNameUI_ = name;
        SetEnemyFleet(currentEnemyFleet_);
    }

    public void SetAIManager(AIManager ai)
    {
        if (ai)
            aiManager_ = ai;
    }

    public void SetWaitingText(WaitingText text)
    {
        waitingText_ = text;
    }

    public void PlayersReady()
    {
        Debug.Log("aaaaaa");
        if (waitingText_)
            waitingText_.gameObject.SetActive(false);
        ChangeState(GameState.PREPARING);
    }

    public void SetGameType(GameType type)
    {
        gameType_ = type;
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

    #endregion

    public void Exit()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        if (netManager_ != null)
            netManager_.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
