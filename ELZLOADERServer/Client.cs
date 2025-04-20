public class Client
{
    public string SessionID { get; set; }
    public string? HWID { get; set; }

    public Client(string sessionID)
    {
        SessionID = sessionID;
    }
}