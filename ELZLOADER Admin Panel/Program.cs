using System;
using WebSocketSharp;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;

public class Program
{
    public static WebSocket ws;
    public static bool authenticated;

    public static void Main()
    {
        try
        {
            Console.WriteLine("[!] Awaiting for authentication, please wait a while.");
            ws = new WebSocket("ws://185.197.194.66:23485/TH11_ELZLOADER_SERVER_ADMIN_ACCESS_CODE_4578734873847_PASS_EREJWHRKHWEJRHWEKJHR");
            ws.OnMessage += Ws_OnMessage;
            ws.OnClose += Ws_OnClose;
            ws.Connect();
            ws.Send(Encoding.UTF8.GetBytes("nUgQkjhITPy05dVTdXvj29NNK6QGHeUz0dcJmauF1My9tG9Ke3L5awc11kkF2yaAj7r4hUAnkguO7MA80bQLCGXi7nAuWYcPJTLisQ9wf4FLE3RNW6LbGYXkG94XElIjlF5UwrZBEgjeaPO4nHEAF28QF2FQq5uywp24070YYKyIwOBniWzQTRJediBtT21Trynaorx33zov092vjvVplDqp8C5EVVsMqQmwOKFUUaw879V1QThODXYnlrcPbJNz5JUQBxGOz3vKvNiPrAhFCN9nC8ylcfK5eeovzc3awnHCT7AvYLRGPBBW6b3ZnC3ezk6pBQxt7nJjrG1RjqymFanZs7y0v5PU602Hfdloz7sXVPWPR0vvvwoOgD6n7CPHnnGRvJ"));
        }
        catch
        {

        }

        while (true)
        {
            if (authenticated)
            {
                try
                {
                    string command = Console.ReadLine();

                    if (command.StartsWith("."))
                    {
                        command = command.Substring(1);

                        if (command == "help")
                        {
                            /* Console.WriteLine("[!] Here is the list of all commands:\r\n\r\n" +
                                 ".help - Get all commands.\r\n" +
                                 ".adduser - Add a new user.\r\n" +
                                 ".deleteuser <username> - Delete a existing user.\r\n" +
                                 ".modifypassword <username> <password> - Modify the password of a existing user.\r\n" +
                                 ".modifyusername <username> <new-username> - Modify the username of a existing user.\r\n" +
                                 ".modifyhwid <username> <hwid> - Modify the HWID of a existing user.\r\n" +
                                 ".getpassword <username> - Get the password of a existing user.\r\n" +
                                 ".resetpassword <username> - Reset the password of a existing user.\r\n" +
                                 ".resethwid <username> - Reset the HWID of a existing user.\r\n" +
                                 ".readcrash - Read the crash report (crash-report.elz).\r\n" +
                                 ".readcredentials - Read the credentials (credentials.elz).\r\n" +
                                 ".update - Make a new update (increment the version)\r\n" +
                                 ".getversion - Get the current version.\r\n" +
                                 ".setversion <version> - Set a new version.\r\n" +
                                 ".accounts - Get the number of accounts.\r\n" +
                                 ".resethwids - Reset all HWIDs of all users.\r\n" +
                                 ".deleteall - Delete all users from the database.\r\n" +
                                 ".getfrompassword <password> - Get the account from its password.\r\n" +
                                 ".getfromhwid <hwid> - Get the account from its HWID.\r\n" +
                                 ".gethwid <username> - Get the HWID of a existing user.\r\n" +
                                 ".isauth <username> - Check if a user is authenticated, so if has a valid HWID.\r\n" +
                                 ".getsize - Get the total size of the database.\r\n" +
                                 ".savedb - Save in local a backup copy of the database.\r\n" +
                                 ".enablemulticlient <username> - Enable the Multi Client feature for a user.\r\n" +
                                 ".disablemulticlient <username> - Disable the Multi Client feature for a user.");*/

                            Console.WriteLine("[!] Here is the list of all commands:\r\n\r\n" +
                                ".help - Get all commands.\r\n" +
                                ".adduser - Add a new user.\r\n" +
                                ".deleteuser <username> - Delete a existing user.\r\n" +
                                ".modifypassword <username> <password> - Modify the password of a existing user.\r\n" +
                                ".modifyusername <username> <new-username> - Modify the username of a existing user.\r\n" +
                                ".modifyhwid <username> <hwid> - Modify the HWID of a existing user.\r\n" +
                                ".getpassword <username> - Get the password of a existing user.\r\n" +
                                ".resetpassword <username> - Reset the password of a existing user.\r\n" +
                                ".resethwid <username> - Reset the HWID of a existing user.\r\n" +
                                ".readcrash - Read the crash report (crash-report.elz).\r\n" +
                                ".readcredentials - Read the credentials (credentials.elz).\r\n" +
                                ".update - Make a new update (increment the version)\r\n" +
                                ".getversion - Get the current version.\r\n" +
                                ".setversion <version> - Set a new version.\r\n" +
                                ".accounts - Get the number of accounts.\r\n" +
                                ".resethwids - Reset all HWIDs of all users.\r\n" +
                                ".deleteall - Delete all users from the database.\r\n" +
                                ".getfrompassword <password> - Get the account from its password.\r\n" +
                                ".getfromhwid <hwid> - Get the account from its HWID.\r\n" +
                                ".gethwid <username> - Get the HWID of a existing user.\r\n" +
                                ".isauth <username> - Check if a user is authenticated, so if has a valid HWID.\r\n" +
                                ".getsize - Get the total size of the database.\r\n" +
                                ".savedb - Save in local a backup copy of the database.");

                        }
                        else if (command == "adduser")
                        {
                            ws.Send(new byte[1] { 0xA5 });
                        }
                        else if (command.StartsWith("deleteuser "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xA4 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("enablemulticlient "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xF7 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("disablemulticlient "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xF8 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("getpassword "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xA3 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("resetpassword "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xA2 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("resethwid "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xA1 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("modifypassword "))
                        {
                            string[] splitted = command.Split(' ');
                            string username = splitted[1], password = splitted[2];
                            byte[] payload = new byte[1] { 0xA6 };
                            payload = Combine(payload, Encoding.UTF8.GetBytes(username));
                            payload = Combine(payload, Encoding.UTF8.GetBytes(password));
                            ws.Send(payload);
                        }
                        else if (command.StartsWith("modifyusername "))
                        {
                            string[] splitted = command.Split(' ');
                            string username = splitted[1], newUsername = splitted[2];
                            byte[] payload = new byte[1] { 0xD8 };
                            payload = Combine(payload, Encoding.UTF8.GetBytes(username));
                            payload = Combine(payload, Encoding.UTF8.GetBytes(newUsername));
                            ws.Send(payload);
                        }
                        else if (command.Equals("readcrash"))
                        {
                            if (!System.IO.File.Exists("crash-report.elz"))
                            {
                                Console.WriteLine("[!] There is no 'crash-report.elz' file present in this directory.");
                                continue;
                            }

                            try
                            {
                                byte[] readBytes = System.IO.File.ReadAllBytes("crash-report.elz");
                                readBytes = DecryptAES256(readBytes, "ELZL_JKERJEKRKLWEJRKWJER_281739817237");

                                int keyLength = BitConverter.ToInt32(readBytes, 0) - 357;
                                readBytes = readBytes.Skip(4).ToArray();

                                string crashKey = Encoding.UTF8.GetString(readBytes.Take(keyLength).ToArray());
                                readBytes = readBytes.Skip(keyLength).ToArray();

                                readBytes = DecryptAES256(readBytes, crashKey);
                                string completeReport = Encoding.UTF8.GetString(readBytes);

                                Console.WriteLine("Got your crash report:\r\n" + completeReport);
                            }
                            catch
                            {
                                Console.WriteLine("[!] Can not read the crash report.");
                            }
                        }
                        else if (command.Equals("readcredentials"))
                        {
                            if (!System.IO.File.Exists("credentials.elz"))
                            {
                                Console.WriteLine("[!] There is no 'credentials.elz' file present in this directory.");
                                continue;
                            }

                            try
                            {
                                string credentials = DecryptCredentials(System.IO.File.ReadAllText("credentials.elz"));
                                string[] splitted = credentials.Split('|');

                                Console.WriteLine("Got your credentials:\r\n" + "Username: " + splitted[0] + "\r\n" + "Password: " + splitted[1]);
                            }
                            catch
                            {
                                Console.WriteLine("[!] Can not read the crash report.");
                            }
                        }
                        else if (command.Equals("update"))
                        {
                            ws.Send(new byte[1] { 0xD1 });
                        }
                        else if (command.Equals("getversion"))
                        {
                            ws.Send(new byte[1] { 0xD2 });
                        }
                        else if (command.StartsWith("setversion "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xD3 }, BitConverter.GetBytes(byte.Parse(command))));
                        }
                        else if (command.Equals("accounts"))
                        {
                            ws.Send(new byte[1] { 0xD4 });
                        }
                        else if (command.Equals("resethwids"))
                        {
                            ws.Send(new byte[1] { 0xD5 });
                        }
                        else if (command.Equals("deleteall"))
                        {
                            ws.Send(new byte[1] { 0xD6 });
                        }
                        else if (command.StartsWith("getfrompassword "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xD7 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("getfromhwid "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xD9 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("gethwid "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xE1 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("isauth "))
                        {
                            command = command.Split(' ')[1];
                            ws.Send(Combine(new byte[1] { 0xE2 }, Encoding.UTF8.GetBytes(command)));
                        }
                        else if (command.StartsWith("sethwid "))
                        {
                            string[] splitted = command.Split(' ');
                            string username = splitted[1], HWID = splitted[2];
                            byte[] payload = new byte[1] { 0xE3 };
                            payload = Combine(payload, Encoding.UTF8.GetBytes(username));
                            payload = Combine(payload, Encoding.UTF8.GetBytes(HWID));
                            ws.Send(payload);
                        }
                        else if (command.Equals("getsize"))
                        {
                            ws.Send(new byte[1] { 0xE4 });
                        }
                        else if (command.Equals("savedb"))
                        {
                            ws.Send(new byte[1] { 0xE5 });
                        }
                        else
                        {
                            Console.WriteLine("[!] Command not recognized. Please, type .help to get the list of all commands.");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("[!] Bad command syntax. Please, type .help to get the list of all commands.");
                }
            }
        }
    }

    private static void Ws_OnClose(object sender, CloseEventArgs e)
    {
        Console.WriteLine("[!] The connection is now closed. Maybe the server is not reachable or a invalid command has been executed.");
        Console.WriteLine("[!] Press ENTER to exit from the program.");
        Console.ReadLine();
        Process.GetCurrentProcess().Kill();
    }

    public static byte[] DecryptAES256(byte[] input, string pass)
    {
        var AES = new RijndaelManaged();

        try
        {
            var hash = new byte[32];
            var temp = new MD5CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(pass));

            Array.Copy(temp, 0, hash, 0, 16);
            Array.Copy(temp, 0, hash, 15, 16);

            AES.Key = hash;
            AES.Mode = CipherMode.ECB;

            return AES.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
        }
        catch
        {
            Process.GetCurrentProcess().Kill();
            return null;
        }
    }

    private static void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        if (e.RawData[0] == 0xF6)
        {
            System.IO.File.WriteAllText("backup-users.json", Encoding.UTF8.GetString(e.RawData.Skip(1).ToArray()));
            Console.WriteLine("[!] Succesfully saved a local backup copy of the database.");
            return;
        }

        if (e.RawData.Length == 1)
        {
            if (e.RawData[0] == 0xB1)
            {
                authenticated = true;
                Console.Title = "ELZLOADER - Admin Panel";
                Console.WriteLine("[!] Succesfully authenticated to the Admin Panel.");
                Console.WriteLine("[!] Please, type '.help' for the list of all commands.");
            }
            else if (authenticated)
            {
                if (e.RawData[0] == 0xB2)
                {
                    Console.WriteLine("[!] This user was not found.");
                }
                else if (e.RawData[0] == 0xB9)
                {
                    Console.WriteLine("[!] Succesfully modified the password of this user.");
                }
                else if (e.RawData[0] == 0xB7)
                {
                    Console.WriteLine("[!] Succesfully deleted this user.");
                }
                else if (e.RawData[0] == 0xB3)
                {
                    Console.WriteLine("[!] Succesfully resetted the HWID of this user.");
                }
                else if (e.RawData[0] == 0xE5)
                {
                    Console.WriteLine("[!] Succesfully resetted the HWIDs of all users in the database.");
                }
                else if (e.RawData[0] == 0xE6)
                {
                    Console.WriteLine("[!] Succesfully deleted all users from the database.");
                }
                else if (e.RawData[0] == 0xE8)
                {
                    Console.WriteLine("[!] Succesfully modified the username of this user.");
                }
                else if (e.RawData[0] == 0xF2)
                {
                    Console.WriteLine("[!] This user has not been authenticated one time, so he/she has no HWID in the account.");
                }
                else if (e.RawData[0] == 0xF3)
                {
                    Console.WriteLine("[!] This user has been authenticated, so he/she has a valid HWID in the account.");
                }
                else if (e.RawData[0] == 0xF4)
                {
                    Console.WriteLine("[!] Succesfully set the HWID of this user.");
                }
                else if (e.RawData[0] == 0xF7)
                {
                    Console.WriteLine("[!] Succesfully enabled the Multi Client feature for this user.");
                }
                else if (e.RawData[0] == 0xF8)
                {
                    Console.WriteLine("[!] Succesfully disabled the Multi Client feature for this user.");
                }
            }
        }
        else if (e.RawData.Length == 5)
        {
            int bytes = BitConverter.ToInt32(e.RawData.Skip(1).ToArray(), 0);

            if (e.RawData[0] == 0xF5)
            {
                Console.WriteLine("[!] Got the size of the entire database: " + bytes + " bytes.");
            }
        }
        else if (e.RawData.Length == 11)
        {
            string username = Encoding.UTF8.GetString(e.RawData.Skip(1).ToArray());

            if (e.RawData[0] == 0xE7)
            {
                Console.WriteLine($"[!] Got the username from the password: {username}.");
            }
            else if (e.RawData[0] == 0xE9)
            {
                Console.WriteLine($"[!] Got the username from the HWID: {username}.");
            }
        }
        else if (e.RawData.Length == 21)
        {
            string password = Encoding.UTF8.GetString(e.RawData.Skip(1).ToArray());

            if (e.RawData[0] == 0xB4)
            {
                Console.WriteLine("[!] Succesfully resetted the password of this user to: " + password + ".");
            }
            else if (e.RawData[0] == 0xB5)
            {
                Console.WriteLine("[!] Got the password of this user: " + password + ".");
            }
        }
        else if (e.RawData.Length == 31)
        {
            if (e.RawData[0] == 0xB8)
            {
                byte[] data = e.RawData.Skip(1).ToArray();
                string username = Encoding.UTF8.GetString(data.Take(10).ToArray());
                data = data.Skip(10).ToArray();
                string password = Encoding.UTF8.GetString(data.Take(20).ToArray());

                Console.WriteLine("[!] Succesfully created a new user, with username (" + username + ") and password (" + password + ").");

                Thread thread = new Thread(() => Clipboard.SetText($"Username: {username}\r\nPassword: {password}"));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();

                Console.WriteLine("[!] Text to send has been copied to the System Clipboard.");
            }
        }
        else if (e.RawData.Length == 3)
        {
            byte version = e.RawData[1];

            if (e.RawData[0] == 0xE1)
            {
                Console.WriteLine("[!] Succesfully updated the program. New version: " + version + ".");
            }
            else if (e.RawData[0] == 0xE2)
            {
                Console.WriteLine("[!] Got the current version of the program: " + version + ".");
            }
            else if (e.RawData[0] == 0xE3)
            {
                Console.WriteLine("[!] Succesfully set the new version: " + version + ".");
            }
        }
        else if (e.RawData.Length == 5)
        {
            if (e.RawData[0] == 0xE4)
            {
                byte[] data = e.RawData.Skip(1).ToArray();
                int num = BitConverter.ToInt32(data, 0);
                Console.WriteLine("[!] Got the number of accounts in the database: " + num + ".");
            }
        }
        else if (e.RawData.Length == 40)
        {
            if (e.RawData[0] == 0xF1)
            {
                byte[] data = e.RawData.Skip(1).ToArray();
                string HWID = Encoding.UTF8.GetString(data);
                Console.WriteLine("[!] Got the HWID of this user: " + HWID + ".");
            }
        }
    }

    public static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];

        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

        return ret;
    }

    public static string DecryptCredentials(string str)
    {
        byte[] data = Convert.FromBase64String(str);
        data = DecryptAES256(data, "ELZL_JHKJERKJEKRJLEWKJRLKJEWR_3748734897239487982374");
        int keyLength = BitConverter.ToInt32(data.Take(4).ToArray(), 0);
        data = data.Skip(4).ToArray();
        string key = Encoding.UTF8.GetString(data.Take(keyLength).ToArray());
        data = data.Skip(keyLength).ToArray();
        data = DecryptAES256(data, key);
        return Encoding.UTF8.GetString(data);
    }
}