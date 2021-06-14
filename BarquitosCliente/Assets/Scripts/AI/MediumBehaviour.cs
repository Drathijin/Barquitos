using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MediumBehaviour : IABehaviour
{
	System.Random generator_;
	List<int> positions_;

	Vector2Int hit_;

	private void Awake() {
		generator_ = new System.Random();
		positions_ = new List<int>();
		
		//Add all even numbers
		for(int i = 0;i < 100; i++)
		{
			if((i/10)%2 == 0) //Even row
			{
				if(i%2 == 0)
				{
					positions_.Add(i);
				}
			}
			else //Odd row
			{
				if(i%2 == 1)
				{
					positions_.Add(i);
				}
			}
		}
		string str ="";
		Debug.Log("|| POSITIONS ||");
		for(int i = 0;i < 100; i++)
		{
			if(positions_.Find((int a)=>{return a==i;})!=0 || i == 0)
				str+="#";
			else
				str+="-";
			if(i%10==0)
			{
				Debug.Log(str);
				str="";
			}
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
