#pragma once
#ifdef _WIN64
#define _AMD64_
#else
#define _X86_
#endif
#define WIN32_LEAN_AND_MEAN
#define EXTERN_API extern "C" _declspec(dllexport)
#define __typedef_func(func, ret, ...) using func##_t = ret(WINAPI*)(##__VA_ARGS__); func##_t m_##func = nullptr;
#define __typedef_func_winapi(func, ret, ...) using func##_t = ret(WINAPI*)(##__VA_ARGS__); func##_t m_##func = func##;
#define __setup_proc(func, proxy) m_##func = reinterpret_cast<func##_t>(::GetProcAddress(##proxy, #func));
#define __clear_proc(func) if(m_##func) m_##func = nullptr;
#include <minwindef.h>
#include <libloaderapi.h>
#include <sysinfoapi.h>
#include <profileapi.h>
#include <winternl.h>
#include <processthreadsapi.h>
#include <timeapi.h>
#include <string>
#include <fstream>
#include <filesystem>
#include <vector>
#include "speedhack.h"