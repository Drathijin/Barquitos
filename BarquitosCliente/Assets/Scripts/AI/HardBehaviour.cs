using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HardBehaviour : IABehaviour
{
	// int[,] positions_;
	int[,] probabilites_;
	List<int> sizes_ = new List<int>();
	Vector2Int hit_;
	Vector2Int[] directions_;
	List<Vector2Int> possibleBoats_;
	bool[,] hitHistory_;
	bool[,] tries_;

	private void Start() {
		probabilites_ = new int[10,10];

		sizes_.Add(2);
		sizes_.Add(3);
		sizes_.Add(3);
		sizes_.Add(4);
		sizes_.Add(5);

		directions_ = new Vector2Int[4];
		directions_[0] = new Vector2Int(1,0);
		directions_[1] = new Vector2Int(-1,0);
		directions_[2] = new Vector2Int(0,-1);
		directions_[3] = new Vector2Int(0,1);
		
		possibleBoats_ = new List<Vector2Int>();
		
		hitHistory_ = new bool[10,10];
		tries_ = new bool[10,10];
		
		for(int i = 0; i<10;i++)
			for(int j = 0; j<10;j++)
			{
				hitHistory_[i,j] = false;
				tries_[i,j] = false;
			}
		
		CalculateProbabilities();
	}

	private bool checkHit(int x = -1, int y = -1)
	{
		if(x == -1 && y == -1)
		{
			x=hit_.x;
			y=hit_.y;
		}
		if(fleet_.GetGrid().GetPos(x,y).Data().State() == CellData.CellState.HIT)
			hitHistory_[x, y] = true;
		return fleet_.GetGrid().GetPos(x,y).Data().State() == CellData.CellState.HIT;
	}
	//Saves board positions in a checkerboard pattern
	
	private bool checkKnown(int x,int y)
	{
		return !tries_[x,y] || (checkHit(x,y));
	}
	
	private void CalculateProbabilities()
	{
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				probabilites_[j,i]=0;
			}
		}
		
		foreach (int size in sizes_)
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					int s = size;
					bool h = true,v=true;

					//Check horizontaly
					for (int k = 0; h && k < s; k++)
					{
						if(j+k>=10)
							h = false;
						else
							h = checkKnown(i,j+k);
					}
					//If it fits, increment
					for (int k = 0; h && k < s; k++)
						probabilites_[i,j+k] = probabilites_[i,j+k]+1;

					//Check vertically
					for (int k = 0; v && k < s; k++)
					{	
						if(i+k>=10)
							v = false;
						else
							v = checkKnown(i+k,j);
					}
					//If it fits, increment
					for (int k = 0; v && k < s; k++)
						probabilites_[i+k,j] = probabilites_[i+k,j]+1;
				}
			}
		}
		int max=0, maxX=0, maxY=0;
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				if(probabilites_[i,j] > max && !tries_[i,j])
				{
					max=probabilites_[i,j];
					maxX=i;
					maxY=j;
				}
			}
		}

		hit_.x=maxX;
		hit_.y=maxY;
		Debug.Log(max+" "+maxX+" "+maxY);
	}
	private void resetHit()
	{
		hit_.x = -1;
		hit_.y = -1;
	}

	private bool seek()
	{
		if(possibleBoats_.Count == 0)
		{
			CalculateProbabilities();
			return true;
		}
		return false;
	}
	private void destroy()
	{
		//Pick one from the possible list
		int position = generator_.Next(0,possibleBoats_.Count);
		Vector2Int vPosition = possibleBoats_[position];
		possibleBoats_.RemoveAt(position);
		hit_.x = vPosition.x;
		hit_.y = vPosition.y;
	}
	private void ConfirmHit()
	{
		//We landed a hit, add all other posible cells to de possible list
		if(checkHit())
		{
			foreach (var dir in directions_)
			{
				Vector2Int res = hit_+dir;
				if((res.x >= 0 && res.x < 10 && res.y >= 0 && res.y < 10)&&
					!hitHistory_[res.x,res.y])
					{
						Debug.Log("Adding: "+res.x+" "+res.y);
						possibleBoats_.Add(hit_+dir);
					}
			}
		}
	}
	public override AttackData Attack(){
		fleet_ = SelectTarget();
		ConfirmHit();
		if(!seek())
			destroy();

		tries_[hit_.x,hit_.y] = true;
		return new AttackData(hit_.x,hit_.y,"Player");
	}
	protected override Fleet SelectTarget()
	{
		return GameManager.Instance().GetFleet("Player");
	}
}
