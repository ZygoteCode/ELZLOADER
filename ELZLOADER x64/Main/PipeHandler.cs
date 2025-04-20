using System.IO.Pipes;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System;

public static class PipeHandler
{
	public static string pipeName = "";
	public static NamedPipeClientStream Client;
	public static Process Elsword;

	public static void PipeConnect()
	{
		Client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
		Client.Connect(Int32.MaxValue);
        Elsword = Process.GetProcessesByName("x2")[0];
    }

    public static void PipeAuth(string PipeKey, string PipeCredentials, string root)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(PipeKey);
		byte[] bytes2 = Encoding.UTF8.GetBytes(PipeCredentials);
		byte[] bytes3 = Encoding.UTF8.GetBytes(root);

		Client.Write(bytes, 0, bytes.Length);
		Client.Write(bytes2, 0, bytes2.Length);
		Client.Write(bytes3, 0, bytes3.Length);
	}

	public static void PipeSend(string data)
	{
		while (!Elsword.Responding)
		{
			Thread.Sleep(5);
		}

		byte[] bytes = Encoding.UTF8.GetBytes(data);
		Client.Write(bytes, 0, bytes.Length);
	}

	public static void ExecuteLua(string data)
	{
		PipeSend($"pcall(function()\r\n{data}\r\nend)");
	}
}