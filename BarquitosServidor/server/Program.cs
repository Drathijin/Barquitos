using System;
using System.Text;
using System.Runtime.InteropServices;


namespace server
{
	class Program
	{
		[DllImport("socket")]
		private static extern void test(int a,int b);
		
		static void Main(string[] args)
		{
				test(1,1);
				using(Socket sock = new Socket("127.0.0.1", "49671")){
					Console.Write(sock.Test());
					Console.Write(sock.Test());
					Console.Write(sock.Test() + 4);
					Console.Write(args[0]);

					foreach(string s in args)
						Console.WriteLine(sock.Send(s+"\n", IntPtr.Zero));
				}
		}
	} 
}
