using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridObject : MonoBehaviour
{
    [SerializeField]
    protected Sprite hit_;
    [SerializeField]
    protected Sprite miss_;

    [SerializeField]
    protected Sprite default_;

    [SerializeField]
    protected Sprite selected_;

    protected Image img_;

    protected CellData data_ = new CellData();

    private Fleet fleet_;

    private void Awake()
    {
        img_ = GetComponent<Image>();
        SetState(CellData.CellState.UNKNOWN);
    }

    public void SetPosition(int x, int y)
    {
        data_.SetPosition(x, y);
    }

    public void Attack()
    {
        if (data_.Ship())
            SetState(CellData.CellState.HIT);
        else
            SetState(CellData.CellState.MISSED);
    }

    void SetState(CellData.CellState state)
    {
        if (data_.State() == state)
            return;

        data_.SetState(state);
        switch (state)
        {
            case CellData.CellState.HIT:
                HitState();
                break;
            case CellData.CellState.MISSED:
                MissedState();
                break;
            default:
                break;
        }
    }

    public void SetShip(bool boat)
    {
        data_.SetShip(boat);
    }

    public CellData Data()
    {
        return data_;
    }

    public void SetFleet(Fleet fleet)
    {
        fleet_ = fleet;
    }

    public Fleet Fleet()
    {
        return fleet_;
    }

    virtual protected void HitState()
    {
        img_.sprite = hit_;
    }

    virtual protected void MissedState() { img_.sprite = miss_; }
}
