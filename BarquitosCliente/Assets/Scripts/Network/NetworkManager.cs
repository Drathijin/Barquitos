using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using server;

public class NetworkManager
{
  private Socket socket_;

  private NetworkData networkData_;

  string ip, port;

  public System.Guid id_;

  Thread th_;

private bool conected_ = false;

  public bool Setup(NetworkData setup, string ip, string port)
  {
    Socket.InitSockets();
    networkData_ = setup;
    this.ip = ip;
    this.port = port;

    try
    {
      socket_ = new Socket("83.41.58.21", "8080");
    }
    catch (Exception e)
    {
      Debug.LogError(e.Message);
      lock (GameManager.lock_)
      {
        GameManager.Instance().ConectionErrorExit = true;
      }
    }

    socket_.Send(networkData_, socket_);

    AcceptConnection ac = new AcceptConnection(Guid.Empty, "");
    socket_.Recv(ac);
    if(ac.name == "\0\0\0\0\0\0\0\0\0\0\0\0")
      return false;

    conected_ = true;
    th_ = new Thread(SetupThread);
    th_.Start();
    return true;
  }

  private void SetupThread()
  {
    ServerSetup conection = new ServerSetup(System.Guid.Empty, new List<string>());
    socket_.Recv(conection);



    // TODO EST� READY PAPA
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
    if (th_ != null)
      th_.Join();
    th_ = null;
    switch (state)
    {
      case GameManager.GameState.PREPARING:
        th_ = new Thread(WaitForReady);
        break;
      case GameManager.GameState.SELECTING:
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

    ClientSetup setup = new ClientSetup(id_, fl.ships, fl.Name());

    socket_.Send(setup, socket_);
  }

  public void SendPlayerAttack()
  {
    PlayerManager player = GameManager.Instance().PlayerManager();
    GridObject obj = player.currentAttackButton_;
    player.CleanButton();

    AttackData data = obj ? new AttackData(obj.Data().GetX(), obj.Data().GetY(), obj.Fleet().Name(), GameManager.Instance().playerName) :
            new AttackData(-1, -1, "", GameManager.Instance().playerName);

    data.header_.gameID_ = id_;
    socket_.Send(data, socket_);
  }

  private void WaitForReady()    // Escuchar al servidor para saber cuando le han dado a listo
  {
    Debug.Log("Waiting for ready");
    ReadyGame ready = new ReadyGame(id_);
    Recieve<ReadyGame>(ref ready, IMessage.MessageType.ReadyTurn);
    Debug.Log("We ready");
    lock (GameManager.lock_)
    {
      GameManager.Instance().nextState = true;
    }
  }

  private void ResolveTurn()   // Ejecutar la decisi�n de ataque tomada en el ManageTurn
  {
    Debug.Log("Resolving Turn");
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
      GameManager.Instance().nextState = true;
    }
  }

  private void Recieve<T>(ref T msg, IMessage.MessageType expectedType) where T : IMessage
  {
    try
    {
      socket_.Recv(msg);

      if(msg.header_.messageType_ != expectedType)
      {
        UnexpectedMessage(msg, expectedType);
      }
    }
    catch (Exception e)
    {
      Debug.LogError(e.Message);
      lock (GameManager.lock_)
      {
        GameManager.Instance().ConectionErrorExit = true;
      }
    }
  }

  private void UnexpectedMessage(IMessage msg, IMessage.MessageType expectedMessage)
  {
    if(msg.header_.messageType_ ==  IMessage.MessageType.ClientExit)
    {
      lock(GameManager.lock_)
      {
        GameManager.Instance().potentialFleets_.Add((msg as ClientExit).name);
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
    if(conected_)
    {
      ClientExit exit = new ClientExit(id_, GameManager.Instance().playerName);
      socket_.Send(exit, socket_);
    }
    socket_.Dispose();
    Socket.QuitSockets();
  }
}
