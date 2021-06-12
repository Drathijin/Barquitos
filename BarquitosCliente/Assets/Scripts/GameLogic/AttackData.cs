struct AttackData
{
    public AttackData(int x, int y , string id)
    {
        this.x = x;
        this.y = y;
        enemyId = id;
    }

    public string enemyId;

    public int x, y;
}
