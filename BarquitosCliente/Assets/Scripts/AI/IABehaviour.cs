using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABehaviour : MonoBehaviour
{
	public EnemyFleet fleet_; //Grid of buttons
	private CellData[,] data_;

	//Values between 1 or 0
	double centerPriority;
	double horizontalPriority;
	double closerPriority;

	public void Setup(EnemyFleet enemyFleet) {
		fleet_ = enemyFleet;
		data_ = new CellData[10,10];
		
		//Save status of the grid at start up
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				data_[i,j]=fleet_.GetGrid().GetPos(i,j).Data();
			}
		}
	}
	public virtual AttackData Attack() {
		return new AttackData();
	}
}
