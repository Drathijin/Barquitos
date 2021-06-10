using System;
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
					test(2,1);
					test(1,15);
					test(15,15);
					test(0,15);
			}
	} 
}
