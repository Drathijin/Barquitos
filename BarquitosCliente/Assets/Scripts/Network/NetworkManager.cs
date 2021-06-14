using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance().SetNetworkManager(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStateChanged(GameManager.GameState state)
    {
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
            default:
                break;
        }
    }

    public void ManageTurn()    // Escuchar al servidor para saber cuando le han dado a listo
    {

    }
    public void ResolveTurn() {     // Ejecutar la decisión de ataque tomada en el ManageTurn
        
    }                
}
