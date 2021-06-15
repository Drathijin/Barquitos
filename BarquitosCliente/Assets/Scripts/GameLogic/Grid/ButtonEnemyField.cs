using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnemyField : GridObject
{
    Button btn_;

    [SerializeField]
    Sprite selectedSpr;

    private void Start()
    {
        btn_ = GetComponent<Button>();
    }

    public void OnClick()
    {
        if (GameManager.Instance().State() != GameManager.GameState.SELECTING)
            return;

        var gm = GameManager.Instance();
        var ce = gm.PlayerManager();
        ce.SetAttackButton(this);
    }

    public void Selected(bool selected)
    {
        img_.sprite = selected ? selected_ : default_;
    }

    protected override void HitState()
    {
        btn_.enabled = false;
        img_.sprite = hit_;
    }

    protected override void MissedState()
    {
        btn_.enabled = false;
        img_.sprite = miss_;
    }
}
