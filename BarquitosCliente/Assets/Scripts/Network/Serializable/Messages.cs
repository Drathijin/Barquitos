using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using server;

public class ClientSetup : IMessage
{
  private List<BattleShip> bs_;
  public string name_;
  public ClientSetup(System.Guid id, List<BattleShip> bs, string name) : base(IMessage.MessageType.ClientSetup, id)
  {
    bs_ = bs;
    name_ = name;
  }
  public List<BattleShip> GetBattleShips() { return bs_; }
  override public Byte[] ToBin()
  {
    base.ToBin();
    int index = HEADER_SIZE;

    byte[] str = Encoding.Unicode.GetBytes(name_);
    Array.Resize(ref str, 24);
    str.CopyTo(data_, index);
    index += 24;

    var count = BitConverter.GetBytes(bs_.Count);
    count.CopyTo(data_, index);
    index += sizeof(int);

    for (int i = 0; i < bs_.Count; i++)
    {
      BattleShip ship = bs_[i];
      var shipSize = BitConverter.GetBytes(ship.GetSize());
      shipSize.CopyTo(data_, index);
      index += sizeof(int);

      var hor = BitConverter.GetBytes(ship.horizontal);
      hor.CopyTo(data_, index);
      index++;

      var x_ = BitConverter.GetBytes(ship.PlacedPositions()[0].x);
      x_.CopyTo(data_, index);
      index += sizeof(int);

      var y_ = BitConverter.GetBytes(ship.PlacedPositions()[0].y);
      y_.CopyTo(data_, index);
      index += sizeof(int);
    }
    return data_;
  }
  override public void FromBin(Byte[] data)
  {
    base.FromBin(data);

    int index = HEADER_SIZE;

    string str = Encoding.Unicode.GetString(data_, index, 24);
    index += 24;
    name_ = str;

    int size = BitConverter.ToInt32(data, index);
    bs_ = new List<BattleShip>();
    index += sizeof(int);

    for (int i = 0; i < size; i++)
    {
      int bSize = BitConverter.ToInt32(data, index);
      index += sizeof(int);

      bool h = BitConverter.ToBoolean(data, index);
      index++;

      int x = BitConverter.ToInt32(data, index);
      index += sizeof(int);

      int y = BitConverter.ToInt32(data, index);
      index += sizeof(int);

      BattleShip b = new BattleShip(bSize);
      b.horizontal = h;
      for (int j = 0; j < bSize; j++)
      {
        b.AddPlacedPosition(x + j * (h ? 1 : 0), y + j * (h ? 0 : 1));
      }
      bs_.Add(b);
    }
  }
  public new static MessageType Type()
  {
    return MessageType.ClientSetup;
  }
}

public class ServerSetup : IMessage
{
  public List<string> names_;
  static int MAX_NAME_SIZE = 24;

  public ServerSetup(System.Guid id, List<string> names) : base(IMessage.MessageType.ServerSetup, id)
  {
    names_ = names;
  }
  override public Byte[] ToBin()
  {
    base.ToBin();
    int index = HEADER_SIZE;

    byte[] ssize = BitConverter.GetBytes(names_.Count);

    ssize.CopyTo(data_, index);
    index += sizeof(int);


    foreach (string name in names_)
    {
      byte[] str = Encoding.Unicode.GetBytes(name);
      Array.Resize(ref str, MAX_NAME_SIZE);
      str.CopyTo(data_, index);
      index += MAX_NAME_SIZE;
    }

    return data_;
  }
  override public void FromBin(Byte[] data)
  {
    base.FromBin(data);

    int index = HEADER_SIZE;
    int length = BitConverter.ToInt32(data_, index);
    index += sizeof(int);
    for (int i = 0; i < length; i++)
    {
      string str = Encoding.Unicode.GetString(data_, index, MAX_NAME_SIZE);
      names_.Add(str);
      index += MAX_NAME_SIZE;
    }
  }

  public new static MessageType Type()
  {
    return MessageType.ServerSetup;
  }
}

