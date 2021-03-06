#ifndef SOCKET_H_
#define SOCKET_H_

#ifdef _WIN32
#ifdef EXPORT_SOCKET
#define SOCKET_API __declspec(dllexport)
#else
#define SOCKET_API __declspec(dllimport)
#endif
#else
#define SOCKET_API
#endif

#ifdef _WIN32
/* See http://stackoverflow.com/questions/12765743/getaddrinfo-on-win32 */
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0501 /* Windows XP. */
#endif
#include <winsock2.h>
#include <Ws2tcpip.h>
using SOCKET_D = unsigned int;
#else
/* Assume that any non-Windows platform uses POSIX-style sockets instead. */
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netdb.h>	/* Needed for getaddrinfo() and freeaddrinfo() */
#include <unistd.h> /* Needed for close() */
using SOCKET_D = int;
#endif

// #include <sys/socket.h>
// #include <sys/types.h>

#include <iostream>
#include <stdexcept>

#include <ostream>

extern "C"
{
	// -----------------------------------------------------------------------------
	// Definiciones adelantadas
	// -----------------------------------------------------------------------------
	class SOCKET_API Socket;

	/**
	 *  Esta función compara dos Socks, realizando la comparación de las structuras
	 *  sockaddr: familia (INET), dirección y puerto, ver ip(7) para comparar
	 *  estructuras sockaddr_in. Deben comparar el tipo (sin_family), dirección
	 *  (sin_addr.s_addr) y puerto (sin_port). La comparación de los campos puede
	 *  realizarse con el operador == de los tipos básicos asociados.
	 */
	//bool operator== (const Socket &s1, const Socket &s2);

	/**
	 *  Imprime la dirección y puerto en número con el formato:"dirección_ip:puerto"
	 */
	std::ostream &operator<<(std::ostream &os, const Socket &dt);
	// -----------------------------------------------------------------------------
	// -----------------------------------------------------------------------------

	/**
	 * Clase base que representa el extremo local de una conexión UDP. Tiene la lógica
	 * para inicializar un sockect y la descripción binaria del extremo
	 *   - dirección IP
	 *   - puerto
	 */
	class SOCKET_API Socket
	{
	public:
		/**
			 * El máximo teórico de un mensaje UDP es 2^16, del que hay que
			 * descontar la cabecera UDP e IP (con las posibles opciones). Se debe
			 * utilizar esta constante para definir buffers de recepción.
			 */
		static const int32_t MAX_MESSAGE_SIZE = 32768;

		/**
			 *  Construye el socket UDP con la dirección y puerto dados. Esta función
			 *  usara getaddrinfo para obtener la representación binaria de dirección y
			 *  puerto.
			 *
			 *  Además abrirá el canal de comunicación con la llamada socket(2).
			 *
			 *    @param address cadena que representa la dirección o nombre
			 *    @param port cadena que representa el puerto o nombre del servicio
			 */
		Socket(const char *address, const char *port);

		/**
			 *  Inicializa un Socket copiando los parámetros del socket
			 */
		Socket(struct sockaddr *_sa, socklen_t _sa_len) : sd(-1), sa(*_sa),
														  sa_len(_sa_len){};

		virtual ~Socket()
		{
			int status = 0;
#ifdef _WIN32
			status = shutdown(sd, SD_BOTH);
			if (status == 0)
			{
				status = closesocket(sd);
			}
#else
			status = shutdown(sd, SHUT_RDWR);
			if (status == 0)
			{
				status = close(sd);
			}
#endif
		};

		/**
			 *  Recibe un mensaje de aplicación
			 *
			 *    @param obj que recibirá los datos de la red. Se usará para la
			 *    reconstrucción del objeto mediante Serializable::from_bin del interfaz.
			 *
			 *    @param sock que identificará al extremo que envía los datos si es
			 *    distinto de 0 se creará un objeto Socket con la dirección y puerto.
			 *
			 *    @return 0 en caso de éxito o -1 si error (cerrar conexión)
			 */
		int recv(char *buff, int size, Socket*& sock);

		/**
			 *  Envía un mensaje de aplicación definido por un objeto Serializable.
			 *
			 *    @param obj en el que se enviará por la red. La función lo serializará
			 *
			 *    @param sock con la dirección y puerto destino
			 *
			 *    @return 0 en caso de éxito o -1 si error
			 */
		int send(char *buff, int size, const Socket &sock);

		/**
			 *  Enlaza el descriptor del socket a la dirección y puerto
			 */
		int bind()
		{
			return ::bind(sd, (const struct sockaddr *)&sa, sa_len);
		}

		friend std::ostream &operator<<(std::ostream &os, const Socket &dt);

		//friend bool operator== (const Socket &s1, const Socket &s2);
	public:
		int test() { return 1; }

	protected:
		/**
			 *  Descriptor del socket
			 */
		SOCKET_D sd;

		/**
			 *  Representación binaria del extremo, usada por servidor y cliente
			 */
		struct sockaddr sa;
		socklen_t sa_len;
	};

	SOCKET_API void test(int a, int b);

	SOCKET_API void *make_socket(const char *add, const char *port);

	SOCKET_API int test_socket(void *sock);

	SOCKET_API void destroy_socket(void *sock);

	SOCKET_API int send_socket(void *sock, char *buff, int size, void *other);

	SOCKET_API int recv_socket(void *sock, char *buff, int size,void** s);

	SOCKET_API void bind_socket(void *sock);

	SOCKET_API int Init_Sockets(void);

	SOCKET_API int Quit_Sockets(void);
}

#endif /* SOCKET_H_ */
