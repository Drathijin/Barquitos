using server;
using System.Collections;
using System.Collections.Generic;
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
		List<Player> players_;
		System.Guid id;
		int secondsToStart_ = 60;
		int secondsForNextRound_ = 30;
		public Game(int playerCount,Socket socket, List<Player> players, System.Guid id)
		{
			players_ = players;
			socket_ = socket;
			//Notify all players that the game started
			foreach (Player player in players_)
			{
				// socket.Send(new ServerConection(id),player.socket);
			}

			while(secondsToStart_ > 0)
			{
				secondsToStart_--;
				Thread.Sleep(1); //Esperamos 30 segundos o hasta que estén todas las posiciones
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