public class ServerAttack : IMessage
{
  static int MAX_NAME_SIZE = 24;
  public struct AttackResult
  {
    public AttackResult(bool hit, int x, int y, string name, bool fleetDestroyed = false)
    {
      this.hit = hit;
      this.x = x;
      this.y = y;
      this.name = name;
      this.fleetDestroyed = fleetDestroyed;
    }
    public bool hit;
    public int x;
    public int y;
    public string name;
    public bool fleetDestroyed;
  }
  public List<AttackResult> attacks_;
  public ServerAttack(System.Guid id) : base(IMessage.MessageType.ServerAttack, id)
  {
    attacks_ = new List<AttackResult>();
  }

  override public Byte[] ToBin()
  {
    base.ToBin();
    int index = HEADER_SIZE;

    byte[] size = BitConverter.GetBytes(attacks_.Count);

    size.CopyTo(data_, index);
    index += sizeof(int);


    foreach (AttackResult attack in attacks_)
    {
      var hit = BitConverter.GetBytes(attack.hit);
      hit.CopyTo(data_, index);
      index += 1;
      var x = BitConverter.GetBytes(attack.x);
      x.CopyTo(data_, index);
      index += sizeof(int);
      var y = BitConverter.GetBytes(attack.y);
      y.CopyTo(data_, index);
      index += sizeof(int);

      var str = Encoding.Unicode.GetBytes(attack.name);
      str.CopyTo(data_, index);
      index += MAX_NAME_SIZE;

      var destroy = BitConverter.GetBytes(attack.fleetDestroyed);
      destroy.CopyTo(data_, index);
      index += 1;
    }
    return data_;
  }
  override public void FromBin(Byte[] data)
  {
    base.FromBin(data);

    int index = HEADER_SIZE;

    int size = BitConverter.ToInt32(data, index);
    attacks_ = new List<AttackResult>();

    index += sizeof(int);

    for (int i = 0; i < size; i++)
    {
      bool hit = BitConverter.ToBoolean(data, index);
      index += 1;

      int x = BitConverter.ToInt32(data, index);
      index += sizeof(int);

      int y = BitConverter.ToInt32(data, index);
      index += sizeof(int);

      string name = Encoding.Unicode.GetString(data_, index, MAX_NAME_SIZE);
      index += MAX_NAME_SIZE;

      bool destroy = BitConverter.ToBoolean(data, index);
      index += 1;

      AttackResult result = new AttackResult(hit, x, y, name, destroy);
      attacks_.Add(result);
    }
  }

  public new static MessageType Type()
  {
    return MessageType.ServerAttack;
  }

}

public class ReadyGame : IMessage
{
  public ReadyGame(System.Guid id) : base(MessageType.ReadyTurn, id)
  { }

  public new static MessageType Type()
  {
    return MessageType.ReadyTurn;
  }
}

public class ClientExit : IMessage
{
  private static int MAX_NAME_SIZE = 24;
  public string name;

  public ClientExit(System.Guid id, string name) : base(IMessage.MessageType.ClientExit, id)
  {
    this.name = name;
  }

  override public Byte[] ToBin()
  {
    base.ToBin();
    int index = HEADER_SIZE;

    var nameBin = Encoding.Unicode.GetBytes(name);
    nameBin.CopyTo(data_, index);

    return data_;
  }
  override public void FromBin(Byte[] data)
  {
    base.FromBin(data);

    int index = HEADER_SIZE;

    name = Encoding.Unicode.GetString(data_, index, MAX_NAME_SIZE);

  }

  public new static MessageType Type()
  {
    return MessageType.ClientExit;
  }
}

public class AcceptConnection : IMessage
{
  private static int MAX_NAME_SIZE = 24;
  public string name;

  public AcceptConnection(System.Guid id, string name) : base(IMessage.MessageType.AcceptConnection, id)
  {
    this.name = name;
  }

  override public Byte[] ToBin()
  {
    base.ToBin();
    int index = HEADER_SIZE;

    var nameBin = Encoding.Unicode.GetBytes(name);
    nameBin.CopyTo(data_, index);

    return data_;
  }
  override public void FromBin(Byte[] data)
  {
    base.FromBin(data);

    int index = HEADER_SIZE;

    name = Encoding.Unicode.GetString(data_, index, MAX_NAME_SIZE);

  }

  public new static MessageType Type()
  {
    return MessageType.AcceptConnection;
  }
}