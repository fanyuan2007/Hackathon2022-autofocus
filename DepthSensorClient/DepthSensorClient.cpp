// DepthSensorClient.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>
#include <iostream>
#include <string>
#include <math.h>
#include <stdio.h>
#include <fcntl.h>
#include <map>
#include <sstream>

#pragma comment (lib, "Ws2_32.lib")

#define DEFAULT_BUFLEN 256
#define DEFAULT_SIZE 100
#define PI 3.1415926535897

const std::string DepthDataFile = "C:\\ProgramData\\AutoFocus\\DepthSensor\\ds_db.json";
const std::string LensDataFile = "C:\\ProgramData\\AutoFocus\\DepthSensor\\lens_db.json";
const std::string LensDataStateFile = "C:\\ProgramData\\AutoFocus\\DepthSensor\\lens_state_db.json";


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

	char buf[256];
	GetCurrentDirectoryA(256, buf);
	std::string currentDir = std::string(buf) + '\\';
	std::cout << "currentDir: " << currentDir << std::endl;

	std::string JsonConverterPath = currentDir + "..\\Bin\\SaveDataToJson.exe";
	
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

	HANDLE serialHandle;

	serialHandle = CreateFileA("COM3",
		GENERIC_READ | GENERIC_WRITE,
		0,
		0,
		OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL,
		0);

	if (serialHandle == INVALID_HANDLE_VALUE) {
		if (GetLastError() == ERROR_FILE_NOT_FOUND) {
			//serial port does not exist. Inform user.
		}
	}

	DCB dcbSerialParams = { 0 };
	dcbSerialParams.DCBlength = sizeof(dcbSerialParams);
	if (!GetCommState(serialHandle, &dcbSerialParams)) {
		//error getting state
	}
	dcbSerialParams.BaudRate = CBR_9600;
	dcbSerialParams.ByteSize = 8;
	dcbSerialParams.StopBits = ONESTOPBIT;
	dcbSerialParams.Parity = NOPARITY;
	if (!SetCommState(serialHandle, &dcbSerialParams)) {
		//error setting serial port state
	}

	COMMTIMEOUTS timeouts = { 0 };
	timeouts.ReadIntervalTimeout = 50;
	timeouts.ReadTotalTimeoutConstant = 30;
	timeouts.ReadTotalTimeoutMultiplier = 10;
	if (!SetCommTimeouts(serialHandle, &timeouts)) {
		//error occureed. Inform user
	}

	//std::string distanceStr;
#define NUM_BYTES_IN_BUFFER 40

	std::map<int, int> stepperMap;
	//the are added to the created map
	stepperMap[3820] = 24;
	stepperMap[4090] = 18;
	stepperMap[4990] = 11;
	int stepperVal = 0;
	int stepperValPrev = 0;
	int stepperState = 0;

	int i = 0;
	int step = 10;
	long long int count = 0;
	int filesize = 500;
	std::string depthSensorData = "";
	std::string lensData = "";
	std::string lensStateData = "";
	
	//variables
	while (1)
	{
		Sleep(30);
		count++;
		char szBuff[NUM_BYTES_IN_BUFFER + 1] = { 0 };
		DWORD dwBytesRead = 0;
		if (!ReadFile(serialHandle, szBuff, NUM_BYTES_IN_BUFFER, &dwBytesRead, NULL)) {
			//error occurred. Report to user.
		}
		szBuff[NUM_BYTES_IN_BUFFER] = '\0';

		std::string strInput(szBuff);
		std::size_t pos1 = -1;
		pos1 = strInput.find(":");
		pos1 += 2;
		std::size_t pos2 = -1;
		pos2 = strInput.find("cm");

		if (pos1 < pos2) {
			std::string str3 = strInput.substr(pos1, pos2 - pos1);
			//std::cout << "str3: " << str3 << '\n';

			int distanceInCm;
			std::istringstream(str3) >> distanceInCm;
			//std::cout << "distanceInCm: " << distanceInCm << '\n';
			int distanceThreshInCm = 2;

			depthSensorData += std::to_string(distanceInCm) + ";";

			std::map<int, int>::iterator it;
			stepperVal = 0;
			lensData += std::to_string(stepperVal) + ";";
			lensStateData += std::to_string(stepperState) + ";";
			for (it = stepperMap.begin(); it != stepperMap.end(); it++)
			{
				int refDistance = it->second;
				int refState = it->first;

				
				bool isWithinRange = false;
				if (refState == 1)
				{
					if (distanceInCm < 14)
					{
						isWithinRange = true;
					}
				}
				else
				{
					if ((distanceInCm == refDistance) || (distanceInCm == (refDistance + 1)))
					{
						isWithinRange = true;
					}
				}

				if (isWithinRange)
				{
					stepperVal = it->first;

					if ((stepperVal == stepperValPrev) && (stepperVal != stepperState))
					{
						stepperState = stepperVal;

						std::string contentStr = std::to_string(stepperState);
						const char* sendbuf = contentStr.c_str();
						/*depthSensorData += contentStr + ";";
						lensData += contentStr + ";";*/

						int length = (int)strlen(sendbuf);
						//char recvbuf[DEFAULT_BUFLEN];
						int recvbuflen = DEFAULT_BUFLEN;

						if (count % filesize == 0)
						{
							count = 0;
							std::string commandDepth = JsonConverterPath + " " + DepthDataFile + " " + depthSensorData;
							system(commandDepth.c_str());
							std::string commandLens = JsonConverterPath + " " + LensDataFile + " " + lensData;
							system(commandLens.c_str());
							std::string commandLensState = JsonConverterPath + " " + LensDataStateFile + " " + lensStateData;
							system(commandLensState.c_str());
							depthSensorData = "";
							lensData = "";
							lensStateData = "";
						}

						// Print out the value to be sent
						//std::cout << "* send value: " << contentStr << ", length: " << length << std::endl;

						// send content to server
						result = send(ConnectSocket, sendbuf, length, 0);
						if (result == SOCKET_ERROR) {
							std::cout << "Send failed with error: " << WSAGetLastError() << std::endl;
							closesocket(ConnectSocket);
							WSACleanup();
							return 1;
						}

						std::cout << "stepperVal: " << stepperVal << '\n';
						std::cout << std::endl;
						break;
					}
					stepperValPrev = stepperVal;

				}
			}
		}
		

		//// receive the info back from server -- for testing purpose
		//result = recv(ConnectSocket, recvbuf, recvbuflen, 0);
		//if (result > 0)
		//{
		//	std::string recvValue;
		//	for (int i = 0; i < result; i++)
		//	{
		//		recvValue += recvbuf[i];
		//	}
		//	if (recvValue == "-1")
		//	{
		//		std::cout << "Wrong feedback received.\n";
		//	}
		//	else
		//	{
		//		std::cout << "The size received is: " << recvValue << std::endl;
		//	}
		//}
		//else
		//{
		//	std::cout << "Wrong feedback: " << result << std::endl;
		//}

		//Sleep(1000);
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
