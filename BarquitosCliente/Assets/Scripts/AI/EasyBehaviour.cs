using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EasyBehaviour : IABehaviour
{
	System.Random generator_;

	private void Awake() {
		generator_ = new System.Random();
	}
	public override AttackData Attack(){
		return new AttackData(-1,-1,"");
	}

}
