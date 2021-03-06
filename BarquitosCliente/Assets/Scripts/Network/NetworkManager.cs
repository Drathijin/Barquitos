using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.IO;
using UnityEngine;
using server;

public class NetworkManager
{
  public static readonly string path = Application.persistentDataPath + "/lastConection.txt";

  private Socket socket_;

  private NetworkData networkData_;

  string ip, port;

  public System.Guid id_;

  Thread th_;

  public bool conected_ = false;

  public void Setup(NetworkData setup, string ip, string port)
  {
    Socket.InitSockets();
    networkData_ = setup;
    Debug.Log(ip + ":" + port);
    this.ip = ip;
    this.port = port;


    socket_ = new Socket(this.ip, this.port);

    socket_.Send(networkData_, socket_);

    th_ = new Thread(SetupThread);
    th_.Start();
  }

  private void SetupThread()
  {
    AcceptConnection ac = new AcceptConnection(Guid.Empty, "");
    Recieve(ref ac, IMessage.MessageType.AcceptConnection);
    if (ac.name == "\0\0\0\0\0\0\0\0\0\0\0\0")
    {
      lock (GameManager.Instance())
      {
        GameManager.Instance().ErrorMessage = $"Name already in use \n{networkData_.playerName}";
        GameManager.Instance().ConectionErrorExit = true;
      }
    }

    //File.WriteAllText(path, this.ip + "\n" + this.port);

    StreamWriter writer = new StreamWriter(path);

    writer.WriteLine(networkData_.playerName);
    writer.WriteLine(this.ip);
    writer.WriteLine(this.port);
    writer.Close();

    conected_ = true;

    ServerSetup conection = new ServerSetup(System.Guid.Empty, new List<string>());
    Recieve(ref conection, IMessage.MessageType.ServerSetup);

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
    GameManager.Instance().StopTimer();

    GameManager.Instance().PlayerManager().CheckFleet();
    Fleet fl = GameManager.Instance().PlayerManager().GetFleet();

    ClientSetup setup = new ClientSetup(id_, fl.ships, fl.Name());

    socket_.Send(setup, socket_);
  }

  public void SendPlayerAttack()
  {
    GameManager.Instance().StopTimer();
    PlayerManager player = GameManager.Instance().PlayerManager();
    GridObject obj = player.currentAttackButton_;
    player.CleanButton();

    AttackData data = obj ? new AttackData(obj.Data().GetX(), obj.Data().GetY(), obj.Fleet().Name(), GameManager.Instance().playerName) :
      RandomAttack();
    //new AttackData(-1, -1, "", GameManager.Instance().playerName);

    Debug.Log("AttackSend: " + data.x + " " + data.y + " " + data.enemyId);
    data.header_.gameID_ = id_;
    socket_.Send(data, socket_);
  }

  private AttackData RandomAttack()
  {
    int x = UnityEngine.Random.Range(0, 10);
    int y = UnityEngine.Random.Range(0, 10);
    string enemy = GameManager.Instance().RandomEnemyFleet().Name();

    return new AttackData(x, y, enemy, GameManager.Instance().playerName);
  }

  private void WaitForReady()    // Escuchar al servidor para saber cuando le han dado a listo
  {
    Debug.Log("Waiting for ready");
    ReadyGame ready = new ReadyGame(id_);
    Recieve(ref ready, IMessage.MessageType.ReadyTurn);
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

    Recieve(ref serverAttack, IMessage.MessageType.ServerAttack);

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
      if (msg.header_.messageType_ != expectedType)
      {
        UnexpectedMessage(msg, expectedType);
      }
    }
    catch (Exception e)
    {
      Debug.LogError(e.Message);
      if (e.Message == "Unreacheable host")
      {
        lock (GameManager.lock_)
        {
          GameManager.Instance().ErrorMessage = $"Conection unreacheable ({ip}:{port})";
          GameManager.Instance().ConectionErrorExit = true;
        }
      }
      else if (msg.header_.messageType_ != expectedType)
      {
        UnexpectedMessage(msg, expectedType);
      }
      else
        throw e;
    }
  }

  private void UnexpectedMessage(IMessage msg, IMessage.MessageType expectedMessage)
  {
    if (msg.header_.messageType_ == IMessage.MessageType.ClientExit)
    {
      lock (GameManager.lock_)
      {
        ClientExit exit = new ClientExit(id_, "");
        exit.FromBin(msg.GetData());
        //GameManager.Instance().potentialFleets_.Add(exit.name);
        GameManager.Instance().fleetExit.Enqueue(exit.name);
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
    if (conected_)
    {
      ClientExit exit = new ClientExit(id_, GameManager.Instance().playerName);
      socket_.Send(exit, socket_);
    }
    socket_.Dispose();
    Socket.QuitSockets();
  }
}
