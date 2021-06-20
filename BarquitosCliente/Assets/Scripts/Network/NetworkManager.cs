using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using server;

public class NetworkManager
{
    private Socket socket_;

    private NetworkData networkData_;

    string ip, port;

    public System.Guid id_;

    Thread th_;

    public void Setup(NetworkData setup, string ip, string port)
    {
        Socket.InitSockets();
        networkData_ = setup;
        this.ip = ip;
        this.port = port;

        th_ = new Thread(SetupThread);
        th_.Start();
        //th_.Join();
    }

    private void SetupThread()
    {
        // MANDAR EL PLAYER AL SERVER O ALGO ----- MIRALO LUEGO

        Debug.Log("Comienza el Setup");

        socket_ = new Socket("83.41.58.21", "8080");

        var g = System.Guid.NewGuid();

        Debug.Log(g.ToString());
        networkData_.header_.gameID_ = g;
        socket_.Send(networkData_, socket_);

        ServerSetup conection = new ServerSetup(System.Guid.Empty, new List<string>());

        socket_.Recv(conection);

        // TODO ESTÁ READY PAPA

        lock (GameManager.lock_)
        {
            for (int i = 0; i < conection.names_.Count; i++)
            {
                Debug.Log("Name: " + conection.names_[i]);

                GameManager.Instance().potentialFleets_.Add(conection.names_[i]);

            }
            Debug.Log("Id " + conection.header_.gameID_.ToString());
            GameManager.Instance().PlayersReady();

            id_ = conection.header_.gameID_;

            GameManager.Instance().id_ = id_.ToString();
        }
    }

    public void OnStateChanged(GameManager.GameState state)
    {
        th_ = null;
        switch (state)
        {
            case GameManager.GameState.PREPARING:
            case GameManager.GameState.SELECTING:
                th_ = new Thread(WaitForReady);
                break;
            case GameManager.GameState.ATTACKING:
                th_ = new Thread(ResolveTurn);
                break;
            default:
                break;
        }
        if (th_ != null)
            th_.Start();
    }

    public void SendPlayerFleet()
    {
        Fleet fl = GameManager.Instance().PlayerManager().GetFleet();

        ClientSetup setup = new ClientSetup(id_, fl.ships);

        socket_.Send(setup, socket_);
    }

    public void SendPlayerAttack(AttackData data)
    {
        data.header_.gameID_ = id_;

        socket_.Send(data, socket_);
    }

    private void WaitForReady()    // Escuchar al servidor para saber cuando le han dado a listo
    {
        ReadyTurn ready = new ReadyTurn(id_);
        socket_.Recv(ready);

        lock (GameManager.lock_)
        {
            GameManager.Instance().nextState = true;
        }
    }

    private void ResolveTurn()   // Ejecutar la decisión de ataque tomada en el ManageTurn
    {
        ServerAttack serverAttack = new ServerAttack(id_);

        socket_.Recv(serverAttack);

        lock (GameManager.lock_)
        {
            Debug.Log("-----ATTACKS------");
            foreach (ServerAttack.AttackResult res in serverAttack.attacks_)
            {
                Debug.Log("Attack to: " + res.name + " in x: " + res.x + " y: " + res.y + " --Result:" + res.hit);
                GameManager.Instance().attacks_.Add(res);
            }
        }
    }

    public void OnDestroy()
    {
        if (th_ != null)
        {
            Debug.Log("Cerrando Thread");
            th_.Abort();
        }
        Socket.QuitSockets();
    }
}
