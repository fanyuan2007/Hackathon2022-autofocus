// ExecutableCallTest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Windows.h>
#include <string>

int main()
{
    std::cout << "Hello World!\n";

    // Get current directory
    char buf[256];
    GetCurrentDirectoryA(256, buf);
    std::string curDir = std::string(buf) + '\\';
    std::cout << "Current working directory: " << curDir << std::endl;

    // Execute the lens executable
    //std::string lensExe = "C:\\MyWork\\Dev\\test\\HackathonProj\\Hackathon2022-autofocus\\Bin\\ComputarAutoLens.exe";
    std::string lensExe = curDir + "..\\Bin\\ComputarAutoLens.exe";
    std::string command = lensExe + " 3000";
    system(command.c_str());

    char ch = getchar();
}


