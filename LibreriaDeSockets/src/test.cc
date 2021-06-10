#include "iostream"

#ifdef __WIN32__
#define SOCKET_API __declspec(dllexport)
#else 
#define SOCKET_API
#endif // DEBUG



extern "C" void test(int a, int b){
	printf("Test from c++ %d + %d = %d\n",a,b,a+b);
}