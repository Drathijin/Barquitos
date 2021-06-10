using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridObject : MonoBehaviour
{
    [SerializeField]
    protected Sprite hit_;

    protected Image img_;

    protected CellData data_;

    private void Awake()
    {
        img_ = GetComponent<Image>();
        data_ = new CellData();
        SetState(CellData.CellState.UNKNOWN);
    }

    public void SetPosition(int x, int y)
    {
        data_.SetPosition(x, y);
    }

    public bool Attack()
    {
        if (data_.Boat())
        {
            SetState(CellData.CellState.HIT);
            return true;
        }
        SetState(CellData.CellState.MISSED);
        return false;
    }

    void SetState(CellData.CellState state)
    {
        if (data_.State() == state)
            return;

        switch (state)
        {
            case CellData.CellState.UNKNOWN:
                EmptyState();
                break;
            case CellData.CellState.HIT:
                HitState();
                break;
            case CellData.CellState.MISSED:
                MissedState();
                break;
        }
    }

    public void SetBoat(bool boat)
    {
        data_.SetBoat(boat);
    }

    public CellData Data()
    {
        return data_;
    }

    protected void EmptyState() {
        HitState();
    }
    protected void HitState() {
        img_.sprite = hit_;
        img_.color = Color.white;
        img_.type = Image.Type.Simple;
    }
    protected void MissedState() { }
}
