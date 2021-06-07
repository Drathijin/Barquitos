using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShip : MonoBehaviour
{
    [SerializeField]
    int size = 2;

    RectTransform tr_;

    private void Start()
    {
        tr_ = GetComponent<RectTransform>();
    }

}
