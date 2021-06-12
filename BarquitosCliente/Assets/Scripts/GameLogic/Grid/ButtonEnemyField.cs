using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnemyField : GridObject
{
    Button btn_;

    private void Start()
    {
        btn_ = GetComponent<Button>();
    }

    public void OnClick()
    {
        Debug.Log("X: " + (data_.GetX() + 1) + " Y: " + (data_.GetY() + 1));

        GameManager.Instance().CurrentEnemyFleet().Attack(data_.GetX(), data_.GetY());

        btn_.enabled = false;
    }
}
