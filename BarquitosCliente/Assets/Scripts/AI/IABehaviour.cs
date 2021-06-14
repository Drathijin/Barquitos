using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABehaviour : MonoBehaviour
{
	public Fleet fleet_; //Grid of buttons

	//Values between 1 or 0
	double centerPriority;
	double horizontalPriority;
	double closerPriority;

	public void Setup(Fleet enemyFleet) {
		fleet_ = enemyFleet;
	}
	public virtual AttackData Attack() {
		return new AttackData();
	}
}
