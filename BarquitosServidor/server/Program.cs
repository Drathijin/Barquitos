using System;
using System.Text;
using System.Runtime.InteropServices;


namespace server
{
	class Program
	{
		[DllImport("socket")]
		private static extern void test(int a,int b);
		
		public static string IP;
		public static string PORT;

		static void Client(string name)
		{
			using (Socket sock = new Socket(IP, PORT))
			{
				while(true)
				{
					String input = Console.ReadLine();
					if(input == "!q")
						return;

					Message message = new Message(name, input);
					sock.Send(message);
				}
			}
		}
		static void Server()
		{
			using (Socket sock = new Socket(IP, PORT))
			{
				sock.Bind();
				while(true)
				{
					Message message = new Message("","");
					sock.Recv(message);
					Console.WriteLine(message.name_);
					Console.WriteLine(message.text_);
				}
			}
		}

		static void Main(string[] args)
		{
				Socket.InitSockets();
				if(args.Length == 3)
				{
					IP = args[1];
					PORT = args[2];
					Client(args[0]);
				}
				else if(args.Length == 2)
				{
					IP = args[0];
					PORT = args[1];
					Server();
				}
				Socket.QuitSockets();
		}
	} 
}
