using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Microsoft.CSharp;

public class Program
{
    public static WebSocketServer server = new WebSocketServer(23485);
    public static List<Client> clients = new List<Client>();
    public static ResourceSemaphore globalSemaphore = new ResourceSemaphore();
    public static List<User> users = new List<User>();
    public static char[] passwordCharacters = ("bdfghijkopqrstuvz" + "bdfghijkopqrstuvz".ToUpper() + "127890").ToCharArray();
    public static byte version = 2;

    public static void Main()
    {
        if (System.IO.File.Exists("users.json"))
        {
            users = (List<User>)JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText("users.json"));
            System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(Program.users));
        }

        if (System.IO.File.Exists("version.txt"))
        {
            version = byte.Parse(System.IO.File.ReadAllText("version.txt"));
        }

        Console.Title = "ELZLOADER Server";
        Console.WriteLine("[!] Starting server...");

        server.KeepClean = false;
        server.AddWebSocketService<Access>("/Access");
        server.AddWebSocketService<Admin>("/TH11_ELZLOADER_SERVER_ADMIN_ACCESS_CODE_4578734873847_PASS_EREJWHRKHWEJRHWEKJHR");
        server.Start();

        Console.WriteLine("[!] Server started!");
        Console.ReadLine();

        server.Stop();
    }
}