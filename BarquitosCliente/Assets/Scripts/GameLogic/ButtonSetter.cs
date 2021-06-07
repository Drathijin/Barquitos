using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSetter : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ButtonData>().SetButton(i % 10, i / 10);
        }
    }
}
