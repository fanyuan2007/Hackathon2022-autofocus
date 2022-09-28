// ExecutableCallTest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <Windows.h>
#include <string>

const std::string IMAGE_FOLDER = "C:\\ProgramData\\AutoFocus\\ImageProc\\";
const std::string HISTORY_IMAGE_FOLDER = "C:\\ProgramData\\AutoFocus\\ImageProc\\History\\";

int main()
{
    std::cout << "Hello World!\n";

    // Get current directory
    char buf[256];
    GetCurrentDirectoryA(256, buf);
    std::string curDir = std::string(buf) + '\\';
    std::cout << "Current working directory: " << curDir << std::endl;

    //// Execute the lens executable
    ////std::string lensExe = "C:\\MyWork\\Dev\\test\\HackathonProj\\Hackathon2022-autofocus\\Bin\\ComputarAutoLens.exe";
    //std::string lensExe = curDir + "..\\Bin\\ComputarAutoLens.exe";
    //std::string command = lensExe + " 3000";
    //system(command.c_str());


	// Execute the image acquisition
	std::string ImageAcquisitionPath = curDir + "..\\Bin\\ImageAcquisition\\Acquisition_CSharpd_v140.exe";
	std::string ImageSavingPath = IMAGE_FOLDER + "currentImage_" + std::to_string(0) + ".jpg";
	std::string ImageAcqCommand = ImageAcquisitionPath + " " + ImageSavingPath;
	system(ImageAcqCommand.c_str());

    char ch = getchar();
}


