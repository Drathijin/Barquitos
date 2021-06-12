using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFleet : MonoBehaviour
{
    Transform positions_;

    Grid grid_;

    string enemyName_;

    private void Awake()
    {
        grid_ = new Grid(GetComponentsInChildren<GridObject>());
    }

    public Grid GetGrid()
    {
        return grid_;
    }

    public void Attack(int x, int y)    // Recivir el ataque de alguien, devolver resultado
    {                                       

    }         

    //public abstract void ManageTurn();                  // Tomar la decision de ataque en el turno y guardarla para el ResolveTurn
    //public abstract void ResolveTurn();                 // Ejecutar la decisión de ataque tomada en el ManageTurn

    public void SetId(string id)
    {
        enemyName_ = id;
    }
}
