using System;
using System.Diagnostics;
using System.Text;

public class NetworkData : ISerializable
{
    private static int MAX_NAME_SIZE = 24;
    public string playerName;
    public bool battleRoyale;

    public NetworkData()
    {
        playerName = "Player";
        battleRoyale = false;
        size_ = (uint)(MAX_NAME_SIZE + 1);
    }

    override public Byte[] ToBin()
    {
        data_ = new Byte[size_];

        var aux = System.Text.UnicodeEncoding.Unicode.GetBytes(playerName);
        Array.Resize<Byte>(ref aux, MAX_NAME_SIZE + 1);
        aux[MAX_NAME_SIZE] = BitConverter.GetBytes(battleRoyale)[0];

        aux.CopyTo(data_, 0);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(data_);
        return data_;
    }
    override public void FromBin(Byte[] data)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(data);
        data_ = data;

        playerName = System.Text.UnicodeEncoding.Unicode.GetString(data_, 0, MAX_NAME_SIZE);
        battleRoyale = BitConverter.ToBoolean(data_, MAX_NAME_SIZE);
    }
}
