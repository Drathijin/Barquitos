using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{


    [SerializeField]
    private Dictionary<string, IABehaviour> behaviours_;
    private Dictionary<string, AttackData> nextAttacks_;

    public void Setup(int nFleets)
    {
        behaviours_ = new Dictionary<string, IABehaviour>();
        nextAttacks_ = new Dictionary<string, AttackData>();
        GameManager.Instance().SetAIManager(this);

        for (int i = 0; i < nFleets; i++)
            GameManager.Instance().AddEnemyFleet("AIFleet" + i, true);
    }

    void SetupFleet()
    {
			foreach (var item in behaviours_)
			{
				Debug.LogError(item.Key);
				item.Value.Setup(GameManager.Instance().GetFleet(item.Key));
			}
    }

    public void addBehaviour(string id, IABehaviour ia)
    {
			behaviours_[id] = ia;
    }

    public void OnStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.PREPARING:
								SetupFleet();
                break;
            case GameManager.GameState.SELECTING:
                ManageTurn();
                break;
            case GameManager.GameState.ATTACKING:
                ResolveTurn();
                break;
            default:
                break;
        }
    }

    public void ManageTurn()    // Tomar la decision de ataque en el turno y guardarla para el ResolveTurn
    {
				Debug.Log("ManageTurn");
        nextAttacks_.Clear();
        foreach (var item in behaviours_)
        {
						Debug.Log(item.Key);
            nextAttacks_[item.Key] = item.Value.Attack();
						var fleet = GameManager.Instance().GetFleet(item.Key);
						if(fleet.IsDestroyed())
							GameManager.Instance().FleetLost(item.Key);
        }
    }
    public void ResolveTurn()   // Ejecutar la decisi�n de ataque tomada en el ManageTurn
    {
				Debug.Log("ResolveTurn");
        var list = GameManager.Instance().GetPlayerList();
        foreach (var attack in nextAttacks_)
        {
            Debug.Log("Attacking " + attack.Value.enemyId + " at x: " + attack.Value.x + " y: " + attack.Value.y);
            Fleet fl = GameManager.Instance().GetFleet(attack.Value.enemyId);
            int x = attack.Value.x;
            int y = attack.Value.y;
            fl.Attack(x, y);
        }
    }
}
