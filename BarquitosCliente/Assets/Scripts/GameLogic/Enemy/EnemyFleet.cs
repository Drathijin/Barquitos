using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyFleet : MonoBehaviour
{
    [SerializeField]
    Transform positions_;

    Grid grid_;

    string enemyName_;

    private void Awake()
    {
        grid_ = new Grid(positions_.GetComponentsInChildren<GridObject>());

        if (GameManager.Instance())
            GameManager.Instance().AddEnemyFleet(this);
    }

    public Grid GetGrid()
    {
        return grid_;
    }

    public abstract void Attack(int x , int y);
    public abstract void ManageTurn();
}
