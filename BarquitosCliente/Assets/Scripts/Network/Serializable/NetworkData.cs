using System;
using System.Diagnostics;
using System.Text;

public class NetworkData : IMessage
{
    private static int MAX_NAME_SIZE = 24;
    public string playerName;
    public bool battleRoyale;

    public NetworkData() : base(IMessage.MessageType.ClientConection, System.Guid.Empty)
    {
        playerName = "Player";
        battleRoyale = false;
    }

    override public Byte[] ToBin()
    {
        data_ = base.ToBin();
				int index = IMessage.HEADER_SIZE;

        var aux = System.Text.UnicodeEncoding.Unicode.GetBytes(playerName);
        Array.Resize<Byte>(ref aux, MAX_NAME_SIZE + 1);
        aux[MAX_NAME_SIZE] = BitConverter.GetBytes(battleRoyale)[0];

        aux.CopyTo(data_, index);

        // if (BitConverter.IsLittleEndian)
        //     Array.Reverse(data_);
        return data_;
    }
    override public void FromBin(Byte[] data)
    {
        // if (BitConverter.IsLittleEndian)
        //     Array.Reverse(data);
				base.FromBin(data);
        data_ = data;

        playerName = System.Text.UnicodeEncoding.Unicode.GetString(data_, IMessage.HEADER_SIZE, MAX_NAME_SIZE);
        battleRoyale = BitConverter.ToBoolean(data_, IMessage.HEADER_SIZE+MAX_NAME_SIZE);
    }
}
