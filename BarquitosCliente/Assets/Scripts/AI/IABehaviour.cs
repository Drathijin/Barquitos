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

	public virtual AttackData Attack() {
		return new AttackData();
	}
	protected virtual Fleet SelectTarget(){return new Fleet();}
}
