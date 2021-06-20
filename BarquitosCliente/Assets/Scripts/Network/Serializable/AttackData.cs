using System;
using System.Diagnostics;
using System.Text;

public class AttackData : IMessage
{
  private static int MAX_NAME_SIZE = 24;
  public string enemyId;
  public string myId;

  public int x, y;

  public AttackData(int x = -1, int y = -1, string id = "", string myID_ = "") : base(IMessage.MessageType.ClientAttack, System.Guid.Empty)
  {
    this.x = x;
    this.y = y;
    enemyId = id;
    myId = myID_;
    size_ = (uint)(MAX_NAME_SIZE + sizeof(int) * 2);
  }

  override public Byte[] ToBin()
  {
    base.ToBin();
    int index = HEADER_SIZE;

    var aux = System.Text.UnicodeEncoding.Unicode.GetBytes(enemyId);
    Array.Resize<Byte>(ref aux, MAX_NAME_SIZE);

    var myName = System.Text.UnicodeEncoding.Unicode.GetBytes(myId);
    Array.Resize<Byte>(ref aux, MAX_NAME_SIZE);

    var x_ = BitConverter.GetBytes(x);
    var y_ = BitConverter.GetBytes(y);

    aux.CopyTo(data_, index);
    index += MAX_NAME_SIZE;
    myName.CopyTo(data_, index);
    index += MAX_NAME_SIZE;
    x_.CopyTo(data_, index);
    index += sizeof(int);
    y_.CopyTo(data_, index);

    return data_;
  }
  override public void FromBin(Byte[] data)
  {
    base.FromBin(data);

    int index = HEADER_SIZE;

    enemyId = System.Text.UnicodeEncoding.Unicode.GetString(data_, index, MAX_NAME_SIZE);
    index += MAX_NAME_SIZE;
    myId = System.Text.UnicodeEncoding.Unicode.GetString(data_, index, MAX_NAME_SIZE);
    index += MAX_NAME_SIZE;
    x = BitConverter.ToInt32(data_, index);
    index += sizeof(int);
    y = BitConverter.ToInt32(data_, index);
  }
}
