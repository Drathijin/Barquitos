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
				using(Socket sock = new Socket("localhost", "8080")){
					Console.WriteLine("Listening with IP 127.0.0.1 in port 8080");
					sock.Bind();
					
					char[] buffer = new char[80]; 
					while(buffer[0] != '!')
					{
						if(sock.Recv(buffer,80))
							Console.Write(new string(buffer));
						else
							Console.WriteLine("whoopsie");
					}
				}
		}
	} 
}
