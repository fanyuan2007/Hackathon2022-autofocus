// ArduinoReaderProj.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

// ArduinoReaderProj.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Windows.h>

int main()
{
    // Open serial port
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

    DWORD n = 16;
    while (1)
    {
        Sleep(10);
        char szBuff[17] = { 0 };
        DWORD dwBytesRead = 0;
        if (!ReadFile(serialHandle, szBuff, n, &dwBytesRead, NULL)) {
            //error occurred. Report to user.
        }
        szBuff[n] = '\0';

        std::cout << szBuff << std::endl;
    }

    CloseHandle(serialHandle);

}

