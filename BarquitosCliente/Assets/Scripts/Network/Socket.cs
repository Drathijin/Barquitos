using System;
using System.Text;
using System.Runtime.InteropServices;


namespace server
{
  public class Socket : IDisposable
  {

    [DllImport("socket", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr make_socket(String str, String port);

    [DllImport("socket")]
    private static extern int destroy_socket(IntPtr sock);

    [DllImport("socket")]
    private static extern int bind_socket(IntPtr sock);

    [DllImport("socket", CallingConvention = CallingConvention.Cdecl)]
    private static extern int send_socket(IntPtr sock, [Out] byte[] buff, int size, IntPtr other);

    [DllImport("socket", CallingConvention = CallingConvention.Cdecl)]
    private static extern int recv_socket(IntPtr sock, [Out] byte[] buff, int size, ref IntPtr o);

    [DllImport("socket")]
    private static extern int Init_Sockets();

    [DllImport("socket")]
    private static extern int Quit_Sockets();

    public static void InitSockets()
    {
      Init_Sockets();
    }
    public static void QuitSockets()
    {
      Quit_Sockets();
    }
    private IntPtr internal_socket;

    public Socket(IntPtr ptr)
    {
      internal_socket = ptr;
    }

    public Socket(String addr, String port)
    {
      internal_socket = make_socket(addr, port);
      if (internal_socket == null)
        throw new Exception("Error creating Socket. Error code");
    }
    public void Dispose()
    {
      destroy_socket(internal_socket);
      Console.WriteLine("\nDestructor called, we can live happy");
    }

    public void Bind()
    {
      bind_socket(internal_socket);
    }
    public bool Send(byte[] s, IntPtr other)
    {
      int val = send_socket(internal_socket, s, s.Length, other);
      if (val <= 0) //Check for EAgain or EWouldBlock
        return false;
      else
        return true;
    }
    public bool Send(ISerializable serializable, Socket other)
    {
      int size = (int)serializable.GetSize();
      byte[] bytes = serializable.ToBin(); ;
      bool ret = Send(bytes, other.internal_socket);
      return ret;
    }

    public bool Recv(byte[] buffer, int size, out Socket other)
    {
      IntPtr o = new IntPtr();
      int val = recv_socket(internal_socket, buffer, size, ref o);
      other = new Socket(o);
      if (val > 0)
        return true;
      else if (val == 0)
        return false;
      else
        throw new Exception("Unreacheable host");
    }

    public bool Recv(byte[] buffer, int size)
    {
      IntPtr o = new IntPtr();
      int val = recv_socket(internal_socket, buffer, size, ref o);
      if (val > 0)
        return true;
      else if (val == 0)
        return false;
      else
        throw new Exception("Unreacheable host");
    }

    public bool Recv(ISerializable serializable, out Socket other)
    {
      int size = (int)serializable.GetSize();
      byte[] bytes = new byte[size];
      bool ret = Recv(bytes, size, out other);
      serializable.FromBin(bytes);
      return ret;
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
