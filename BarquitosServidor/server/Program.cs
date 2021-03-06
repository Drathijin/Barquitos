using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace server
{
  class Program
  {
    public static string IP;
    public static string PORT;
    public static Socket socket_;
    public static object socket_lock;

    public static Dictionary<System.Guid, Game> games_;
    public static Dictionary<System.Guid, Thread> gameThreads_;

    public static object player_lock;
    public static Queue<Player> players_;

    public static object br_player_lock;
    public static Queue<Player> BattleRoyalePlayers_;

    public static Thread handler;

  
    //Juan
    public static bool CheckName(string actual)
    {
      if(actual == "\0\0\0\0\0\0\0\0\0\0\0\0")
        return false;
      List<string> names = new List<string>();
      lock (br_player_lock)
      {
        foreach (Player p in BattleRoyalePlayers_)
          names.Add(p.name_);
        lock(player_lock)
        {
          foreach (Player p in players_)
            names.Add(p.name_);
          foreach (string name in names)
          {
            if(name == actual)
              return false;
          }
          return true;
        }
      }
    }

    //Adds a single conection to the correct queue
    public static void ManageConection(NetworkData conectionMessage, Socket socket) //this should be casted to conectionMessage
    {
      Console.WriteLine($"Player: {conectionMessage.playerName} Queueing for ${conectionMessage.battleRoyale}");
      if (conectionMessage.battleRoyale)
      {
        lock (br_player_lock)
        {
          BattleRoyalePlayers_.Enqueue(new Player(conectionMessage.playerName, socket));
          if(BattleRoyalePlayers_.Count >= 5)
            handler.Resume();
        }
      }
      else
      {
        lock (player_lock)
        {
          players_.Enqueue(new Player(conectionMessage.playerName, socket));
        }
      }
    }

    //Every 5 seconds tries to create games with the current amount of players in queue and spawns a thread with the corresponding game
    public static void HandleQueues()
    {
      while (true)
      {
        lock (player_lock)
        {
          while (players_.Count >= 2)
          {
            System.Guid id = System.Guid.NewGuid();
            Player player1 = players_.Dequeue();
            Player player2 = players_.Dequeue();
            List<Player> pList = new List<Player>();
            pList.Add(player1);
            pList.Add(player2);
            Console.WriteLine($"Starting game with {player1.name_} and {player2.name_}. ID = {id.ToString()}");
            gameThreads_.Add(id, new Thread(() => ManageGame(id, 2, pList)));
            gameThreads_[id].Start();
          }
        }
        lock (br_player_lock)
        {
          while (BattleRoyalePlayers_.Count >= 5)
          {
            System.Guid id = System.Guid.NewGuid();
            List<Player> pList = new List<Player>();
            for (int i = 0; i < 5; i++)
            {
              pList.Add(BattleRoyalePlayers_.Dequeue());
            }
            gameThreads_.Add(id, new Thread(() => ManageGame(id, 5, pList)));
            gameThreads_[id].Start();
          }
        }
        foreach (var pair in games_)
        {
          if (pair.Value.finished)
          {
            games_.Remove(pair.Key);
            gameThreads_.Remove(pair.Key);
          }
        }
        Thread.Sleep(5000);
      }
    }

    public static void ManageGame(System.Guid id, int playerCount, List<Player> pList)
    {
      Game game = new Game(playerCount, socket_, socket_lock, pList, id);
      games_.Add(id, game);
      game.StartGame();
    }

    public static void Init()
    {
      games_ = new Dictionary<Guid, Game>();
      gameThreads_ = new Dictionary<Guid, Thread>();
      players_ = new Queue<Player>();
      BattleRoyalePlayers_ = new Queue<Player>();
      player_lock = new Object();
      br_player_lock = new Object();
      socket_lock = new Object();

      handler = new Thread(HandleQueues);
      handler.Start();


      socket_ = new Socket(IP, PORT);
      socket_.Bind();

      Console.WriteLine($"Initialized server on {IP}:{PORT} successfully");
    }

    public static T ProcessClientMessage<T>(IMessage message, T ClientMessage) where T : IMessage
    {
      ClientMessage.FromBin(message.GetData());
      if(games_.ContainsKey(ClientMessage.header_.gameID_))
      {
        Game game = games_[ClientMessage.header_.gameID_];
        lock (game.messages_lock_)
        {
          game.messages_.Add(ClientMessage);
          return ClientMessage;
        }
      }
      else
      {
        if(message.header_.messageType_ != IMessage.MessageType.ClientExit)
          Console.WriteLine($"[Error]: Invalid game id - {ClientMessage.header_.gameID_}");
        return ClientMessage;
      }
    }

    public static void Server()
    {
      Init();
      IMessage message = new IMessage(0, System.Guid.Empty);
      Socket other;
      while (true)
      {
        socket_.Recv(message, out other);
        switch (message.header_.messageType_)
        {
          case IMessage.MessageType.ClientConection:
            NetworkData data = new NetworkData();
            data.FromBin(message.GetData());
            if(CheckName(data.playerName))
            {
              Console.WriteLine("Aceptando conexión");
              socket_.Send(new AcceptConnection(Guid.Empty, data.playerName), other);
              ManageConection(data, other);
            }
            else
            {
              Console.WriteLine("Nombre repetido");
              socket_.Send(new AcceptConnection(Guid.Empty, ""), other);
            }
            break;
          case IMessage.MessageType.ClientSetup:
            Console.WriteLine("Client Setup");
            ProcessClientMessage<ClientSetup>(message, new ClientSetup(Guid.Empty, new List<BattleShip>(), ""));
            break;
          case IMessage.MessageType.ClientAttack:
            Console.WriteLine("Client Attack");
            ProcessClientMessage<AttackData>(message, new AttackData());
            break;
          case IMessage.MessageType.ClientExit:
          {
            Console.WriteLine("Client Exit");
            var current = ProcessClientMessage<ClientExit>(message, new ClientExit(Guid.Empty,""));
            if(message.header_.gameID_ == Guid.Empty)
            {
              lock(player_lock){
                players_ = new Queue<Player>(players_.Where(x => x.name_ != current.name));
              }
              lock(br_player_lock)
              {
                BattleRoyalePlayers_ = new Queue<Player>(BattleRoyalePlayers_.Where(x => x.name_ != current.name));
              }
            }
            break;
          }
          default:
            break;
        }
      }

    }

    static void Main(string[] args)
    {
      Socket.InitSockets();
      if (args.Length == 2)
      {
        IP = args[0];
        PORT = args[1];
        Server();
      }
      Socket.QuitSockets();
    }
  }
}
