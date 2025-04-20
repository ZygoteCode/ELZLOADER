public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string? HWID { get; set; }
    public bool MultiClient { get; set; }

    public User(string username, string password, string? hWID = null, bool multiClient = false)
    {
        Username = username;
        Password = password;
        HWID = hWID;
        MultiClient = multiClient;
    }
}