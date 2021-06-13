using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

		
		[SerializeField]
		private Dictionary<string,IABehaviour> behaviours_;
		private Dictionary<string,AttackData> nextAttacks_;
    
		void Start()
    {
        behaviours_ = new Dictionary<string, IABehaviour>();
        nextAttacks_ = new Dictionary<string, AttackData>();
        GameManager.Instance().SetAIManager(this);
    }

		void SetupFleet()
		{

		}

		public void addBehaviour(string id, IABehaviour ia)
		{
			behaviours_[id] = ia;
		}

    public void ManageTurn()    // Tomar la decision de ataque en el turno y guardarla para el ResolveTurn
    {
        nextAttacks_.Clear();
				foreach (var item in behaviours_)
				{
					nextAttacks_[item.Key] = item.Value.Attack();
				}
    }
    public void ResolveTurn()   // Ejecutar la decisi�n de ataque tomada en el ManageTurn
    {
			var list = GameManager.Instance().GetPlayerList();
			foreach (var item in nextAttacks_)
			{
				
			}
    }
}
