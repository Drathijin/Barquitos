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

		public static Dictionary<System.Guid, Game> games_;
		public static Dictionary<System.Guid, Thread> gameThreads_;

		public static object player_lock;
		public static Queue<Player> players_;
		public static object br_player_lock;
		public static Queue<Player> BattleRoyalePlayers_;

		public static Thread handler;
		
		//Adds a single conection to the correct queue
		public static void ManageConection(IMessage conectionMessage, Socket socket) //this should be casted to conectionMessage
		{
			if(conectionMessage/*.isBattleRoyale()*/!=null)
			{
				lock(br_player_lock){
					BattleRoyalePlayers_.Enqueue(new Player("conectionMessage.name",socket));
				}
			}
			else
			{
				lock(player_lock){
					players_.Enqueue(new Player("conectionMessage.name",socket));
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

					games_.Add(id, new Game(2,socket_,pList,id));
				}
			}
			lock(br_player_lock)
			{

			}
			Thread.Sleep(5);
		}

		public static void ManageGames()
		{

		}

		public static System.Guid Match()
		{
			// Console.WriteLine($"This simulates a game between {game_.p1Name} and {game_.p2Name}");
			var uuid = System.Guid.NewGuid();
			return uuid;
		}

		public static void Server()
		{
			handler = new Thread(HandleQueues);
			socket_ = new Socket(IP, PORT);
			socket_.Bind();
			NetworkData conection = new NetworkData();
			IMessage message = new IMessage(0,System.Guid.Empty);
			while(true)
			{
				socket_.Recv(message);
				switch (message.header_.messageType_)
				{
						case IMessage.MessageType.ClientConection:
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
			
			
			// var id = Match();
			// Console.WriteLine($"ID:{id.ToString()} with size of: {id.ToByteArray().Length} bytes");
		}
	} 
}
