using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MediumBehaviour : IABehaviour
{
	System.Random generator_;
	List<int> positions_;

	Vector2Int hit_;

	private void Start() {
		generator_ = new System.Random();
		positions_ = new List<int>();
		
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


		hit_= new Vector2Int(-1,-1);
	}
	private bool checkHit()
	{
		return hit_.x != -1 && hit_.y != -1;
	}
	private void resetHit()
	{
		hit_.x = -1;
		hit_.y = -1;
	}

	private bool seek()
	{
		int position = generator_.Next(0,positions_.Count);
		// positions_[position];
		return false;
	}
	private bool destroy()
	{
		return false;
	}
	public override AttackData Attack(){
		int random = generator_.Next(0, positions_.Count);
		int pos = positions_[random];
		positions_.RemoveAt(random);
		return new AttackData(pos%10,pos/10,"pepepopo");
	}

}
