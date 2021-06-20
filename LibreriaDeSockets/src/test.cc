#include "iostream"
#include "test.h"

#include <string.h>
#include <errno.h>

void handleGAIError(const char *s, int err)
{
	printf("Some error with, Ricky talking from dll land %s\n", s);
	fprintf(stderr, "%s\n", gai_strerror(err));
	exit(1);
}
void handleError(const char *s)
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
	hints.ai_family = AF_INET;	 // IPv4
	hints.ai_socktype = SOCK_DGRAM;
	int rc;

	if ((rc = getaddrinfo(address, port, &hints, &result)) != 0)
		handleGAIError("getaddrinfo", rc);
	if ((sd = socket(result->ai_family, result->ai_socktype, 0)) < 0)
		handleError("socket");
	sa = *((struct sockaddr *)result->ai_addr);
	sa_len = result->ai_addrlen;
}

int Socket::recv(char *buff, int size, Socket*& sock)
{
	struct sockaddr sa;
	socklen_t sa_len = sizeof(struct sockaddr);

	char buffer[MAX_MESSAGE_SIZE];
	int bytes = ::recvfrom(sd, buffer, MAX_MESSAGE_SIZE, 0, &sa, &sa_len);

	if (bytes <= 0)
	{
		return (errno == EWOULDBLOCK || errno == EAGAIN) ? 0 : -1;
	}

	if(sock != NULL)
		sock = new Socket(&sa, sa_len);

	memcpy(buff, buffer, bytes);
	return bytes;
}

int Socket::send(char *buff, int size, const Socket &sock)
{
	return sendto(sd, buff, size, 0, &sock.sa, sock.sa_len) == -1 ? -1 : 0;
}


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
	void test(int a, int b)
	{
		printf("Test from c++ %d + %d = %d\n", a, b, a + b);
	}

	void *make_socket(const char *add, const char *port)
	{
		return new Socket(add, port);
	}

	int test_socket(void *sock)
	{
		return ((Socket *)sock)->test();
	}
	void destroy_socket(void *sock)
	{
		delete (Socket *)sock;
	}

	int send_socket(void *sock, char *buff, int size, void *other)
	{
		return ((Socket *)sock)->send(buff, size, (const Socket &)(*(Socket *)other));
	}

	int recv_socket(void *sock, char *buff, int size, void** s)
	{
		return ((Socket *)sock)->recv(buff, size, (Socket*&)(*s));
	}

	void bind_socket(void *sock)
	{
		((Socket *)sock)->bind();
	}

	int Init_Sockets(void)
	{
#ifdef _WIN32
		WSADATA wsa_data;
		return WSAStartup(MAKEWORD(1, 1), &wsa_data);
#else
		return 0;
#endif
	}

	int Quit_Sockets(void)
	{
#ifdef _WIN32
		return WSACleanup();
#else
		return 0;
#endif
	}
}
