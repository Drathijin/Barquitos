using System;
using System.Diagnostics;

public class ISerializable {
	protected uint size_;
	public uint GetSize(){return size_;}
	public virtual Byte[] ToBin(){return null;}
	public virtual void FromBin(Byte[] data){}
}

public class Message : ISerializable
{
	private byte[] data_;
	public string name_;
	public string text_;
	private static int MAX_NAME_SIZE = 256;
	private static int MAX_TEXT_SIZE = 1024;

	public Message(string name, string text)
	{
		size_ = (uint)(MAX_NAME_SIZE+MAX_TEXT_SIZE) * 2; // every char is 2 bytes in size bcs unicode
		data_ = new Byte[size_];
		name_=name;
		text_=text;
	}

	override public Byte[] ToBin(){
		data_ = new Byte[size_];

		for(int i = 0; i+1<MAX_NAME_SIZE;i++)
		{
			data_[2*i]   = BitConverter.GetBytes((i<name_.Length) ?  name_[i] : '\0')[0];
			data_[2*i+1] = BitConverter.GetBytes((i<name_.Length) ?  name_[i] : '\0')[1];
		}

		for(int i = 0; i+1<MAX_TEXT_SIZE;i++)
		{
			data_[MAX_NAME_SIZE+2*i]   = BitConverter.GetBytes((i<text_.Length) ?  text_[i] : '\0')[0];
			data_[MAX_NAME_SIZE+2*i+1] = BitConverter.GetBytes((i<text_.Length) ?  text_[i] : '\0')[1];
		}

		if(BitConverter.IsLittleEndian)
			Array.Reverse(data_);
		return data_;
	}
	override public void FromBin(Byte[] data)
	{
		if(BitConverter.IsLittleEndian)
			Array.Reverse(data);
		data_ = data;

		char[] name = new char[MAX_NAME_SIZE];
		for(int i = 0; i<MAX_NAME_SIZE;i++)
		{
			name[i] = BitConverter.ToChar(data_,i*2);
			if(name[i]=='\0')
				break;
		}

		char[] text = new char[MAX_TEXT_SIZE];
		for(int i = 0; i<MAX_TEXT_SIZE;i++)
		{
			text[i] = BitConverter.ToChar(data_,MAX_NAME_SIZE+(i*2));
			if(text[i]=='\0')
				break;
		}
		name_ = new String(name);
		text_ = new String(text);

	}
}