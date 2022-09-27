// DepthSensorClient.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>
#include <iostream>
#include <string>
#include <math.h>

#pragma comment (lib, "Ws2_32.lib")

#define DEFAULT_BUFLEN 256
#define PI 3.1415926535897

int main()
{
	std::cout << "*****************    CLIENT    *****************\n\n";

	//Initialize Winsock
	WSADATA wsaData;
	int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (result != NO_ERROR) {
		std::cout << "WSAStartup failed with error: " << result << std::endl;
		return 1;
	}

	SOCKET ConnectSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (ConnectSocket == INVALID_SOCKET) {
		std::cout << "Error at socket(): " << WSAGetLastError() << std::endl;
		WSACleanup();
		return 1;
	}

	// The sockaddr_in structure specifies the address family,
	// IP address, and port for the socket that is being bound.
	sockaddr_in addrServer;
	addrServer.sin_family = AF_INET;
	InetPton(AF_INET, L"127.0.0.1", &addrServer.sin_addr.s_addr);
	addrServer.sin_port = htons(6666);
	memset(&(addrServer.sin_zero), '\0', 8);

	// Connect to server.
	std::cout << "Connecting..." << std::endl;
	result = connect(ConnectSocket, (SOCKADDR*)&addrServer, sizeof(addrServer));
	if (result == SOCKET_ERROR) {
		closesocket(ConnectSocket);
		std::cout << "Unable to connect to server: " << WSAGetLastError() << std::endl;
		WSACleanup();
		return 1;
	}

	int i = 0;
	int step = 10;
	//variables
	while (1)
	{
		// Prepare the data sending to the server
		std::string contentStr = std::to_string(cos((step*PI*i++)/180));
		const char* sendbuf = contentStr.c_str();

		int length = (int)strlen(sendbuf);
		char recvbuf[DEFAULT_BUFLEN];
		int recvbuflen = DEFAULT_BUFLEN;

		// Print out the value to be sent
		std::cout << "* send value: " << contentStr << ", length: " << length << std::endl;

		// send content to server
		result = send(ConnectSocket, sendbuf, length, 0);
		if (result == SOCKET_ERROR) {
			std::cout << "Send failed with error: " << WSAGetLastError() << std::endl;
			closesocket(ConnectSocket);
			WSACleanup();
			return 1;
		}

		// receive the info back from server -- for testing purpose
		result = recv(ConnectSocket, recvbuf, recvbuflen, 0);
		if (result > 0)
		{
			std::string recvValue;
			for (int i = 0; i < result; i++)
			{
				recvValue += recvbuf[i];
			}
			if (recvValue == "-1")
			{
				std::cout << "Wrong feedback received.\n";
			}
			else
			{
				std::cout << "The size received is: " << recvValue << std::endl;
			}
		}
		else
		{
			std::cout << "Wrong feedback: " << result << std::endl;
		}

		Sleep(1000);
	}

	// shutdown the connection since no more data will be sent
	result = shutdown(ConnectSocket, SD_SEND);
	if (result == SOCKET_ERROR) {
		printf("shutdown failed with error: %d\n", WSAGetLastError());
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
	}


	// cleanup
	closesocket(ConnectSocket);
	WSACleanup();

	return 0;
}
