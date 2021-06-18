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

		[DllImport("socket")]
		private static extern int bind_socket(IntPtr sock);

		[DllImport("socket", CallingConvention = CallingConvention.Cdecl)]
		private static extern int send_socket(IntPtr sock, [Out]byte[] buff, int size, IntPtr other);

		[DllImport("socket", CallingConvention = CallingConvention.Cdecl)]
		private static extern int recv_socket(IntPtr sock, [Out]byte[] buff, int size);

		[DllImport("socket")]
		private static extern int Init_Sockets();
		
		[DllImport("socket")]
		private static extern int Quit_Sockets();

		public static void InitSockets(){
			Init_Sockets();
		}
		public static void QuitSockets(){
			Quit_Sockets();
		}
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
		public void Bind()
		{
			bind_socket(internal_socket);
		}
		public bool Send(byte[] s, IntPtr other)
		{
			int val = send_socket(internal_socket, s, s.Length+1, (other==IntPtr.Zero) ? internal_socket : other);
			if(val == 1102 || val == 112) //Check for EAgain or EWouldBlock
				return false;
			else if(val >=0)
				return true;
			else
				throw new Exception("Error sending with socket. Error code: "+val);
		}
		public bool Send(ISerializable serializable)
		{
			int size = (int)serializable.GetSize();
			byte[] bytes = serializable.ToBin();;
			bool ret = Send(bytes, IntPtr.Zero);
			return ret;
		}

		public bool Recv(byte[] buffer, int size)
		{
			int val = recv_socket(internal_socket, buffer, size);
			if(val >0)
				return true;
			else if(val == 0)
				return false;
			else 
				throw new Exception("Error recv with socket. Error code");
		}

		public bool Recv(ISerializable serializable)
		{
			int size = (int)serializable.GetSize();
			byte[] bytes = new byte[size];
			bool ret = Recv(bytes, size);
			serializable.FromBin(bytes);
			return ret;
		}
	} 
}
