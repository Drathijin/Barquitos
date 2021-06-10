
public class CellData
{
    public enum CellState
    {
        UNKNOWN,
        HIT,
        MISSED
    }

    private int x_, y_;

    private CellState state_ = CellState.MISSED;

    private bool boat_ = false;

    public CellData()
    {
        x_ = -1;
        y_ = -1;
        state_ = CellState.UNKNOWN;
        boat_ = false;
    }

    public int GetX()
    {
        return x_;
    }

    public int GetY()
    {
        return y_;
    }

    public void SetPosition(int x, int y)
    {
        x_ = x;
        y_ = y;
    }

    public bool Boat()
    {
        return boat_;
    }

    public void SetBoat(bool boat)
    {
        boat_ = boat;
    }

    public CellState State()
    {
        return state_;
    }

    public void SetState(CellState state)
    {
        state_ = state;
    }
}
