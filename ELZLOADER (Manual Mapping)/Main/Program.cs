using System;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using System.Reflection;

internal static class Program
{
    public static int STATE_1 = -1;
    public static byte[] BYTE_ARRAY_1 = null;
    public static string STRING_1 = "";

    [STAThread]

    static void Main(string[] args)
    {
        try
        {
            if (!System.IO.Directory.Exists("autorun"))
            {
                System.IO.Directory.CreateDirectory("autorun");
            }

            string theArgs = "";

            foreach (string arg in args)
            {
                if (theArgs == "")
                {
                    theArgs = arg;
                }
                else
                {
                    theArgs = theArgs + " " + arg;
                }
            }

            if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("You must run the program with Administrator privileges!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
                return;
            }

            string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":";

            if (System.IO.File.Exists(Application.StartupPath + "\\credentials.elz"))
            {
                System.IO.File.Move(Application.StartupPath + "\\credentials.elz", rootDir + "\\credentials.elz");
            }

            if (!ValidateArgs(theArgs))
            {
                if (!System.IO.File.Exists(Application.StartupPath + "\\ELZLOADER.exe"))
                {
                    MessageBox.Show("You must run the program with Administrator privileges!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                ProtoRandom random = new ProtoRandom(5);
                char[] characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
                string folderName = random.GetRandomString(characters, random.GetRandomInt32(16, 32));
                System.IO.Directory.CreateDirectory(rootDir + "\\Temp\\" + folderName);
                string executableName = random.GetRandomString(characters, random.GetRandomInt32(8, 48)) + ".exe";
                System.IO.File.Copy(Application.StartupPath + "\\ELZLOADER.exe", rootDir + "\\Temp\\" + folderName + "\\" + executableName);
                HideFile(rootDir + "\\Temp\\" + folderName + "\\" + executableName);
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(rootDir + "\\Temp\\" + folderName);
                info.Attributes = System.IO.FileAttributes.Hidden | System.IO.FileAttributes.Directory | System.IO.FileAttributes.ReadOnly;

                if (System.IO.File.Exists("FastColoredTextBox.dll"))
                {
                    System.IO.File.Copy(Application.StartupPath + "\\FastColoredTextBox.dll", rootDir + "\\Temp\\" + folderName + "\\FastColoredTextBox.dll");
                    HideFile(rootDir + "\\Temp\\" + folderName + "\\FastColoredTextBox.dll");
                }

                if (System.IO.File.Exists("Guna.UI.dll"))
                {
                    System.IO.File.Copy(Application.StartupPath + "\\Guna.UI.dll", rootDir + "\\Temp\\" + folderName + "\\Guna.UI.dll");
                    HideFile(rootDir + "\\Temp\\" + folderName + "\\Guna.UI.dll");
                }

                if (System.IO.File.Exists("MetroSuite 2.0.dll"))
                {
                    System.IO.File.Copy(Application.StartupPath + "\\MetroSuite 2.0.dll", rootDir + "\\Temp\\" + folderName + "\\MetroSuite 2.0.dll");
                    HideFile(rootDir + "\\Temp\\" + folderName + "\\MetroSuite 2.0.dll");
                }

                if (System.IO.File.Exists("Siticone.UI.dll"))
                {
                    System.IO.File.Copy(Application.StartupPath + "\\Siticone.UI.dll", rootDir + "\\Temp\\" + folderName + "\\Siticone.UI.dll");
                    HideFile(rootDir + "\\Temp\\" + folderName + "\\Siticone.UI.dll");
                }

                if (System.IO.File.Exists("websocket-sharp.dll"))
                {
                    System.IO.File.Copy(Application.StartupPath + "\\websocket-sharp.dll", rootDir + "\\Temp\\" + folderName + "\\websocket-sharp.dll");
                    HideFile(rootDir + "\\Temp\\" + folderName + "\\websocket-sharp.dll");
                }

                if (System.IO.Directory.Exists("autorun"))
                {
                    System.IO.Directory.CreateDirectory(rootDir + "\\Temp\\" + folderName + "\\autorun");

                    foreach (string file in System.IO.Directory.GetFiles("autorun"))
                    {
                        System.IO.File.WriteAllText(rootDir + "\\Temp\\" + folderName + "\\autorun\\" + System.IO.Path.GetFileNameWithoutExtension(file) + ".lua", System.IO.File.ReadAllText(file));
                    }
                }

                long timestamp = Utils.GetTimestamp();
                byte[] timestampBuffer = BitConverter.GetBytes(timestamp);
                byte[] timestampHash = System.Security.Cryptography.MD5.Create().ComputeHash(timestampBuffer);
                byte[] theBuffer = new byte[1] { 0xC1 };

                theBuffer = Combine(theBuffer, timestampHash);
                theBuffer = Combine(theBuffer, timestampBuffer);
                theBuffer = Utils.EncryptAES256(theBuffer, "ELZL_ERKKEJRLKJEWKRJ_237123612");

                ExecuteAsAdmin(rootDir + "\\Temp\\" + folderName + "\\" + executableName, Convert.ToBase64String(theBuffer));
                Process.GetCurrentProcess().Kill();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin());
        }
        catch (Exception ex)
        {
            CrashReporting.WriteCrashReport("Detection 100, failed to open the program", ex);
            Process.GetCurrentProcess().Kill();
        }
    }

    private static void HideFile(string file)
    {
        System.IO.File.SetAttributes(file, System.IO.FileAttributes.Hidden);
        System.IO.FileInfo info = new System.IO.FileInfo(file);
        info.IsReadOnly = true;
    }

    private static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];

        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

        return ret;
    }

