using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField]
    protected int x_ = -1, y_ = -1;

    public void SetPosition(int x, int y)
    {
        x_ = x;
        y_ = y;
    }
}
