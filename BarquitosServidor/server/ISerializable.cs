using System;
using System.Diagnostics;
using System.Text;

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

			var aux = System.Text.UnicodeEncoding.Unicode.GetBytes(name_);
			Array.Resize<Byte>(ref aux, MAX_NAME_SIZE);

			var aux2 = System.Text.UnicodeEncoding.Unicode.GetBytes(text_);
			Array.Resize<Byte>(ref aux2, MAX_TEXT_SIZE);
			
			aux.CopyTo(data_, 0);
			aux2.CopyTo(data_, MAX_NAME_SIZE);

		if(BitConverter.IsLittleEndian)
			Array.Reverse(data_);
		return data_;
	}
	override public void FromBin(Byte[] data)
	{
		if(BitConverter.IsLittleEndian)
			Array.Reverse(data);
		data_ = data;

		name_ = System.Text.UnicodeEncoding.Unicode.GetString(data_,0,MAX_NAME_SIZE);
		text_ = System.Text.UnicodeEncoding.Unicode.GetString(data_,MAX_NAME_SIZE,MAX_TEXT_SIZE);
	}
}