#include "pch.h"

/*DWORD WINAPI CSpeedHack::hook_GetTickCount()
{
	return instance().set_tick_count();
}

ULONGLONG WINAPI CSpeedHack::hook_GetTickCount64()
{
	return instance().set_tick_count64();
}

DWORD WINAPI CSpeedHack::hook_timeGetTime()
{
	return instance().set_time_get_time();
}*/

BOOL WINAPI CSpeedHack::hook_QueryPerformanceCounter( LARGE_INTEGER* lpPerformanceCount)
{
	instance().set_performance_counter(lpPerformanceCount);
	return TRUE;
}