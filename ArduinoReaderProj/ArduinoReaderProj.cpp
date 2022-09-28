// ArduinoReaderProj.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Windows.h>
#include <map>
#include <string>
#include <sstream>

using namespace std;

 int main()
{
    // Open serial port
    HANDLE serialHandle;

    serialHandle = CreateFileA("COM5",
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

	map<int, int> stepperMap;
	//the are added to the created map
	stepperMap[3820] = 24;
	stepperMap[4090] = 18;
	stepperMap[4990] = 11;
	//stepperMap[3] = 24;
	//stepperMap[2] = 18;
	//stepperMap[1] = 11;
	int stepperVal = 0;
	int stepperValPrev = 0;
	int stepperState = 0;

    while (1)
    {
		Sleep(10);
		char szBuff[NUM_BYTES_IN_BUFFER + 1] = { 0 };
		DWORD dwBytesRead = 0;
		if (!ReadFile(serialHandle, szBuff, NUM_BYTES_IN_BUFFER, &dwBytesRead, NULL)) {
			//error occurred. Report to user.
		}
		szBuff[NUM_BYTES_IN_BUFFER] = '\0';
	
		string strInput(szBuff);	
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

			map<int, int>::iterator it;
			stepperVal = 0;
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

				if(isWithinRange)
				{
					stepperVal = it->first;

					if ((stepperVal == stepperValPrev) && (stepperVal != stepperState))
					{
						stepperState = stepperVal;
						std::cout << "stepperVal: " << stepperVal << '\n';
						std::cout << std::endl;
						break;
					}
					stepperValPrev = stepperVal;
					
				}
			}
		}
	}

    CloseHandle(serialHandle);
}

