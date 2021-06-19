using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;

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
		
		//Adds a single conection to the correct queue
		public static void ManageConection(NetworkData conectionMessage, Socket socket) //this should be casted to conectionMessage
		{
			if(conectionMessage.battleRoyale)
			{
				lock(br_player_lock){
					BattleRoyalePlayers_.Enqueue(new Player(conectionMessage.playerName,socket));
				}
			}
			else
			{
				lock(player_lock){
					players_.Enqueue(new Player(conectionMessage.playerName,socket));
				}
			}
		}

		//Every 5 seconds tries to create games with the current amount of players in queue and spawns a thread with the corresponding game
		public static void HandleQueues()
		{
			lock(player_lock)
			{
				while(players_.Count >= 2)
				{
					System.Guid id = System.Guid.NewGuid();
					Player player1 = players_.Dequeue();
					Player player2 = players_.Dequeue();
					List<Player> pList = new List<Player>();
					pList.Add(player1);
					pList.Add(player2);

					gameThreads_.Add(id, new Thread(() => ManageGame(id,2,pList)));
					gameThreads_[id].Start();
				}
			}
			lock(br_player_lock)
			{

			}
			Thread.Sleep(5);
		}

		public static void ManageGame(System.Guid id, int playerCount, List<Player> pList)
		{
			Game game = new Game(playerCount,socket_,socket_lock,pList,id);
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
		}

		public static void Server()
		{
			Init();
			IMessage message = new IMessage(0,System.Guid.Empty);
			Socket other;
			while(true)
			{
				lock(socket_lock)
				{
					socket_.Recv(message, out other);
				}
				switch (message.header_.messageType_)
				{
						case IMessage.MessageType.ClientConection:
							NetworkData data = ((NetworkData)message);
							data.FromBin(message.GetData());
							ManageConection(data, other);
							break;
						default:
							break;
				}
			}
			
		}

		static void Main(string[] args)
		{
			Socket.InitSockets();
			if(args.Length == 2)
			{
				IP = args[0];
				PORT = args[1];
				Server();
			}
			Socket.QuitSockets();
		}
	} 
}
