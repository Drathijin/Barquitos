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
			name_=name;
			socket_=s;
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
							if(p.x == x && p.y == y)
								{
									hit = true;
									ret = ship.CheckAttack(p.x,p.y); 
									break;
								}
					}
					if(hit)
						break;
			}
			if(ret) //Checked ship sunk
			{
				//Player lost until we find a ship alive
				dead = true; 
				foreach (BattleShip ship in ships_)
					if(!ship.Destroyed())
						dead = false;
			}
			return ret;
		}
	}


	public class Game {
		
		public static Socket socket_;
		public static object socket_lock;
		List<Player> players_;
		System.Guid id_;
		int secondsToStart_ = 60;
		int secondsForNextRound_ = 30;


		public Game(int playerCount,Socket socket, object sck_lock, List<Player> players, System.Guid id)
		{
			players_ = players;
			socket_ = socket;
			id_=id;
			socket_lock = sck_lock;
			
			//Notify all players that the game started
			lock(socket_lock)
			{
				List<string> names = new List<string>();
			
				foreach (Player player in players_)
				{
					foreach (Player p in players_)
					{
						if(p.name_ != player.name_)
							names.Add(p.name_);
					}
					Console.WriteLine("Sending ServerConection to player");
					socket.Send(new ServerSetup(id,names), player.socket_);
					names.Clear();
				}
			}
		}

		public void StartGame()
		{
			while(secondsToStart_ > 0)
			{
				secondsToStart_--;
				Thread.Sleep(1000); //Esperamos 60 segundos o hasta que estén todas las posiciones
			}
			SetAllPositions();
		}

		public void SetPlayerPositions(/*ClientPositions*/)
		{
			string name = "";//GetPlayerName
			bool allReady = true;
			foreach(Player p in players_)
			{
				if(p.name_ == name)
				{
					//setPlayerPositions to ClientPositionsParam
					p.ready = true;
				}
				if(!p.ready)
					allReady = false;
			}
			if(allReady)
				secondsToStart_ = 0;
		}

		public void Play(){
			bool playing = true;
			while(playing)
			{
				while(secondsForNextRound_ > 0)
				{
					secondsForNextRound_--;
					Thread.Sleep(1);
				}
				playing = ResolveRound();
			}
		}

		public void SetAllPositions()
		{
			/*send ServerSetup to all players*/
			Console.WriteLine("Setting all positions");
		}

		//returns whether or not the game has ended
		public bool ResolveRound()
		{
			int aliveCount = 0;
			foreach(Player a in players_)
			{
				bool hit = false;
				foreach(Player p in players_)
				{
					if(a.targetName_ == p.name_)
					{
						if(p.attack(a.taretAttack_.x, a.taretAttack_.y,out hit) && !p.dead)
							aliveCount++;
					}
				}
				foreach(Player p in players_)
				{
					//Send ServerAttack(a.targetName, a.targetPosition, hit) to p.socket;					
				}
			}
			if(aliveCount >= 2)
			{
				secondsForNextRound_ = 30;
				return true;
			}
			else
				return false;
		}
	}
}