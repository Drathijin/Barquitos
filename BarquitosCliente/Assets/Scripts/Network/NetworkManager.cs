using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public void Setup(NetworkData setup, string ip , string port)
    {
        // MANDAR EL PLAYER AL SERVER O ALGO ----- MIRALO LUEGO


        // TODO ESTÁ READY PAPA
        GameManager.Instance().PlayersReady();
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
    
    public void SendPlayer()
    {
        string name = GameManager.Instance().PlayerManager().GetFleet().Name();
    }
}
