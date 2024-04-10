// MemTest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <windows.h>

unsigned long long getTotalSystemMemory()
{
	MEMORYSTATUSEX status;
	status.dwLength = sizeof(status);
	GlobalMemoryStatusEx(&status);
	return status.ullTotalPhys;
}

int main()
{
    // allocate ram until we run out of memory
	// if it runs out of memory, it will throw a std::bad_alloc exception
	// we can catch this exception and handle it
	while (true)
	{
		int* p = new int[1000000];
		if (p == nullptr)
		{
			std::cout << "Out of memory" << std::endl;
			break;
		}
		else
		{
			std::cout << "Total system memory: " << getTotalSystemMemory() << std::endl;
		}
	}
	return 0;
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
