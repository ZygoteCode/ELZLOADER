using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;

public class SpeedHackManager
{
    public static string pipeName = "ELZLOADERSpeedHackCommunicationPipe";
    public static NamedPipeClientStream Client;
    public static Process Elsword;

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

    const int PROCESS_CREATE_THREAD = 0x0002;
    const int PROCESS_QUERY_INFORMATION = 0x0400;
    const int PROCESS_VM_OPERATION = 0x0008;
    const int PROCESS_VM_WRITE = 0x0020;
    const int PROCESS_VM_READ = 0x0010;

    const uint MEM_COMMIT = 0x00001000;
    const uint MEM_RESERVE = 0x00002000;
    const uint PAGE_READWRITE = 4;

    public static void InjectDLL(int processId, string path)
    {
        IntPtr procHandle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, processId);
        IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
        IntPtr allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((path.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
        UIntPtr bytesWritten;
        WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(path), (uint)((path.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten);
        CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
    }

    public static void SetSpeed()
    {
        Elsword = Process.GetProcessesByName("x2")[0];

        while (!Elsword.Responding)
        {
            Thread.Sleep(5);
        }

        Client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
        Client.Connect(Int32.MaxValue);

        while (!Elsword.Responding)
        {
            Thread.Sleep(5);
        }

        byte[] bytes = Encoding.UTF8.GetBytes(Utils.SpeedHackValue);
        Client.Write(bytes, 0, bytes.Length);
    }
}