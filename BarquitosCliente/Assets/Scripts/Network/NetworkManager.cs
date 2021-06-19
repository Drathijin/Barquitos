using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using server;

public class NetworkManager
{
    private Socket socket_;

    private NetworkData data_;

    string ip, port;

    string id_;

    Thread th_;

    public void Setup(NetworkData setup, string ip, string port)
    {
        Socket.InitSockets();
        data_ = setup;
        this.ip = ip;
        this.port = port;

        th_ = new Thread(SetupThread);
        th_.Start();
        //th_.Join();
    }

    public void SetupThread()
    {
        // MANDAR EL PLAYER AL SERVER O ALGO ----- MIRALO LUEGO

        Debug.Log("Comienza el Setup");

        socket_ = new Socket("83.41.58.21", "8080");

        socket_.Send(data_, socket_);

        // TODO ESTÁ READY PAPA
        GameManager.Instance().PlayersReady();

        //socket_.Recv(data_);
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
    public void ResolveTurn()
    {     // Ejecutar la decisión de ataque tomada en el ManageTurn

    }

    public void OnDestroy()
    {
        if (th_ != null)
            th_.Abort();
        Socket.QuitSockets();
    }
}
