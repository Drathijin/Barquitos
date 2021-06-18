using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnemyField : GridObject
{
    Button btn_;

    [SerializeField]
    Sprite selectedSpr;

    [SerializeField]
    Image imgViewer_;

    private void Start()
    {
        btn_ = GetComponent<Button>();
        imgViewer_.enabled = false;
    }

    public void OnClick()
    {
        if (GameManager.Instance().State() != GameManager.GameState.SELECTING ||
            GameManager.Instance().FleetsReady[GameManager.Instance().PlayerManager().GetFleet().Name()])
            return;

        var gm = GameManager.Instance();
        var ce = gm.PlayerManager();
        ce.SetAttackButton(this);
    }

    public void Selected(bool selected)
    {
        img_.sprite = selected ? selected_ : default_;
    }

    private void Update()
    {
        if (GameManager.Instance().GetGameType() != GameManager.GameType.AI)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            imgViewer_.enabled = !imgViewer_.enabled && data_.Ship() != null;
        }
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
