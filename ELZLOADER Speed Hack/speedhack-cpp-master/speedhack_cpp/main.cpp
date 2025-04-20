#include "pch.h"
#include <Windows.h>

/*DWORD WINAPI UpdateAll(LPVOID lpParam)
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
                try
                {
                    double vOut = strtod(buffer, NULL);
                    CSpeedHack::instance().setSpeed(vOut);
                }
                catch (...)
                {

                }
            }
        }

        DisconnectNamedPipe(hPipe);
    }

    return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            auto result = CSpeedHack::instance().setup_hooks();

            if (!result)
            {
                return FALSE;
            }

            CreateThread(nullptr, 0, UpdateAll, nullptr, 0, nullptr);
            break;
        }

        case DLL_PROCESS_DETACH:
        {
            CSpeedHack::instance().remove_hooks();
            break;
        }
    }

    return TRUE;
}*/

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH)
    {
        auto result = CSpeedHack::instance().setup_hooks();

        if (!result)
        {
            return FALSE;
        }

        CSpeedHack::instance().setSpeed(1.7);
    }

    return TRUE;
}