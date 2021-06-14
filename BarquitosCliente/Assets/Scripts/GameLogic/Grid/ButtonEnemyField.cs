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
        if (GameManager.Instance().State() != GameManager.GameState.SELECTING)
            return;
        Debug.Log("X: " + (data_.GetX() + 1) + " Y: " + (data_.GetY() + 1));

        var gm = GameManager.Instance();
        var ce = gm.PlayerManager();
        ce.SetAttackButton(this);
    }
}
