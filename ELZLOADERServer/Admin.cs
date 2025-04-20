using WebSocketSharp.Server;
using WebSocketSharp;
using System.Text;
using Newtonsoft.Json;

public class Admin : WebSocketBehavior
{
    public bool authenticated = false;
    public ProtoRandom random = new ProtoRandom(2);
    public char[] usernameCharacters = "123456789".ToCharArray();

    protected override void OnMessage(MessageEventArgs e)
    {
        try
        {
            if (!authenticated)
            {
                if (e.RawData.Length != 390)
                {
                    Sessions.CloseSession(ID);
                    return;
                }

                if (Encoding.UTF8.GetString(e.RawData) != "nUgQkjhITPy05dVTdXvj29NNK6QGHeUz0dcJmauF1My9tG9Ke3L5awc11kkF2yaAj7r4hUAnkguO7MA80bQLCGXi7nAuWYcPJTLisQ9wf4FLE3RNW6LbGYXkG94XElIjlF5UwrZBEgjeaPO4nHEAF28QF2FQq5uywp24070YYKyIwOBniWzQTRJediBtT21Trynaorx33zov092vjvVplDqp8C5EVVsMqQmwOKFUUaw879V1QThODXYnlrcPbJNz5JUQBxGOz3vKvNiPrAhFCN9nC8ylcfK5eeovzc3awnHCT7AvYLRGPBBW6b3ZnC3ezk6pBQxt7nJjrG1RjqymFanZs7y0v5PU602Hfdloz7sXVPWPR0vvvwoOgD6n7CPHnnGRvJ")
                {
                    Sessions.CloseSession(ID);
                    return;
                }

                authenticated = true;
                Sessions.SendTo(new byte[1] { 0xB1 }, ID);
            }
            else
            {
                if (e.RawData.Length == 11)
                {
                    string username = Encoding.UTF8.GetString(e.RawData.Skip(1).ToArray());
                    User user = null;

                    foreach (User usr in Program.users)
                    {
                        if (usr.Username.Equals(username))
                        {
                            user = usr;
                            break;
                        }
                    }

                    if (user == null)
                    {
                        Sessions.SendTo(new byte[1] { 0xB2 }, ID);
                        return;
                    }

                    if (e.RawData[0] == 0xA1) // Reset HWID
                    {
                        user.HWID = null;
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xB3 }, ID);
                    }
                    else if (e.RawData[0] == 0xA2) // Reset password
                    {
                        user.Password = GeneratePassword();
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(Combine(new byte[1] { 0xB4 }, Encoding.UTF8.GetBytes(user.Password)), ID);
                    }
                    else if (e.RawData[0] == 0xA3) // Get password
                    {
                        Sessions.SendTo(Combine(new byte[1] { 0xB5 }, Encoding.UTF8.GetBytes(user.Password)), ID);
                    }
                    else if (e.RawData[0] == 0xA4) // Delete user
                    {
                        Program.users.Remove(user);
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xB7 }, ID);
                    }
                    else if (e.RawData[0] == 0xE1)
                    {
                        if (user.HWID == null)
                        {
                            Sessions.SendTo(new byte[1] { 0xF2 }, ID);
                        }
                        else
                        {
                            Sessions.SendTo(Combine(new byte[1] { 0xF1 }, Encoding.UTF8.GetBytes(user.HWID)), ID);
                        }
                    }
                    else if (e.RawData[0] == 0xE2)
                    {
                        if (user.HWID == null)
                        {
                            Sessions.SendTo(new byte[1] { 0xF2 }, ID);
                        }
                        else
                        {
                            Sessions.SendTo(new byte[1] { 0xF3 }, ID);
                        }
                    }
                    else if (e.RawData[0] == 0xF7) // Multiclient: ON
                    {
                        user.MultiClient = true;
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xF7 }, ID);
                    }
                    else if (e.RawData[0] == 0xF8) // Multiclient: OFF
                    {
                        user.MultiClient = false;
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xF8 }, ID);
                    }
                    else
                    {
                        Sessions.CloseSession(ID);
                    }
                }
                else if (e.RawData.Length == 50)
                {
                    if (e.RawData[0] == 0xE3)
                    {
                        byte[] data = e.RawData.Skip(1).ToArray();
                        string username = Encoding.UTF8.GetString(data.Take(10).ToArray());
                        data = data.Skip(10).ToArray();
                        string HWID = Encoding.UTF8.GetString(data);
                        User user = null;

                        foreach (User usr in Program.users)
                        {
                            if (usr.Username.Equals(username))
                            {
                                user = usr;
                                break;
                            }
                        }

                        if (user == null)
                        {
                            Sessions.SendTo(new byte[1] { 0xB2 }, ID);
                            return;
                        }

                        user.HWID = HWID;
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xF4 }, ID);
                    }
                    else
                    {
                        Sessions.CloseSession(ID);
                    }
                }
                else if (e.RawData.Length == 1)
                {
                    if (e.RawData[0] == 0xA5) // Add user
                    {
                        User user = new User(GenerateUsername(), GeneratePassword());
                        Program.users.Add(user);
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        byte[] response = new byte[1] { 0xB8 };
                        response = Combine(response, Encoding.UTF8.GetBytes(user.Username));
                        response = Combine(response, Encoding.UTF8.GetBytes(user.Password));
                        Sessions.SendTo(response, ID);
                    }
                    else if (e.RawData[0] == 0xD1)
                    {
                        Program.version++;
                        System.IO.File.WriteAllText("version.txt", Program.version.ToString());
                        Sessions.SendTo(Combine(new byte[1] { 0xE1 }, new byte[1] { Program.version }), ID);
                    }
                    else if (e.RawData[0] == 0xD2)
                    {
                        Sessions.SendTo(Combine(new byte[1] { 0xE2 }, new byte[1] { Program.version }), ID);
                    }
                    else if (e.RawData[0] == 0xD4)
                    {
                        Sessions.SendTo(Combine(new byte[1] { 0xE4 }, BitConverter.GetBytes(Program.users.Count)), ID);
                    }
                    else if (e.RawData[0] == 0xD5)
                    {
                        foreach (User usr in Program.users)
                        {
                            usr.HWID = null;
                        }

                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xE5 }, ID);
                    }
                    else if (e.RawData[0] == 0xD6)
                    {
                        Program.users.Clear();
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xE6 }, ID);
                    }
                    else if (e.RawData[0] == 0xE4)
                    {
                        Sessions.SendTo(Combine(new byte[1] { 0xF5 }, BitConverter.GetBytes(System.IO.File.ReadAllBytes("users.json").Length)), ID);
                    }
                    else if (e.RawData[0] == 0xE5)
                    {
                        Sessions.SendTo(Combine(new byte[1] { 0xF6 }, Encoding.UTF8.GetBytes(System.IO.File.ReadAllText("users.json"))), ID);

                    }
                    else
                    {
                        Sessions.CloseSession(ID);
                    }
                }
                else if (e.RawData.Length == 40)
                {
                    if (e.RawData[0] == 0xD9)
                    {
                        byte[] data = e.RawData;
                        data = data.Skip(1).ToArray();
                        string HWID = Encoding.UTF8.GetString(data);

                        User user = null;

                        foreach (User usr in Program.users)
                        {
                            if (usr.HWID != null)
                            {
                                if (usr.HWID.Equals(HWID))
                                {
                                    user = usr;
                                    break;
                                }
                            }
                        }

                        if (user == null)
                        {
                            Sessions.SendTo(new byte[1] { 0xB2 }, ID);
                            return;
                        }

                        Sessions.SendTo(Combine(new byte[1] { 0xE9 }, Encoding.UTF8.GetBytes(user.Username)), ID);
                    }
                    else
                    {
                        Sessions.CloseSession(ID);
                    }
                }
                else if (e.RawData.Length == 21)
                {
                    if (e.RawData[0] == 0xD7)
                    {
                        byte[] data = e.RawData;
                        data = data.Skip(1).ToArray();
                        string password = Encoding.UTF8.GetString(data);

                        User user = null;

                        foreach (User usr in Program.users)
                        {
                            if (usr.Password.Equals(password))
                            {
                                user = usr;
                                break;
                            }
                        }

                        if (user == null)
                        {
                            Sessions.SendTo(new byte[1] { 0xB2 }, ID);
                            return;
                        }

                        Sessions.SendTo(Combine(new byte[1] { 0xE7 }, Encoding.UTF8.GetBytes(user.Username)), ID);
                    }
                    else if (e.RawData[0] == 0xD8)
                    {
                        byte[] data = e.RawData;
                        data = data.Skip(1).ToArray();
                        string username = Encoding.UTF8.GetString(data.Take(10).ToArray());
                        data = data.Skip(10).ToArray();
                        string newUsername = Encoding.UTF8.GetString(data.Take(10).ToArray());

                        User user = null;

                        foreach (User usr in Program.users)
                        {
                            if (usr.Username.Equals(username))
                            {
                                user = usr;
                                break;
                            }
                        }

                        if (user == null)
                        {
                            Sessions.SendTo(new byte[1] { 0xB2 }, ID);
                            return;
                        }

                        user.Username = newUsername;
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xE8 }, ID);
                    }
                    else
                    {
                        Sessions.CloseSession(ID);
                    }
                }
                else if (e.RawData.Length == 31)
                {
                    if (e.RawData[0] == 0xA6) // Modify user
                    {
                        byte[] data = e.RawData;
                        data = data.Skip(1).ToArray();
                        byte[] usernameBytes = data.Take(10).ToArray();
                        data = data.Skip(10).ToArray();
                        byte[] passwordBytes = data;

                        string username = Encoding.UTF8.GetString(usernameBytes);
                        string password = Encoding.UTF8.GetString(passwordBytes);

                        User user = null;

                        foreach (User usr in Program.users)
                        {
                            if (usr.Username.Equals(username))
                            {
                                user = usr;
                                break;
                            }
                        }

                        if (user == null)
                        {
                            Sessions.SendTo(new byte[1] { 0xB2 }, ID);
                            return;
                        }

                        user.Password = password;
                        System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
                        Sessions.SendTo(new byte[1] { 0xB9 }, ID);
                    }
                    else
                    {
                        Sessions.CloseSession(ID);
                    }
                }
                else if (e.RawData.Length == 3)
                {
                    if (e.RawData[0] == 0xD3)
                    {
                        byte theVer = e.RawData[1];
                        Program.version = theVer;
                        System.IO.File.WriteAllText("version.txt", Program.version.ToString());
                        Sessions.SendTo(Combine(new byte[1] { 0xE3 }, new byte[1] { Program.version }), ID);
                    }
                    else
                    {
                        Sessions.CloseSession(ID);
                    }
                }
            }
        }
        catch
        {
            try
            {
                Sessions.CloseSession(ID);
            }
            catch
            {

            }
        }
    }

    protected override void OnOpen()
    {
        authenticated = false;
    }

    protected override void OnClose(CloseEventArgs e)
    {
        authenticated = false;
    }

    public string GeneratePassword()
    {
        return random.GetRandomString(Program.passwordCharacters, 20);
    }

    public string GenerateUsername()
    {
        return "user" + random.GetRandomString(usernameCharacters, 6);
    }

    public static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];

        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

        return ret;
    }
}