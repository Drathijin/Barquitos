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

  public static GameManager Instance()
  {
    return instace_;
  }

  public static Object lock_;

  public Transform enemyWater_;

  #region Managers

  private PlayerManager playerMng_;

  private NetworkManager netManager_;

  private AIManager aiManager_;
  private AudioManager audioManager_;

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

  #region NetworkingData

  public string id_ = "123id123id123id123id123id";

  public List<string> potentialFleets_ = new List<string>();

  public bool playersReady_ = false;

  public bool nextState = false;

  public List<ServerAttack.AttackResult> attacks_ = new List<ServerAttack.AttackResult>();

  public bool ConectionErrorExit = false;

  public string ErrorMessage = "";

  public Queue<string> fleetExit = new Queue<string>();

  private float timerEnd = 0;
  private float timerTime = 0;

  private bool timer = false;
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

  private IdText idText_;

  private TimerText timerText_;

  #endregion


  #region Functions

  #region MonoBehaviour

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

  private void Update()
  {
    if (timer && gameType_ != GameType.AI)
    {
      timerTime = timerEnd - Time.time;
      if (timerTime <= 0)
        OnTimerEnd();
      else
        timerText_.SetTime(timerTime);
    }
    lock (lock_)
    {
      if (ConectionErrorExit)
        LoadLevel("Error");
      while (fleetExit.Count != 0)
        FleetLost(fleetExit.Dequeue());
      while (potentialFleets_.Count > 0)
      {
        AddEnemyFleet(potentialFleets_[0]);
        potentialFleets_.RemoveAt(0);
      }
      while (attacks_.Count > 0)
      {
        ServerAttack.AttackResult res = attacks_[0];
        Fleet fleet = GetFleet(res.name);
        fleet.GetGrid().GetPos(res.x, res.y).SetState(res.hit ?
            CellData.CellState.HIT : CellData.CellState.MISSED);
        attacks_.RemoveAt(0);
        if (res.fleetDestroyed)
          FleetLost(res.name);
      }
      if (state_ == GameState.WAITINGFORPLAYERS && playersReady_)
      {
        if (waitingText_)
          waitingText_.gameObject.SetActive(false);
        ChangeState(GameState.PREPARING);
      }
      if (nextState)
        NextState();
      SetId(id_);
    }
  }

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

  #endregion

  #region SceneManagement
  public void LoadLevel(string level)
  {
    if (netManager_ != null)
    {
      netManager_.OnDestroy();
      netManager_ = null;
    }
    SceneManager.LoadScene(level);
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    ConectionErrorExit = false;
    ChangeState(GameState.MENU);
    if (scene.name == "Game")
    {
      audioManager_.PlayMusic(global::AudioManager.Music.Game, true);
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
            Invoke("NoResponse", 2f);
            break;
          }
      }
    }
  }

  private void NoResponse()
  {
    if (!netManager_.conected_)
    {
      ErrorMessage = "Conection time out";
      LoadLevel("Error");
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
      aiManager_.OnStateChanged(state);
    else if (netManager_ != null)
      netManager_.OnStateChanged(state);

    if (state == GameState.ATTACKING)
      ChangeState(GameState.SELECTING);
    if (gameType_ == GameType.ONLINE)
      StartTimer();
  }

  private void NextState()
  {
    nextState = false;
    ReadyChange();
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
    playersReady_ = false;
    foreach (var b in fleets_)
      fleetsReady_[b.Key] = false;
  }

  public void OnReadyClick()
  {
    if (state_ == GameState.END)
      ReadyChange();

    if (gameType_ == GameType.ONLINE)
    {
      if (state_ == GameState.PREPARING)
        netManager_.SendPlayerFleet();
      else if (state_ == GameState.SELECTING)
        netManager_.SendPlayerAttack();
    }
    else
    {
      string plName = playerMng_.GetFleet().Name();
      ReadyCheck(plName, !fleetsReady_[plName]);
    }
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
      case GameState.ATTACKING:
        ChangeState(GameState.SELECTING);
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

  private void StartTimer()
  {
    float start, end;
    float offset;
    if (state_ == GameState.PREPARING)
      offset = 45;
    else if (state_ == GameState.SELECTING)
      offset = 15;
    else
      return;
    start = Time.time;
    end = start + offset;
    timerText_.gameObject.SetActive(true);
    timerText_.SetTime(end - start);
    timerEnd = end;
    timerTime = start;
    timer = true;
  }

  private void OnTimerEnd()
  {
    StopTimer();
    if (netManager_ != null)
    {
      if (state_ == GameState.PREPARING)
        netManager_.SendPlayerFleet();
      else if (state_ == GameState.SELECTING)
        netManager_.SendPlayerAttack();
    }
  }

  public void StopTimer()
  {
    timerText_.SetTime(0);
    timer = false;
  }

  #endregion

  #region FleetsManagement
  public void FleetLost(string fleet)
  {
    fleets_.Remove(fleet);
    if (fleets_.Count == 1 || fleet == playerName)
    {
      Debug.Log("GAME END");
      Debug.Log(fleets_.First().Key + " WINS");
      winText_.OnEnd(fleets_.First().Key);
      resultText_.OnEnd(fleets_.First().Key == playerMng_.GetFleet().Name());

      if (fleets_.First().Key == playerMng_.GetFleet().Name())
        audioManager_.PlayMusic(global::AudioManager.Music.Win, false);
      else
        audioManager_.PlayMusic(global::AudioManager.Music.Loose, false);

      ChangeState(GameState.END);
      ConectionErrorExit = false;
      nextState = false;
      fleetExit.Clear();
      potentialFleets_.Clear();
      attacks_.Clear();
      currentEnemyFleet_ = -1;
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

  public GameObject AddEnemyFleet(string name)
  {
    if (name == playerName)
      return null;
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

  public Fleet RandomEnemyFleet()
  {
    return enemyFleets_.Count == 1 ? enemyFleets_[0] : enemyFleets_[Random.Range(0, enemyFleets_.Count)];
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

  public NetworkManager NetworkManager()
  {
    return netManager_;
  }

  public AudioManager AudioManager()
  {
    return audioManager_;
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

  public void SetAudioManager(AudioManager am)
  {
    if (audioManager_)
    {
      Destroy(am);
      return;
    }
    audioManager_ = am;
    audioManager_.PlayMusic(global::AudioManager.Music.Menu, true);
    DontDestroyOnLoad(am);
  }

  public void SetWaitingText(WaitingText text)
  {
    waitingText_ = text;
  }

  public void SetIdText(IdText id)
  {
    idText_ = id;
    if (gameType_ == GameType.AI)
      id.gameObject.SetActive(false);
  }

  public void SetId(string id)
  {
    if (idText_)
      idText_.SetText(id);
  }

  public void SetTimerText(TimerText text)
  {
    timerText_ = text;
    StopTimer();
  }

  public void PlayersReady()
  {
    playersReady_ = true;
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

  #endregion
}
