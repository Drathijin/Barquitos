using System;
using System.Text;
using System.Runtime.InteropServices;


namespace server
{
	class Socket : IDisposable
	{
		[DllImport("socket")]
		private static extern void test(int a,int b);
			
		[DllImport("socket", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr make_socket(String str, String port);

		[DllImport("socket")]
		private static extern int test_socket(IntPtr sock);

		[DllImport("socket")]
		private static extern int destroy_socket(IntPtr sock);

		[DllImport("socket", CallingConvention = CallingConvention.Cdecl)]
		private static extern int send_socket(IntPtr sock, String buff, int size, IntPtr other);


		private IntPtr internal_socket;

		public Socket(String addr, String port)
		{
			internal_socket = make_socket(addr, port);
		}
		public void Dispose()
		{
			destroy_socket(internal_socket);
			Console.WriteLine("\nDestructor called, we can live happy");
		}
		
		public int Test()
		{
				return test_socket(internal_socket);
		}
		public int Send(String s, IntPtr other)
		{
			return send_socket(internal_socket, s, s.Length+1, (other==IntPtr.Zero) ? internal_socket : other);
		}
	} 
}
