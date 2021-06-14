using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class MediumBehaviour : IABehaviour
{
	System.Random generator_;
	List<int> positions_;

	Vector2Int hit_;
	Vector2Int[] directions_;
	List<Vector2Int> possibleBoats_;
	List<Vector2Int> hitHistory_;
	private void Start() {
		generator_ = new System.Random();
		positions_ = new List<int>();
		directions_ = new Vector2Int[4];
		directions_[0] = new Vector2Int(1,0);
		directions_[1] = new Vector2Int(-1,0);
		directions_[2] = new Vector2Int(0,-1);
		directions_[3] = new Vector2Int(0,1);
		possibleBoats_ = new List<Vector2Int>();
		CheckerBoardPositionSet();
	}

	private bool checkHit()
	{
		Debug.LogError(fleet_.GetGrid().GetPos(hit_.x,hit_.y).Data().State());
		if(fleet_.GetGrid().GetPos(hit_.x,hit_.y).Data().State() == CellData.CellState.HIT)
			Debug.LogError("ASSSASASASASASASDADSDASDSAD");
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
		//We landed a hit, add all other posible cells to de possible list
		if(checkHit())
		{
			foreach (var dir in directions_)
			{
				if(hit_.x+dir.x >= 0 && dir.x +hit_.x < 10 && hit_.y+dir.y >= 0 && dir.y +hit_.y < 10)
					if(possibleBoats_.Find(x => (x.x==(hit_+dir).x)&&(x.y == (hit_+dir).y)) != new Vector2Int() || hit_+dir == new Vector2Int())
						possibleBoats_.Add(hit_+dir);
			}
		}
		//Pick one from the possible list
		int position = generator_.Next(0,possibleBoats_.Count);
		Vector2Int vPosition = possibleBoats_[position];	
		possibleBoats_.RemoveAt(position);
		hit_.x = vPosition.x;
		hit_.y = vPosition.y;
	}
	public override AttackData Attack(){
		fleet_ = SelectTarget();
		if(!seek())
			destroy();

		return new AttackData(hit_.x,hit_.y,"Player");
	}
	protected override Fleet SelectTarget()
	{
		return GameManager.Instance().GetFleet("Player");
	}
}
