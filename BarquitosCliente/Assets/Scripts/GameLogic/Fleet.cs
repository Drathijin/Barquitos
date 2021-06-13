using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet : MonoBehaviour
{
    Grid grid_;

    string name_;

    private void Awake()
    {
        grid_ = new Grid(GetComponentsInChildren<GridObject>(), this);
    }

    public Grid GetGrid()
    {
        return grid_;
    }

    public void Attack(int x, int y)    // Recivir el ataque de alguien, devolver resultado
    {

    }

    public void SetName(string name)
    {
        name_ = name;
    }

    public string Name()
    {
        return name_;
    } 
}
