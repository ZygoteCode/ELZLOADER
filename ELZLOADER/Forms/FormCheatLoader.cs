using System.Windows.Forms;
using MetroSuite;
using System.Diagnostics;
using System.Threading;
using System;
using System.Security.Principal;
using System.Text;
using System.Linq;
using System.Management;
using System.IO;

public partial class FormCheatLoader : MetroForm
{
    private string thingFolderName = "";
    private string rootDir = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":";
    private string folderName, nvcplDllName, luaCompDllName, pipeName, speedHackDllName;
    private bool canDo = false;
    private ProtoRandom random = new ProtoRandom(100);
    private char[] characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    private void timer1_Tick(object sender, EventArgs e)
    {
        if (canDo)
        {
            canDo = false;
            timer1.Stop();
            Process theProcess = null;

            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (p.ProcessName.ToLower().Equals("x2"))
                    {
                        theProcess = p;
                        break;
                    }
                }
                catch
                {

                }
            }

            if (theProcess == null)
            {
                CrashReporting.WriteCrashReport("Detection 20, Elsword closed");
                Process.GetCurrentProcess().Kill();
                return;
            }

            while (!theProcess.Responding)
            {
                Thread.Sleep(5);
            }

            Thread.Sleep(1500);
            new FormLuaInjector().Show();
            this.Hide();
            this.Size = new System.Drawing.Size(0, 0);
            this.Visible = false;
            this.Enabled = false;
            this.Opacity = 0;
        }
    }

    private void FormCheatLoader_FormClosing(object sender, FormClosingEventArgs e)
    {
        Process.GetCurrentProcess().Kill();
    }

    public int FindBytes(byte[] src, byte[] find)
    {
        int index = -1;
        int matchIndex = 0;

        for (int i = 0; i < src.Length; i++)
        {
            if (src[i] == find[matchIndex])
            {
                if (matchIndex == (find.Length - 1))
                {
                    index = i - matchIndex;
                    break;
                }

                matchIndex++;
            }
            else if (src[i] == find[0])
            {
                matchIndex = 1;
            }
            else
            {
                matchIndex = 0;
            }
        }

        return index;
    }

    public byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
    {
        byte[] dst = null;
        int index = FindBytes(src, search);

        if (index >= 0)
        {
            dst = new byte[src.Length - search.Length + repl.Length];

            Buffer.BlockCopy(src, 0, dst, 0, index);
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            Buffer.BlockCopy(src, index + search.Length, dst, index + repl.Length, src.Length - (index + search.Length));
        }

        return dst;
    }

    public bool IsPasswordValid(string str)
    {
        foreach (char c in str)
        {
            bool found = false;

            foreach (char s in characters)
            {
                if (c.Equals(s))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
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

    public FormCheatLoader()
    {
        try
        {
            try
            {
                if (!(Utils.STRING_1.Length >= 375 && Utils.STRING_1.Length <= 963) || !IsPasswordValid(Utils.STRING_1))
                {
                    CrashReporting.WriteCrashReport("Detection 21, Illegal violation");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                if (!CompareByteArrays(Utils.BYTE_ARRAY_1, Utils.EncryptAES256(Encoding.UTF8.GetBytes(HWIDUtils.GetBIOSID() + HWIDUtils.GetDeviceID()), "JK32J4L32J43H4J12B3NMQKWEKJQWEKOWEN1MN")))
                {
                    CrashReporting.WriteCrashReport("Detection 22, Illegal violation");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                if (!(Program.STATE_1 >= 23 && Program.STATE_1 <= 46))
                {
                    CrashReporting.WriteCrashReport("Detection 23, Illegal violation");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                if (!CompareByteArrays(Program.BYTE_ARRAY_1, Utils.EncryptAES256(Encoding.UTF8.GetBytes(HWIDUtils.GetBIOSID() + HWIDUtils.GetDeviceID()), "LWEKRJWEKRJLKWEJRLKJWERLKJEWLKRJE")))
                {
                    CrashReporting.WriteCrashReport("Detection 24, Illegal violation");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                if (Program.STRING_1.Length != 811 || !IsPasswordValid(Program.STRING_1))
                {
                    CrashReporting.WriteCrashReport("Detection 25, Illegal violation");
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }
            catch
            {
                CrashReporting.WriteCrashReport("Detection 26, Illegal violation");
                Process.GetCurrentProcess().Kill();
                return;
            }

            InitializeComponent();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            CheckForIllegalCrossThreadCalls = false;

            if (!System.IO.Directory.Exists(rootDir + "\\Temp"))
            {
                System.IO.Directory.CreateDirectory(rootDir + "\\Temp");
            }

            folderName = random.GetRandomString(characters, random.GetRandomInt32(32, 64));

            if (System.IO.Directory.Exists(rootDir + "\\Temp\\" + folderName))
            {
                System.IO.Directory.Delete(rootDir + "\\Temp\\" + folderName);
            }

            foreach (string file in System.IO.Directory.GetFiles(rootDir + "\\"))
            {
                try
                {
                    if (System.IO.Path.GetExtension(file).ToLower().Equals(".dll") && System.IO.Path.GetFileNameWithoutExtension(file).Length == 7)
                    {
                        System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
                        System.IO.File.Delete(file);
                    }
                }
                catch
                {

                }
            }

            System.IO.Directory.CreateDirectory(rootDir + "\\Temp\\" + folderName);

            nvcplDllName = random.GetRandomString(characters, random.GetRandomInt32(32, 64));
            luaCompDllName = random.GetRandomString(characters, 7);
            pipeName = random.GetRandomString(characters, 21);
            PipeHandler.pipeName = pipeName;

            byte[] nvcpl = ELZLOADER.Properties.Resources.LuaLoaderDLL;

            nvcpl = ReplaceBytes(nvcpl, System.Text.Encoding.UTF8.GetBytes("luacomp.dll"), System.Text.Encoding.UTF8.GetBytes(luaCompDllName + ".dll"));
            nvcpl = ReplaceBytes(nvcpl, System.Text.Encoding.UTF8.GetBytes("elsword.exe"), System.Text.Encoding.UTF8.GetBytes(random.GetRandomString(characters, 7) + ".exe"));
            nvcpl = ReplaceBytes(nvcpl, System.Text.Encoding.UTF8.GetBytes("ahjrjhwehjqehqjehreje"), System.Text.Encoding.UTF8.GetBytes(pipeName));

            string[] strings = new string[]
            {
                "aaaaaaaaaa attached!",
                "aaaaaaaaaa",
                "Lua initialization failed.",
                "Internal error.",
                "Injection error.",
                "Unable to open communication pipe.",
                "Unable to connect to communication pipe.",
                "Pipe read error.",
                "Cracking attempt logged.",
                "LunaLoader startup failed.",
                "Lua init error."
            };

            foreach (string str in strings)
            {
                nvcpl = ReplaceBytes(nvcpl, System.Text.Encoding.UTF8.GetBytes(str), System.Text.Encoding.UTF8.GetBytes(random.GetRandomString(characters, str.Length)));
            }

            byte[] newthing = Combine(nvcpl, System.Text.Encoding.UTF8.GetBytes(random.GetRandomString(characters, 250)));
            byte[] newthing1 = Combine(ELZLOADER.Properties.Resources.LuaCompilerDLL, System.Text.Encoding.UTF8.GetBytes(random.GetRandomString(characters, 250)));
            newthing1 = ReplaceBytes(newthing1, System.Text.Encoding.UTF8.GetBytes("LuaJIT 2.0.2"), System.Text.Encoding.UTF8.GetBytes(random.GetRandomString(characters, 12)));
            newthing1 = ReplaceBytes(newthing1, System.Text.Encoding.UTF8.GetBytes("CPU not supported"), System.Text.Encoding.UTF8.GetBytes(random.GetRandomString(characters, 17)));

            System.IO.File.WriteAllBytes(rootDir + "\\Temp\\" + folderName + "\\" + nvcplDllName + ".dll", newthing);
            System.IO.File.WriteAllBytes(rootDir + "\\" + luaCompDllName + ".dll", newthing1);

            speedHackDllName = random.GetRandomString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(), random.GetRandomInt32(6, 17));
            System.IO.File.WriteAllBytes(rootDir + "\\Temp\\" + speedHackDllName + ".dll", ELZLOADER.Properties.Resources.SpeedHack);

            HideFile(rootDir + "\\Temp\\" + folderName + "\\" + nvcplDllName + ".dll");
            HideFile(rootDir + "\\" + luaCompDllName + ".dll");
            HideFile(rootDir + "\\Temp\\" + speedHackDllName + ".dll");

            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(rootDir + "\\Temp\\" + folderName);
            info.Attributes = System.IO.FileAttributes.Hidden | System.IO.FileAttributes.Directory | System.IO.FileAttributes.ReadOnly;

            try
            {
                foreach (string file in System.IO.Directory.GetFiles(rootDir + "\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Microsoft\\Windows\\Recent"))
                {
                    try
                    {
                        if (file.ToLower().Contains("elzloader") || file.ToLower().Contains("skype") || file.ToLower().Contains("x2") || file.ToLower().Contains("elsword"))
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }

            timer1.Start();

            Thread thread = new Thread(Waiting);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            siticoneCheckBox1.Visible = Utils.AllowMultiClient;
            siticoneCheckBox1.Enabled = Utils.AllowMultiClient;
        }
        catch (Exception ex)
        {
            CrashReporting.WriteCrashReport("Detection 27, Illegal violation");
            Process.GetCurrentProcess().Kill();
            return;
        }
    }

    public static void ExecuteAsAdmin(string fileName, string arguments)
    {
        Process proc = new Process();
        proc.StartInfo.FileName = fileName;
        proc.StartInfo.Arguments = arguments;
        proc.StartInfo.UseShellExecute = true;
        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.Verb = "runas";
        proc.Start();
        proc.WaitForExit();
    }

    public static void HideFile(string file)
    {
        System.IO.File.SetAttributes(file, System.IO.FileAttributes.Hidden);
        System.IO.FileInfo info = new System.IO.FileInfo(file);
        info.IsReadOnly = true;
    }

    private void siticoneCheckBox9_CheckedChanged(object sender, EventArgs e)
    {
        Utils.SpeedHackEnabled = siticoneCheckBox9.Checked;
    }

    private void siticoneCheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (Utils.AllowMultiClient)
        {
            Utils.MultiClient = siticoneCheckBox1.Checked;
        }
    }

    private void gunaLineTextBox1_TextChanged(object sender, EventArgs e)
    {
        Utils.SpeedHackValue = gunaLineTextBox1.Text;
    }
    public static void ShowFile(string file)
    {
        System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
        System.IO.FileInfo info = new System.IO.FileInfo(file);
        info.IsReadOnly = false;
    }

    public static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];

        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

        return ret;
    }

    public void Waiting()
    {
        bool canContinue = true;

        while (canContinue)
        {
            try
            {
                if (label1.Text.Equals("Waiting for the launcher..."))
                {
                    foreach (Process process in Process.GetProcesses())
                    {
                        try
                        {
                            if (process.ProcessName.ToLower().Equals("elsword") || process.ProcessName.ToLower().Equals("elsrift"))
                            {
                                label1.Text = "Waiting for the game...";
                                break;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else if (label1.Text.Equals("Waiting for the game..."))
                {
                    foreach (Process process in Process.GetProcesses())
                    {
                        try
                        {
                            if (process.ProcessName.ToLower().Equals("x2"))
                            {
                                int processId = process.Id;

                                try
                                {
                                    Utils.DataFolder = "";

                                    try
                                    {
                                        Utils.DataFolder = process.MainModule.FileName.ToLower().Replace("\\x2.exe", "");
                                    }
                                    catch
                                    {

                                    }

                                    if (Utils.DataFolder == "" || Utils.DataFolder == null)
                                    {
                                        CrashReporting.WriteCrashReport("Can not find the data folder of Elsword");
                                        Process.GetCurrentProcess().Kill();
                                        return;
                                    }

                                    siticoneCheckBox9.Visible = false;
                                    gunaLineTextBox1.Visible = false;

                                    try
                                    {
                                        if (Utils.SpeedHackEnabled && Utils.SpeedHackValue != null && Utils.SpeedHackValue != "" && Utils.SpeedHackValue != "1.0")
                                        {
                                            double value = double.Parse(Utils.SpeedHackValue.Replace(".", ","));

                                            if (value > 1.0D)
                                            {
                                                SpeedHackManager.InjectDLL(processId, rootDir + "\\Temp\\" + speedHackDllName + ".dll");
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }

                                    try
                                    {
                                        SpeedHackManager.InjectDLL(processId, rootDir + "\\Temp\\" + folderName + "\\" + nvcplDllName + ".dll");
                                    }
                                    catch
                                    {

                                    }
                                }
                                catch (Exception ex)
                                {
                                    CrashReporting.WriteCrashReport("DLL Injection failed", ex);
                                    Process.GetCurrentProcess().Kill();
                                    return;
                                }

                                label1.Text = "Waiting for the window...";
                                break;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else if (label1.Text.Equals("Waiting for the window..."))
                {
                    bool found = false;

                    foreach (Process process in Process.GetProcesses())
                    {
                        try
                        {
                            if (process.ProcessName.ToLower().Equals("x2"))
                            {
                                found = true;

                                if (process.MainWindowHandle == IntPtr.Zero || process.MainWindowHandle == new IntPtr(-1) || !process.MainWindowTitle.StartsWith("Elsword - "))
                                {
                                    continue;
                                }
                                else
                                {
                                    canContinue = false;
                                    break;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    if (!found)
                    {
                        CrashReporting.WriteCrashReport("Detection 28, Illegal violation");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                }
            }
            catch
            {

            }
        }

        label1.Text = "Injecting, please wait...";
        canDo = true;
    }
}