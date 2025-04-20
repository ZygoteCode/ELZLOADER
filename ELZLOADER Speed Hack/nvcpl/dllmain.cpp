#include "pch.h"
#include <Windows.h>
#pragma comment(lib, "detours.lib")

void InitDLL(LPVOID hModule)
{

}

DWORD WINAPI MainThread(LPVOID lpParam)
{
	HANDLE hPipe;
	char buffer[1024];
	DWORD dwRead;
	hPipe = CreateNamedPipe(TEXT("\\\\.\\pipe\\ELZLOADERSpeedHackCommunicationPipe"), PIPE_ACCESS_DUPLEX, PIPE_TYPE_BYTE | PIPE_READMODE_BYTE | PIPE_WAIT, 1, 1024 * 16, 1024 * 16, NMPWAIT_USE_DEFAULT_WAIT, NULL);

	while (hPipe != INVALID_HANDLE_VALUE)
	{
		if (ConnectNamedPipe(hPipe, NULL) != FALSE)
		{
			while (ReadFile(hPipe, buffer, sizeof(buffer) - 1, &dwRead, NULL) != FALSE)
			{
				/*int readNumber = 0;
				sscanf(buffer, "%02lf", &readNumber);

				*/
			}
		}

		DisconnectNamedPipe(hPipe);
	}

	return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	if (ul_reason_for_call == DLL_PROCESS_ATTACH)
	{
		CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)MainThread, (LPVOID)hModule, 0, NULL);
		CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)InitDLL, (LPVOID)hModule, 0, NULL);
	}

	return TRUE;
}