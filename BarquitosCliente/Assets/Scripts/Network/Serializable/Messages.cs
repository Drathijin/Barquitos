using System.Collections;
using System.Collections.Generic;
using server;

public class ServerConection : IMessage
{	public ServerConection(System.Guid id) : base(IMessage.MessageType.ServerConection, id){}
}