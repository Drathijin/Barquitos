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

    public void SetFleet(Fleet fleet)
    {
        fleet_ = fleet;
    }

    public Fleet Fleet()
    {
        return fleet_;
    }

    protected void EmptyState() {
        HitState();
    }
    protected void HitState() {
        img_.sprite = hit_;
    }
		public void setSelected(){img_.sprite = selected_;}
		public void setDeselected(){if(img_.sprite == selected_)img_.sprite = default_;}
		public void setHit(){HitState();}
    protected void MissedState() { img_.sprite =  miss_;}
}
