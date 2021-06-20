using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using server;

public class ServerConection : IMessage
{	
	public ServerConection() : base(IMessage.MessageType.ServerConection, System.Guid.Empty){}
	override public Byte[] ToBin()
	{
		return base.ToBin();
	}
	override public void FromBin(Byte[] data)
	{
		base.FromBin(data);
		data_ = data;
	}
}


public class ClientSetup : IMessage
{
	private List<BattleShip> bs_;
	public ClientSetup(System.Guid id, List<BattleShip> bs) : base(IMessage.MessageType.ClientSetup, id)
	{
		bs_ = bs;
	}

	override public Byte[] ToBin()
	{
		base.ToBin();
		int index = HEADER_SIZE;

		var count = BitConverter.GetBytes(bs_.Count);
		count.CopyTo(data_, index);
		index+= sizeof(int);

		foreach (BattleShip ship in bs_)
		{
			var shipSize = BitConverter.GetBytes(ship.GetSize());
			var hor = BitConverter.GetBytes(ship.horizontal);

			shipSize.CopyTo(data_, index);
			hor.CopyTo(data_, index+sizeof(int));

			var x_ = BitConverter.GetBytes(ship.PlacedPositions()[0].x);
			var y_ = BitConverter.GetBytes(ship.PlacedPositions()[0].y);
			x_.CopyTo(data_, index + sizeof(int)   + 1);
			y_.CopyTo(data_, index + sizeof(int)*2 + 1);

			index+= sizeof(int)*3 + 1;
		}
		return data_;
	}
	override public void FromBin(Byte[] data)
	{
		base.FromBin(data);

		int index = HEADER_SIZE;

		int size = BitConverter.ToInt32(data, index);
		bs_ = new List<BattleShip>();

		index+= sizeof(int);

		for(int i = 0;i<size;i++)
		{
			int bSize =  BitConverter.ToInt32(data, index);
			index+=sizeof(int);
			
			bool h = BitConverter.ToBoolean(data, index);
			index++;
			
			int x = BitConverter.ToInt32(data, index);
			index += sizeof(int);

			int y = BitConverter.ToInt32(data, index);
			index += sizeof(int);

			BattleShip b = new BattleShip(bSize);
			b.horizontal = h;
			for(int j = 0; j<bSize;j++)
			{
				b.AddPlacedPosition(x + j*(h? 1:0), y + j*(h? 0:1));
			}
		}
	}
}

public class ServerSetup : IMessage
{
	List<string> names_;
	static int MAX_NAME_SIZE=24;

	public ServerSetup(System.Guid id, List<string> names) : base(IMessage.MessageType.ServerSetup, id){
		names_ = names;
	} 
	override public Byte[] ToBin()
	{
		base.ToBin();
		int index= HEADER_SIZE;

		byte[] size = BitConverter.GetBytes(names_.Count);
		
		size.CopyTo(data_,index);
		index+=sizeof(int);


		foreach(string name in names_)
		{
			byte[] str = Encoding.Unicode.GetBytes(name);
			Array.Resize(ref str, MAX_NAME_SIZE);
			
			str.CopyTo(data_,index);
			index+=MAX_NAME_SIZE;
		}
		return data_;
	}
	override public void FromBin(Byte[] data)
	{
		base.FromBin(data);

		int index = HEADER_SIZE;
		int length = BitConverter.ToInt32(data_,index);
		index+=sizeof(int);
		for (int i = 0; i < length; i++)
		{
				string str = Encoding.Unicode.GetString(data_,index,MAX_NAME_SIZE);
				names_.Add(str);
				index+=MAX_NAME_SIZE;
		}
	}
}