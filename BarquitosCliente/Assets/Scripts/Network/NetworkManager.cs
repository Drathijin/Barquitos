using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using server;

public class NetworkManager : MonoBehaviour
{
    private Socket socket_;

    private void Awake()
    {
        Socket.InitSockets();
    }

    public void Setup(NetworkData setup, string ip , string port)
    {
        // MANDAR EL PLAYER AL SERVER O ALGO ----- MIRALO LUEGO

        socket_ = new Socket(ip, port);

        socket_.Send(setup, socket_);

        //socket_.Recv();

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

    private void OnDestroy()
    {
        Socket.QuitSockets();
    }
}
