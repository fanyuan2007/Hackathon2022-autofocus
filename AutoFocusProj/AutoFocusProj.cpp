// AutoFocusProj.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>
#include <iostream>
#include <string>
#include <fstream>

#pragma comment (lib, "Ws2_32.lib")

#define DEFAULT_BUFLEN 256

int main()
{
	std::cout << "*****************    SERVER    *****************\n\n";

	char buf[256];
	GetCurrentDirectoryA(256, buf);
	std::string currentDir = std::string(buf) + '\\';
	std::cout << "currentDir: " << currentDir << std::endl;

	//char str[INET_ADDRSTRLEN];

	//Initialize Winsock
	WSADATA wsaData;
	int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (result != NO_ERROR) {
		std::cout << "WSAStartup failed with error: " << result << std::endl;
		return 1;
	}


	//Create a SOCKET for listening for incoming connections request
	SOCKET ListenSocket, ClientSocket;
	ListenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (ListenSocket == INVALID_SOCKET) {
		std::cout << "Socket failed with error: " << WSAGetLastError() << std::endl;
		WSACleanup();
		return 1;
	}


	//The sockaddr_in structure specifies the address family,
	//IP address, and port for the socket that is being bound
	sockaddr_in addrServer;
	addrServer.sin_family = AF_INET;
	InetPton(AF_INET, L"127.0.0.1", &addrServer.sin_addr.s_addr);
	addrServer.sin_port = htons(6666);
	memset(&(addrServer.sin_zero), '\0', 8);

	//Bind socket
	if (bind(ListenSocket, (SOCKADDR*)&addrServer, sizeof(addrServer)) == SOCKET_ERROR) {
		std::cout << "Bind failed with error: " << WSAGetLastError() << std::endl;
		closesocket(ListenSocket);
		WSACleanup();
		return 1;
	}

	//Listen for incomin connection requests on the created socket
	if (listen(ListenSocket, 5) == SOCKET_ERROR) {
		std::cout << "Listen failed with error: " << WSAGetLastError() << std::endl;
		closesocket(ListenSocket);
		WSACleanup();
		return 1;
	}

	// Accept a client socket
	ClientSocket = accept(ListenSocket, NULL, NULL);
	if (ClientSocket == INVALID_SOCKET) {
		std::cout << "Accept failed with error: " << WSAGetLastError() << std::endl;
		closesocket(ListenSocket);
		WSACleanup();
		return 1;
	}

	

	while (1)
	{
		//Variables for recieve
		int iSendResult;
		char recvbuf[DEFAULT_BUFLEN];
		int recvbuflen = DEFAULT_BUFLEN;

		// Step 1: Receive the depth information from the client
		result = recv(ClientSocket, recvbuf, recvbuflen, 0);
		if (result > 0)
		{
			std::string recvContent;
			for (int i = 0; i < result; ++i)
			{
				recvContent += recvbuf[i];
			}
			std::cout << "content received: " << recvContent << std::endl;

			const char* sendbuf = recvContent.c_str();
			// Send file size to client
			iSendResult = send(ClientSocket, sendbuf, (int)strlen(sendbuf), 0);
			if (iSendResult == SOCKET_ERROR) {
				std::cout << "Send failed with error: " << WSAGetLastError() << std::endl;
				closesocket(ClientSocket);
				WSACleanup();
				return 1;
			}

			// Step 2: Update the lens' focus value with calling the C# executable
			std::string lensExePath = currentDir + "..\\Bin\\ComputarAutoLens.exe";
			std::string command = lensExePath + " 3000"; // The value 3000 here needs change
			system(command.c_str());


			// Step 3: Wait some time here, and do image acquisition, save the image to some place
			// - May also save the other information in the same folder here?


			// Step 4: Do barcode/QR code image processing

		}
		else if (result == 0)
		{
			// No information sent from client
		}
		else
		{
			std::cout << "Recieve failed with error: " << WSAGetLastError() << std::endl;
			closesocket(ClientSocket);
			WSACleanup();
			return 1;
		}
		Sleep(1000);
	}
	
	std::cout << "Connection closing...\n" << std::endl;
	int ch = getchar();

	//Shutdown the connection since we're done
	result = shutdown(ClientSocket, SD_SEND);
	if (result == SOCKET_ERROR) {
		printf("shutdown failed with error: %d\n", WSAGetLastError());
		closesocket(ClientSocket);
		WSACleanup();
		return 1;
	}

	// cleanup
	closesocket(ClientSocket);
	WSACleanup();

	return 0;
}
