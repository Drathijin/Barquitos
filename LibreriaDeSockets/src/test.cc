#include "iostream"
#include "test.h"


#include <string.h>
#include <errno.h>

extern "C" void test(int a, int b){
	printf("Test from c++ %d + %d = %d\n",a,b,a+b);
}

void handleGAIError(const char* s,int)
{
	printf("Some error with, Ricky talking from dll land %s\n", s);
	exit(1);
}
void handleError(const char* s)
{
	printf("Some error with, Ricky talking from dll land %s\n", s);
	exit(1);
}

Socket::Socket(const char *address, const char *port) : sd(-1)
{
  //Construir un socket de tipo AF_INET y SOCK_DGRAM usando getaddrinfo.
  //Con el resultado inicializar los miembros sd, sa y sa_len de la clase
  addrinfo hints, *result;
  memset(&hints, 0, sizeof(addrinfo));
  hints.ai_flags = AI_PASSIVE; //Devolver 0.0.0.0
  hints.ai_family = AF_INET;   // IPv4
  hints.ai_socktype = SOCK_DGRAM;
  int rc;
  if (rc = getaddrinfo(address, port, &hints, &result) != 0)
    handleGAIError("getaddrinfo", rc);
  if((sd = socket(result->ai_family, result->ai_socktype, 0)) < 0)
    handleError("socket");
  sa = *((struct sockaddr*)result->ai_addr);
  sa_len = result->ai_addrlen;
}

int Socket::recv(char* buff,int size, Socket *&sock)
{
  struct sockaddr sa;
  socklen_t sa_len = sizeof(struct sockaddr);

  char buffer[MAX_MESSAGE_SIZE];
  ssize_t bytes = ::recvfrom(sd, buffer, MAX_MESSAGE_SIZE, 0, &sa, &sa_len);
  
	if (bytes <= 0)
  {
    return (errno == EWOULDBLOCK || errno == EAGAIN) ? 0 : -1;
  }

  if (sock != 0)
  {
    sock = new Socket(&sa, sa_len);
  }

  //obj.from_bin(buffer);
	memccpy(buff,buffer,1,bytes);
  return bytes;
}

int Socket::send(char* buff, int size, const Socket &sock)
{
	// { Asumamos que viene serializado ya :)
  // //Serializar el objeto
  // obj.to_bin();
	// }
  //Enviar el objeto binario a sock usando el socket sd
  // return sendto(sd,obj.data(),obj.size(),0,&sock.sa,sock.sa_len) == -1? -1:0;
  return sendto(sd,buff,size,0,&sock.sa,sock.sa_len) == -1? -1:0;
}

bool operator==(const Socket &s1, const Socket &s2){
    //Comparar los campos sin_family, sin_addr.s_addr y sin_port
    //de la estructura sockaddr_in de los Sockets s1 y s2
    //Retornar false si alguno difiere
    const sockaddr_in& sin1 = (const sockaddr_in&)(s1.sa);
    const sockaddr_in& sin2 = (const sockaddr_in&)(s2.sa);
    return (sin1.sin_addr.s_addr   == sin2.sin_addr.s_addr &&
            sin1.sin_family        == sin2.sin_family      &&
            sin1.sin_port          == sin2.sin_port         );
};

std::ostream &operator<<(std::ostream &os, const Socket &s)
{
  char host[NI_MAXHOST];
  char serv[NI_MAXSERV];

  getnameinfo((struct sockaddr *)&(s.sa), s.sa_len, host, NI_MAXHOST, serv,
              NI_MAXSERV, NI_NUMERICHOST);

  os << host << ":" << serv;

  return os;
};

extern "C"
{
	void* SOCKET_API make_socket(const char* add, const char* port){
		return new Socket(add, port);
	}

	int SOCKET_API test_socket(void* sock){
		return ((Socket*)sock)->test();
	}
	void SOCKET_API destroy_socket(void* sock){
		delete (Socket*)sock;
	}

	int SOCKET_API send_socket(void* sock,char* buff, int size, void* other){
	return ((Socket*)sock)->send(buff,size,(const Socket &)(*(Socket*)other));
	}
	
	int SOCKET_API recv_socket(void* sock, char*buff, int size)
	{
		Socket * s = 0;
		return ((Socket*)sock)->recv(buff,size,s);
	}

	void SOCKET_API bind_socket(void* sock)
	{
		((Socket*)sock)->bind();
	}

}
