using server;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace server
{
  public class Player
  {
    public Socket socket_;
    public string name_;
    public List<BattleShip> ships_;

    public bool ready = false;
    public bool dead = false;
    public BattleShip.Position taretAttack_;
    public string targetName_;
    public Player(string name, Socket s)
    {
      name_ = name;
      socket_ = s;
    }

    //returns wether the ship has been sunk or not
    public bool attack(int x, int y, out bool hit)
    {
      bool ret = false;
      hit = false;
      foreach (BattleShip ship in ships_)
      {
        List<BattleShip.Position> positions = ship.PlacedPositions();
        foreach (BattleShip.Position p in positions)
        {
          if (p.x == x && p.y == y)
          {
            hit = true;
            ret = ship.CheckAttack(p.x, p.y);
            break;
          }
        }
        if (hit)
          break;
      }
      if (ret) //Checked ship sunk
      {
        //Player lost until we find a ship alive
        dead = true;
        foreach (BattleShip ship in ships_)
          if (!ship.Destroyed())
            dead = false;
      }
      return ret;
    }
  }


  public class Game
  {

    public Socket socket_;
    public object socket_lock;

    public object messages_lock_;
    public List<IMessage> messages_;

    List<Player> players_;
    Player winningPlayer_ = null;
    System.Guid id_;
    int secondsToStart_ = 120;
    int secondsForNextRound_ = 300;
    bool playing_ = false; //this bool will be true only in the second fase of the game
    public bool finished = false;
    public Game(int playerCount, Socket socket, object sck_lock, List<Player> players, System.Guid id)
    {
      messages_ = new List<IMessage>();
      messages_lock_ = new Object();

      players_ = players;
      socket_ = socket;
      id_ = id;
      socket_lock = sck_lock;

      //Notify all players that the game started
      lock (socket_lock)
      {
        List<string> names = new List<string>();

        foreach (Player player in players_)
        {
          foreach (Player p in players_)
          {
            if (p.name_ != player.name_)
              names.Add(p.name_);
          }
          Console.WriteLine($"Sending ServerConection to player | ID of the game is: ${id}");
          socket.Send(new ServerSetup(id, names), player.socket_);
          names.Clear();
        }
      }
    }

    public void StartGame()
    {
      while (secondsToStart_ > 0)
      {
        CheckMessages();
        secondsToStart_--;
        Thread.Sleep(1000); //Esperamos 60 segundos o hasta que estén todas las posiciones
      }
      Play();
    }

    public void SetPlayerPositions(ClientSetup setup)
    {
      string name = setup.name_;
      bool allReady = true;
      foreach (Player p in players_)
      {
        if (p.name_ == name)
        {
          //setPlayerPositions to ClientPositionsParam
          Console.WriteLine($"Setting up positions for {name}");
          p.ships_ = setup.GetBattleShips();
          foreach (var s in p.ships_)
          {
            Console.WriteLine($"Ship of size {s.GetSize()} is on position [{s.PlacedPositions()[0].x},{s.PlacedPositions()[0].y}] with {(s.horizontal ? "horizontal" : "vertical")} direction");
          }
          p.ready = true;
        }
        if (!p.ready)
          allReady = false;
      }
      if (allReady)
        secondsToStart_ = 0;
    }

    public void Play()
    {
      //Notify change state
      ReadyGame rt = new ReadyGame(id_);
      lock (socket_lock)
      {
        Console.WriteLine("Sending ready message to all players");
        foreach (Player p in players_)
          socket_.Send(rt, p.socket_);
      }

      //Start playing
      playing_ = true;
      while (playing_)
      {
        foreach (Player p in players_)
          p.ready = false;
        while (secondsForNextRound_ > 0)
        {
          CheckMessages();
          secondsForNextRound_--;
          Thread.Sleep(1000);
        }
        playing_ = ResolveRound();
      }
      finished = true;
    }

    public void CheckMessages()
    {
      lock (messages_lock_)
      {
        while (messages_.Count > 0)
        {
          IMessage current = messages_[0];
          messages_.RemoveAt(0);
          switch (current.header_.messageType_)
          {
            case IMessage.MessageType.ClientSetup:
              ClientSetup cs = current as ClientSetup;
              SetPlayerPositions(cs);
              break;
            case IMessage.MessageType.ClientAttack:
              if (playing_)
              {
                AttackData attack = current as AttackData;
                Console.WriteLine($"Attack comming from {attack.myId} to {attack.enemyId} in [{attack.x},{attack.y}]");
                bool allReady = true;
                foreach (Player p in players_)
                {
                  if (p.name_ == attack.myId)
                  {
                    p.targetName_ = attack.enemyId;
                    p.taretAttack_ = new BattleShip.Position(attack.x, attack.y);
                    p.ready = true;
                  }
                }
                foreach (Player p in players_)
                  if (allReady) allReady = p.ready;
                if (allReady)
                  secondsForNextRound_ = 0;
              }
              break;
            case IMessage.MessageType.ClientExit:
              ClientExit ce = current as ClientExit;
              foreach (Player p in players_)
              {
                if (p.name_ != ce.name)
                {
                  socket_.Send(ce,p.socket_);
                }
              }
              players_.RemoveAll(x => x.name_ == ce.name);
              break;
          }
        }
      }
    }

    //returns whether or not the game has ended
    public bool ResolveRound()
    {
      int aliveCount = players_.Count;
      ServerAttack sa = new ServerAttack(id_);

      foreach (Player a in players_)
      {
        //If we don't actually have a target, skip the attack
        if (a.targetName_ == null || a.targetName_ == "\0\0\0\0\0\0\0\0\0\0\0\0")
          continue;

        bool hit = false;
        bool dead = false;
        foreach (Player p in players_)
        {
          if (a.targetName_ == p.name_)
          {
            if (p.attack(a.taretAttack_.x, a.taretAttack_.y, out hit) || p.dead)
            {
              aliveCount--;
              dead = p.dead;
            };
          }
        }
        sa.attacks_.Add(new ServerAttack.AttackResult(hit, a.taretAttack_.x, a.taretAttack_.y, a.targetName_, dead));
      }
      lock (socket_lock)
      {
        foreach (Player p in players_)
          socket_.Send(sa, p.socket_);
      }

      players_.RemoveAll(x => x.dead);

      if (aliveCount >= 2)
      {
        secondsForNextRound_ = 300000;
        return true;
      }
      else
      {
        foreach (Player p in players_)
          if (!p.dead)
            winningPlayer_ = p;
        return false;
      }
    }
  }
}