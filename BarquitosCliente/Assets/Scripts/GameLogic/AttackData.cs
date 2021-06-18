using System;
using System.Diagnostics;
using System.Text;

public class AttackData : ISerializable
{
    private static int MAX_NAME_SIZE = 24;
    public string enemyId;

    public int x, y;

    public AttackData(int x = 0, int y = 0, string id = "")
    {
        this.x = x;
        this.y = y;
        enemyId = id;
        size_ = (uint)(MAX_NAME_SIZE + sizeof(int) * 2);
    }

    override public Byte[] ToBin()
    {
        data_ = new Byte[size_];

        var aux = System.Text.UnicodeEncoding.Unicode.GetBytes(enemyId);
        Array.Resize<Byte>(ref aux, MAX_NAME_SIZE);

        var x_ = BitConverter.GetBytes(x);
        var y_ = BitConverter.GetBytes(y);

        aux.CopyTo(data_, 0);
        x_.CopyTo(data_, MAX_NAME_SIZE);
        y_.CopyTo(data_, MAX_NAME_SIZE + sizeof(int));

        if (BitConverter.IsLittleEndian)
            Array.Reverse(data_);
        return data_;
    }
    override public void FromBin(Byte[] data)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(data);
        data_ = data;

        enemyId = System.Text.UnicodeEncoding.Unicode.GetString(data_, 0, MAX_NAME_SIZE);
        x = BitConverter.ToInt32(data_, MAX_NAME_SIZE);
        y = BitConverter.ToInt32(data_, MAX_NAME_SIZE + sizeof(int));
    }
}
