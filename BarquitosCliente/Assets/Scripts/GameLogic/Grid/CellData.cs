
public class CellData
{
    public enum CellState
    {
        UNKNOWN,
        HIT,
        MISSED
    }

    private int x_, y_;

    private CellState state_;

    private BattleShip ship_ = null;

    public CellData()
    {
        x_ = -1;
        y_ = -1;
        state_ = CellState.MISSED;
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

    public BattleShip Ship()
    {
        return ship_;
    }

    public void SetShip(BattleShip ship)
    {
        ship_ = ship;
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
