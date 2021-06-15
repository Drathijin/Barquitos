using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MediumBehaviour : IABehaviour
{
	List<int> positions_;

	Vector2Int hit_;
	Vector2Int[] directions_;
	List<Vector2Int> possibleBoats_;
	bool[,] hitHistory_;

	private void Start() {
		positions_ = new List<int>();
		directions_ = new Vector2Int[4];
		directions_[0] = new Vector2Int(1,0);
		directions_[1] = new Vector2Int(-1,0);
		directions_[2] = new Vector2Int(0,-1);
		directions_[3] = new Vector2Int(0,1);
		possibleBoats_ = new List<Vector2Int>();
		hitHistory_ = new bool[10,10];
		for(int i = 0; i<10;i++)
			for(int j = 0; j<10;j++)
				hitHistory_[i,j] = false;
		CheckerBoardPositionSet();
	}

	private bool checkHit()
	{
		if(fleet_.GetGrid().GetPos(hit_.x,hit_.y).Data().State() == CellData.CellState.HIT)
			hitHistory_[hit_.x, hit_.y] = true;
		return fleet_.GetGrid().GetPos(hit_.x,hit_.y).Data().State() == CellData.CellState.HIT;
	}
	//Saves board positions in a checkerboard pattern
	private void CheckerBoardPositionSet()
	{
		//Add all even numbers
		Debug.Log("|| POSITIONS ||");
		var str= new char[10];
		int count = 0;
		int parity = 0;
		for(int i = 0;i < 100; i++)
		{
			if(i%2 == parity)
			{
				positions_.Add(i);
				str[i%10]='#';
			}
			else
				str[i%10]='-';
			if(count==9)
			{
				Debug.Log(new string(str));
				count= 0;
				parity = parity == 0 ? 1 :0;
			}
			else
				count++;
		}
		Debug.Log("|| ENDPOSITIONS ||");
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
			int position = generator_.Next(0,positions_.Count);
			position = positions_[position];	
			hit_.x = position %10;
			hit_.y = position /10;
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
		positions_.Remove(vPosition.x+vPosition.y*10);
		hit_.x = vPosition.x;
		hit_.y = vPosition.y;
	}
	private void ConfirmHit()
	{
		//We landed a hit, add all other posible cells to de possible list
		if(checkHit())
		{
			if(fleet_.GetGrid().GetPos(hit_.x,hit_.y).Data().Ship().Destroyed())
			{
				possibleBoats_.Clear();
			}
			else
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
	}
	public override AttackData Attack(){
		fleet_ = SelectTarget();
		ConfirmHit();
		if(!seek())
			destroy();

		return new AttackData(hit_.x,hit_.y,"Player");
	}
	protected override Fleet SelectTarget()
	{
		return GameManager.Instance().GetFleet("Player");
	}
}
