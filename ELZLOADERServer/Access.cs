using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System;

public class Access : WebSocketBehavior
{
    private static char[] keyChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    protected override void OnClose(CloseEventArgs e)
    {
        try
        {
            Program.clients.Remove(GetClientBySessionID(ID));
        }
        catch
        {

        }
    }

    public static string EncryptCloseReason(string str)
    {
        byte[] data = Encoding.UTF8.GetBytes(str);
        ProtoRandom random = new ProtoRandom(2);
        int keyLength = random.GetRandomInt32(7, 18);
        string key = random.GetRandomString(keyChars, keyLength);
        byte[] encrypted = EncryptAES256(data, key);
        byte[] header = Combine(BitConverter.GetBytes(keyLength), Encoding.UTF8.GetBytes(key));
        byte[] total = Combine(header, encrypted);
        total = EncryptAES256(total, "ELZL_ERHJHEWRKJHWEKJRH_21736213");
        return Convert.ToBase64String(total);
    }

    public static string DecryptCloseReason(string str)
    {
        byte[] data = Convert.FromBase64String(str);
        data = DecryptAES256(data, "ELZL_ERHJHEWRKJHWEKJRH_21736213");
        int keyLength = BitConverter.ToInt32(data.Take(4).ToArray());
        data = data.Skip(4).ToArray();
        string key = Encoding.UTF8.GetString(data.Take(keyLength).ToArray());
        data = data.Skip(keyLength).ToArray();
        data = DecryptAES256(data, key);
        return Encoding.UTF8.GetString(data);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        try
        {
            byte[] data = e.RawData;
            Client theClient = GetClientBySessionID(ID);
            data = DecryptAES256(data, "ELZL_8394839293_JKMRJRTN3EA");

            if (data.Length <= 23)
            {
                Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Bad data length"));
                return;
            }

            byte[] md5Hash = data.Take(16).ToArray();
            data = data.Skip(16).ToArray();

            if (!CompareByteArrays(System.Security.Cryptography.MD5.Create().ComputeHash(data), md5Hash))
            {
                Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Integrity violation"));
                return;
            }

            long timestamp = BitConverter.ToInt64(data.Take(8).ToArray(), 0);
            long current = GetTimestamp();

            if (current - timestamp > 10 || timestamp - current > 10)
            {
                Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Bad timestamp"));
                return;
            }

            data = data.Skip(8).ToArray();

            if (theClient.HWID == null)
            {
                if (data.Length != 40)
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Invalid HWID data length"));
                    return;
                }

                if (data[0] != 0xC1)
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Not received 0xC1"));
                    return;
                }

                data = data.Skip(1).ToArray();
                theClient.HWID = Encoding.UTF8.GetString(data);
                Sessions.SendTo(Encrypt(new byte[3] { 0xA6, 0xF5, 0xD9 }), ID);
            }
            else
            {
                if (data.Length != 31)
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Bad received login data length"));
                    return;
                }

                if (data[0] != 0xE5)
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Not received 0xE5"));
                    return;
                }

                data = data.Skip(1).ToArray();
                byte[] usernameBytes = data.Take(10).ToArray();
                data = data.Skip(10).ToArray();
                byte[] passwordBytes = data;

                string username = Encoding.UTF8.GetString(usernameBytes);
                string password = Encoding.UTF8.GetString(passwordBytes);

                if (username.Length != 10 || password.Length != 20)
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Bad credentials length"));
                    return;
                }

                if (!username.StartsWith("user"))
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Bad username"));
                    return;
                }

                if (!Microsoft.VisualBasic.Information.IsNumeric(username.Substring("user".Length)))
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Bad username format"));
                    return;
                }

                if (username.Contains("0"))
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Username not numeric"));
                    return;
                }

                if (!IsPasswordValid(password))
                {
                    Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Bad password format"));
                    return;
                }

                bool done = false;

                foreach (User user in Program.users)
                {
                    if (user.Username.Equals(username) && user.Password.Equals(password))
                    {
                        if (user.HWID == null)
                        {
                            user.HWID = theClient.HWID;
                            done = true;
                        }
                        else
                        {
                            if (user.HWID != theClient.HWID)
                            {
                                Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Invalid HWID"));
                                return;
                            }
                        }

                        if (user.MultiClient)
                        {
                            Sessions.SendTo(Encrypt(new byte[2] { 0xF8, 0xC3 }), ID);
                        }
                        else
                        {
                            Sessions.SendTo(Encrypt(new byte[2] { 0xF8, 0xC2 }), ID);
                        }

                        done = true;
                    }
                }

                if (done)
                {
                    System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                }
                else
                {
                    Sessions.SendTo(Encrypt(new byte[2] { 0x83, 0xB4 }), ID);
                }
            }
        }
        catch
        {
            try
            {
                Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Exception thrown"));
            }
            catch
            {

            }
        }
    }

    protected override void OnOpen()
    {
        try
        {
            Program.clients.Add(new Client(ID));
            Sessions.SendTo(Encrypt(new byte[1] { Program.version }), ID);
        }
        catch
        {
            try
            {
                Sessions.CloseSession(ID, CloseStatusCode.Normal, EncryptCloseReason("Exception thrown"));
            }
            catch
            {

            }
        }
    }

    public Client GetClientBySessionID(string sessionID)
    {
        Client theClient = null;

        foreach (Client client in Program.clients)
        {
            if (client.SessionID.Equals(sessionID))
            {
                theClient = client;
                break;
            }
        }

        return theClient;
    }

    public bool IsPasswordValid(string str)
    {
        foreach (char c in str)
        {
            bool found = false;

            foreach (char s in Program.passwordCharacters)
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

    public static byte[] EncryptAES256(byte[] input, string pass)
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

            return AES.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
        }
        catch
        {
            return null;
        }
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
            return null;

        }
    }

    public static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];

        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

        return ret;
    }

    public static byte[] GetMD5Hash(byte[] input)
    {
        return System.Security.Cryptography.MD5.Create().ComputeHash(input);
    }

    public static long GetTimestamp()
    {
        return ((DateTimeOffset)DateTime.UtcNow.ToUniversalTime()).ToUniversalTime().ToUnixTimeSeconds();
    }

    public static byte[] Encrypt(byte[] data)
    {
        try
        {
            byte[] timestamp = BitConverter.GetBytes(GetTimestamp());
            byte[] payload = Combine(timestamp, data);
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(payload);
            payload = Combine(hash, payload);
            payload = EncryptAES256(payload, "ELZL_8394839293_JKMRJRTN3EA");
            return payload;
        }
        catch
        {
            return null;
        }
    }
}