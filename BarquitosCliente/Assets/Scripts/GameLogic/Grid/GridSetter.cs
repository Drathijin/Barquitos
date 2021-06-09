using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSetter : MonoBehaviour
{
    public enum GridType
    {
        ENEMY,
        PLAYER
    }

    public GridType type;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<GridObject>().SetPosition(i % 10, i / 10);
        }
        if (GameManager.Instance())
            GameManager.Instance().SetGrid(this);
    }
}
