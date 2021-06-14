using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EasyBehaviour : IABehaviour
{
	System.Random generator_;
	List<int> positions_;

	private void Awake() {
		generator_ = new System.Random();
		positions_ = new List<int>();
		for(int i = 0;i < 100; i++)
			positions_.Add(i);
	}
	public override AttackData Attack(){
		int random = generator_.Next(0, positions_.Count);
		int pos = positions_[random];
		positions_.RemoveAt(random);
		return new AttackData(pos%10,pos/10,"Player");
	}

}
