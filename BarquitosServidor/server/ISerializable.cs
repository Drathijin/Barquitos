using System;
using System.Diagnostics;
using System.Text;

public class ISerializable {
	protected uint size_;
	protected byte[] data_;
	public uint GetSize(){return size_;}
	public virtual Byte[] ToBin(){return null;}
	public virtual void FromBin(Byte[] data){}
}

public class IMessage : ISerializable
{
	public enum MessageType : int
	{
		ClientConection,//Manda nombre y modo de juego
		ServerConection,//
		ClientSetup,    //Manda todos los barcos de un cliente
		ServerSetup,    //Manda todos los nombres de los jugadores de la partida a los clientes de la partida
		ClientAttack,   //Manda un x,y para el próximo ataque
		ServerAttack,   //Responde con el resultado de la última ronda de ataques
		FleetDefeated,  //Name de la fleet derrotada
		EndGame,        //NAme del jugador ganador
	}

/*
-----------------
|               |   <--- | Message Type
|               |   <--- | Game ID
-----------------
|               |
|               |
|               |
|               |
|               |
|               |
-----------------
*/
	public struct Header
	{
			public MessageType messageType_;
			public System.Guid gameID_;
	}
	public Header header_;

	protected static int HEADER_SIZE = 20;
	protected static int MESSAGE_SIZE = 1024;
	
	//!Carefull only use this on server to cast message to correct type after it has been constructed by socket
	public byte[] GetData(){return data_;}
	
	public IMessage(MessageType messageType, System.Guid id)
	{
		header_ = new IMessage.Header();
		header_.messageType_ = messageType;
		header_.gameID_ = id;
		data_ = new Byte[MESSAGE_SIZE];
		size_ = (uint)MESSAGE_SIZE;
	}

	override public Byte[] ToBin()
	{
		Byte[] bytes = BitConverter.GetBytes((int)header_.messageType_);
		bytes.CopyTo(data_,0);

		Byte[] guid = header_.gameID_.ToByteArray();
		guid.CopyTo(data_,4);

		Console.WriteLine(header_.gameID_.ToString());
		Console.WriteLine((new System.Guid(guid)).ToString());
		return data_;
	}
	override public void FromBin(Byte[] data)
	{
		data_ = data;
		
		byte[] bytes = new byte[16];
		Buffer.BlockCopy(data_,4,bytes,0,16);

		Console.WriteLine(new System.Guid(bytes).ToString());
		
		header_.messageType_ = (IMessage.MessageType)BitConverter.ToInt32(data_,0);
		header_.gameID_ = new System.Guid(bytes);
	}
}