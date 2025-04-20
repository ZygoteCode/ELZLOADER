using MetroSuite;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Principal;
using WebSocketSharp;
using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Linq;
using System.Runtime.InteropServices;

public partial class FormLogin : MetroForm
{
    public char[] characters = ("bdfghijkopqrstuvz" + "bdfghijkopqrstuvz".ToUpper() + "127890").ToCharArray();
    public bool opened = false, initialized = false, actionLogin = false, logged = false, respondedLogin = false, hwidReceived = false, closed = false;
    public WebSocket ws;
    public string credentialsDir = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":\\credentials.elz";

    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    public FormLogin()
    {
        try
        {
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.Id != Process.GetCurrentProcess().Id)
                    {
                        if (process.ProcessName.ToLower().Contains("elzloader"))
                        {
                            process.Kill();
                        }
                    }
                }
                catch
                {

                }
            }

            IntPtr kernel32 = LoadLibrary("kernel32.dll");
            IntPtr GetProcessId = GetProcAddress(kernel32, "IsDebuggerPresent");
            byte[] data = new byte[1];
            Marshal.Copy(GetProcessId, data, 0, 1);

            if (data[0] == 0xE9)
            {
                CrashReporting.WriteCrashReport("Detection 1, Anti DnSpy");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            GetProcessId = GetProcAddress(kernel32, "CheckRemoteDebuggerPresent");
            data = new byte[1];
            Marshal.Copy(GetProcessId, data, 0, 1);

            if (data[0] == 0xE9)
            {
                CrashReporting.WriteCrashReport("Detection 2, Anti DnSpy");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            GetProcessId = GetProcAddress(kernel32, "WriteProcessMemory");
            data = new byte[1];
            Marshal.Copy(GetProcessId, data, 0, 1);

            if (data[0] == 0xE9)
            {
                CrashReporting.WriteCrashReport("Detection 3, Anti DnSpy");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            GetProcessId = GetProcAddress(kernel32, "ReadProcessMemory");
            data = new byte[1];
            Marshal.Copy(GetProcessId, data, 0, 1);

            if (data[0] == 0xE9)
            {
                CrashReporting.WriteCrashReport("Detection 4, Anti DnSpy");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("You must run the program with Administrator privileges!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
                return;
            }

            try
            {
                if (System.IO.File.Exists(Environment.SystemDirectory.Substring(0, 2) + "\\Windows\\System32\\drivers\\etc\\hosts"))
                {
                    if (System.IO.File.ReadAllText(Environment.SystemDirectory.Substring(0, 2) + "\\Windows\\System32\\drivers\\etc\\hosts").Contains("zygotecode.it") || System.IO.File.ReadAllText(Environment.SystemDirectory.Substring(0, 2) + "\\Windows\\System32\\drivers\\etc\\hosts").Contains("185.197.194.66"))
                    {
                        CrashReporting.WriteCrashReport("Detection 5, Hosts File");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                }
            }
            catch
            {

            }

            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ip in host.AddressList)
                {
                    if (ip.ToString().Equals("zygotecode.it") || ip.ToString().Equals("185.197.194.66"))
                    {
                        CrashReporting.WriteCrashReport("Detection 6, Anti Proxy");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                }
            }
            catch
            {

            }

            try
            {
                var host1 = Dns.GetHostAddresses("zygotecode.it");

                foreach (var ip in host1)
                {
                    if (ip.ToString().Equals("127.0.0.1") || ip.ToString().Equals("0.0.0.0") || ip.ToString().Equals("localhost") || ip.ToString().StartsWith("192."))
                    {
                        CrashReporting.WriteCrashReport("Detection 7, Anti Proxy");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                }
            }
            catch
            {

            }

            try
            {
                var host1 = Dns.GetHostAddresses("185.197.194.66");

                foreach (var ip in host1)
                {
                    if (ip.ToString().Equals("127.0.0.1") || ip.ToString().Equals("0.0.0.0") || ip.ToString().Equals("localhost") || ip.ToString().StartsWith("192."))
                    {
                        CrashReporting.WriteCrashReport("Detection 8, Anti Proxy");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                }
            }
            catch
            {

            }

            Utils.STRING_1 = new ProtoRandom(25).GetRandomString(characters, new ProtoRandom(25).GetRandomInt32(375, 963));
            Utils.BYTE_ARRAY_1 = Utils.EncryptAES256(Encoding.UTF8.GetBytes(HWIDUtils.GetBIOSID() + HWIDUtils.GetDeviceID()), "JK32J4L32J43H4J12B3NMQKWEKJQWEKOWEN1MN");

            ws = new WebSocket("ws://185.197.194.66:23485/Access");
            ws.OnClose += Ws_OnClose;
            ws.OnMessage += Ws_OnMessage;
            ws.OnOpen += Ws_OnOpen;
            ws.Connect();

            while (!hwidReceived)
            {
                Thread.Sleep(1);

                if (!ws.IsAlive)
                {
                    MessageBox.Show("Sorry but the Server is not reachable. Maybe the Server is down for maintenance or for hosting problems. Press OK to exit from the program.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }

            InitializeComponent();

            try
            {
                if (System.IO.File.Exists("credentials.txt"))
                {
                    string[] thing = System.IO.File.ReadAllText("credentials.txt").Split('|');
                    gunaLineTextBox1.Text = thing[0];
                    gunaLineTextBox2.Text = thing[1];
                    System.IO.File.Delete("credentials.txt");
                    SaveCredentials();
                }
                else if (System.IO.File.Exists(credentialsDir))
                {
                    LoadCredentials();
                }
            }
            catch
            {

            }        }
        catch (Exception ex)
        {
            CrashReporting.WriteCrashReport("Detection 17, Illegal violation", ex);
            Process.GetCurrentProcess().Kill();
        }
    }

    public void SaveCredentials()
    {
        System.IO.File.WriteAllText(credentialsDir, EncryptCredentials(gunaLineTextBox1.Text + "|" + gunaLineTextBox2.Text));
    }

    public void LoadCredentials()
    {
        string[] thing = DecryptCredentials(System.IO.File.ReadAllText(credentialsDir)).Split('|');
        gunaLineTextBox1.Text = thing[0];
        gunaLineTextBox2.Text = thing[1];
        SaveCredentials();
    }

    private static char[] keyChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    public static string EncryptCredentials(string str)
    {
        byte[] data = Encoding.UTF8.GetBytes(str);
        ProtoRandom random = new ProtoRandom(4);
        int keyLength = random.GetRandomInt32(20, 40);
        string key = random.GetRandomString(keyChars, keyLength);
        byte[] encrypted = Utils.EncryptAES256(data, key);
        byte[] header = Combine(BitConverter.GetBytes(keyLength), Encoding.UTF8.GetBytes(key));
        byte[] total = Combine(header, encrypted);
        total = Utils.EncryptAES256(total, "ELZL_JHKJERKJEKRJLEWKJRLKJEWR_3748734897239487982374");
        return Convert.ToBase64String(total);
    }

    public static string DecryptCredentials(string str)
    {
        byte[] data = Convert.FromBase64String(str);
        data = Utils.DecryptAES256(data, "ELZL_JHKJERKJEKRJLEWKJRLKJEWR_3748734897239487982374");
        int keyLength = BitConverter.ToInt32(data.Take(4).ToArray(), 0);
        data = data.Skip(4).ToArray();
        string key = Encoding.UTF8.GetString(data.Take(keyLength).ToArray());
        data = data.Skip(keyLength).ToArray();
        data = Utils.DecryptAES256(data, key);
        return Encoding.UTF8.GetString(data);
    }

    private void Ws_OnOpen(object sender, System.EventArgs e)
    {
        opened = true;
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

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        try
        {
            if (opened)
            {
                long current = Utils.GetTimestamp();
                byte[] data = e.RawData;
                data = Utils.DecryptAES256(data, "ELZL_8394839293_JKMRJRTN3EA");

                if (data.Length <= 23)
                {
                    CrashReporting.WriteCrashReport("Detection 9, Invalid data length received");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                byte[] md5Hash = data.Take(16).ToArray();
                data = data.Skip(16).ToArray();

                if (!CompareByteArrays(System.Security.Cryptography.MD5.Create().ComputeHash(data), md5Hash))
                {
                    CrashReporting.WriteCrashReport("Detection 10, Data integrity broken");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                long timestamp = BitConverter.ToInt64(data.Take(8).ToArray(), 0);

                if (current - timestamp > 10 || timestamp - current > 10)
                {
                    CrashReporting.WriteCrashReport("Security detection, Bad timestamp");
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                data = data.Skip(8).ToArray();

                if (data.Length == 1)
                {
                    if (initialized || hwidReceived || logged || respondedLogin || actionLogin)
                    {
                        CrashReporting.WriteCrashReport("Detection 202, Bad login data");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }

                    if (data[0] != 36)
                    {
                        MessageBox.Show("A new update is available. Go in the Telegram channel to update.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ws.Close();
                        Process.GetCurrentProcess().Kill();
                    }
                    else
                    {
                        initialized = true;
                        SendPacket(Combine(new byte[1] { 0xC1 }, Encoding.UTF8.GetBytes(HWIDUtils.GetHWID())));
                    }
                }
                else if (data.Length == 2)
                {
                    if (!hwidReceived || logged || respondedLogin)
                    {
                        CrashReporting.WriteCrashReport("Detection 11, Illegal violation");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }

                    if (actionLogin)
                    {
                        if (data[0] == 0xF8 && data[1] == 0xC2)
                        {
                            logged = true;
                            respondedLogin = true;
                        }
                        else if (data[0] == 0x83 && data[1] == 0xB4)
                        {
                            logged = false;
                            respondedLogin = true;
                        }
                        else if (data[0] == 0xF8 && data[1] == 0xC3)
                        {
                            Utils.AllowMultiClient = true;
                            logged = true;
                            respondedLogin = true;
                        }
                        else
                        {
                            CrashReporting.WriteCrashReport("Detection 200, Bad login data");
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                    else
                    {
                        CrashReporting.WriteCrashReport("Detection 201, Bad login data");
                        Process.GetCurrentProcess().Kill();
                    }
                }
                else if (data.Length == 3)
                {
                    if (hwidReceived || logged || respondedLogin || actionLogin)
                    {
                        CrashReporting.WriteCrashReport("Detection 12, Illegal violation");
                        Process.GetCurrentProcess().Kill();
                        return;
                    }

                    if (data[0] == 0xA6 && data[1] == 0xF5 && data[2] == 0xD9)
                    {
                        hwidReceived = true;
                    }
                    else
                    {
                        CrashReporting.WriteCrashReport("Detection 13, Illegal violation");
                        Process.GetCurrentProcess().Kill();
                    }
                }
                else
                {
                    CrashReporting.WriteCrashReport("Detection 14, Illegal violation");
                    Process.GetCurrentProcess().Kill();
                }
            }
            else
            {
                CrashReporting.WriteCrashReport("Detection 15, Illegal violation");
                Process.GetCurrentProcess().Kill();
            }
        }
        catch (Exception ex)
        {
            CrashReporting.WriteCrashReport("Detection 16, Illegal violation", ex);
            Process.GetCurrentProcess().Kill();
        }
    }

    private void Ws_OnClose(object sender, CloseEventArgs e)
    {
        if (!closed || !actionLogin || !logged)
        {
            string reason = e.Reason;
            byte[] data = Convert.FromBase64String(reason);
            data = Utils.DecryptAES256(data, "ELZL_ERHJHEWRKJHWEKJRH_21736213");
            int keyLength = BitConverter.ToInt32(data.Take(4).ToArray(), 0);
            data = data.Skip(4).ToArray();
            string key = Encoding.UTF8.GetString(data.Take(keyLength).ToArray());
            data = data.Skip(keyLength).ToArray();
            data = Utils.DecryptAES256(data, key);
            reason = Encoding.UTF8.GetString(data);
            CrashReporting.WriteCrashReport($"WS Closed. Details:\r\nClose code: {e.Code}.\r\nClose reason: {reason}.");
            Process.GetCurrentProcess().Kill();
        }
    }

    private void gunaButton1_Click(object sender, System.EventArgs e)
    {
        try
        {
            string username = gunaLineTextBox1.Text, password = gunaLineTextBox2.Text;

            if (username.Length != 10)
            {
                MessageBox.Show("Invalid username or password.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password.Length != 20)
            {
                MessageBox.Show("Invalid username or password.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!username.StartsWith("user"))
            {
                MessageBox.Show("Invalid username or password.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Microsoft.VisualBasic.Information.IsNumeric(username.Substring("user".Length)))
            {
                MessageBox.Show("Invalid username or password.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (username.Contains("0"))
            {
                MessageBox.Show("Invalid username or password.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsPasswordValid(password))
            {
                MessageBox.Show("Invalid username or password.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] loginPacket = new byte[1] { 0xE5 };

            loginPacket = Combine(loginPacket, Encoding.UTF8.GetBytes(username));
            loginPacket = Combine(loginPacket, Encoding.UTF8.GetBytes(password));

            SendPacket(loginPacket);
            actionLogin = true;

            while (!respondedLogin)
            {
                Thread.Sleep(1);
            }

            if (!logged)
            {
                respondedLogin = false;
                logged = false;
                MessageBox.Show("Invalid username or password.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveCredentials();
            Program.STATE_1 = new ProtoRandom(25).GetRandomInt32(23, 46);
            Program.BYTE_ARRAY_1 = Utils.EncryptAES256(Encoding.UTF8.GetBytes(HWIDUtils.GetBIOSID() + HWIDUtils.GetDeviceID()), "LWEKRJWEKRJLKWEJRLKJWERLKJEWLKRJE");
            Program.STRING_1 = new ProtoRandom(25).GetRandomString(characters, 811);

            closed = true;
            ws.Close();
            ws = null;
            GC.Collect();
            new FormCheatLoader().Show();

            this.Hide();
            this.Size = new System.Drawing.Size(0, 0);
            this.Visible = false;
            this.Enabled = false;
            this.Opacity = 0;
        }
        catch
        {
            actionLogin = false;
            respondedLogin = false;
            logged = false;
            MessageBox.Show("Invalid username or password.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];

        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

        return ret;
    }

    public void SendPacket(byte[] data)
    {
        try
        {
            byte[] timestamp = BitConverter.GetBytes(Utils.GetTimestamp());
            byte[] payload = Combine(timestamp, data);
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(payload);
            payload = Combine(hash, payload);
            payload = Utils.EncryptAES256(payload, "ELZL_8394839293_JKMRJRTN3EA");
            ws.Send(payload);
        }
        catch (Exception ex)
        {
            CrashReporting.WriteCrashReport("Detection 19, Unable to send packets", ex);
            Process.GetCurrentProcess().Kill();
        }
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
}