    private static void ExecuteAsAdmin(string fileName, string arguments)
    {
        Process proc = new Process();
        proc.StartInfo.FileName = fileName;
        proc.StartInfo.Arguments = arguments;
        proc.StartInfo.UseShellExecute = true;
        proc.StartInfo.Verb = "runas";
        proc.Start();
    }

    private static bool ValidateArgs(string arguments)
    {
        if (arguments == null)
        {
            return false;
        }

        if (arguments.Replace(" ", "").Replace('\t'.ToString(), "") == "")
        {
            return false;
        }

        byte[] theBuffer = Convert.FromBase64String(arguments);
        theBuffer = Utils.DecryptAES256(theBuffer, "ELZL_ERKKEJRLKJEWKRJ_237123612");

        byte[] firstByte = theBuffer.Take(1).ToArray();
        theBuffer = theBuffer.Skip(1).ToArray();

        byte[] hash = theBuffer.Take(16).ToArray();
        theBuffer = theBuffer.Skip(16).ToArray();

        byte[] timestampBuffer = theBuffer.Take(8).ToArray();

        long timestamp = BitConverter.ToInt64(timestampBuffer, 0);
        byte[] realHash = System.Security.Cryptography.MD5.Create().ComputeHash(timestampBuffer);

        if (!CompareByteArrays(hash, realHash))
        {
            return false;
        }

        if (firstByte[0] != 0xC1)
        {
            return false;
        }

        long timestampNow = Utils.GetTimestamp() - timestamp;

        if (timestampNow < 0 || timestampNow > 4)
        {
            return false;
        }

        return true;
    }

    private static bool CompareByteArrays(byte[] first, byte[] second)
    {
        if (first.Length != second.Length)
        {
            return false;
        }

        for (int i = 0; i < first.Length; i++)
        {
            if (first[i] != second[i])
            {
                return false;
            }
        }

        return true;
    }

    private static byte[] Compress(byte[] data)
    {
        using (var compressedStream = new MemoryStream())
        {
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }
    }

    private static byte[] Decompress(byte[] data)
    {
        using (var compressedStream = new MemoryStream(data))
        {
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                using (var resultStream = new MemoryStream())
                {
                    zipStream.CopyTo(resultStream);
                    return resultStream.ToArray();
                }
            }
        }
    }
}