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

    [SerializeField]
    private GameObject buttonsPrefabs_;
    [SerializeField]
    private Transform enemyWater_;

    private GameState state_;

    private PlayerManager playerMng_;

    private NetworkManager netManager_;

    private AIManager aiManager_;

    private Dictionary<string, EnemyFleet> fleets_;

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

    private void Start()
    {
        fleets_ = new Dictionary<string, EnemyFleet>();
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
    }

    public void SetPlayerManager(PlayerManager mng)
    {
        playerMng_ = mng;
    }

    public void AddEnemyFleet(string id, bool ai)
    {
        Debug.Log("AAAAAAAAAAHHHHHH");

        GameObject g = Instantiate(buttonsPrefabs_, enemyWater_);

        EnemyFleet fleet = g.AddComponent<EnemyFleet>();
        fleets_[id] = fleet;
        currentEnemyFleet_ = fleet;

				//Asumimos or ahora easyAI
				if(ai)
				{
					EasyBehaviour eb = g.AddComponent<EasyBehaviour>();
					eb.Setup(fleet);
					if(aiManager_)
						aiManager_.addBehaviour(id, eb);
				}

    }

		public EnemyFleet GetFleet(string id)
		{
			return fleets_[id];
		}
		public List<string> GetPlayerList()
		{
			List<string> list = fleets_.Keys;
			return list;
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

    public EnemyFleet CurrentEnemyFleet()
    {
        return currentEnemyFleet_;
    }

    public PlayerManager PlayerManager()
    {
        return playerMng_;
    }
}
