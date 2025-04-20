using System;
using System.Text;

public class CrashReporting
{
    public static void WriteCrashReport(string error, Exception ex = null)
    {
        try
        {
            ProtoRandom random = new ProtoRandom(25);
            char[] characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            int keyLength = random.GetRandomInt32(16, 24);
            string crashKey = new ProtoRandom(25).GetRandomString(characters, keyLength);

            byte[] reportPayload = Combine(BitConverter.GetBytes(keyLength + 357), Encoding.UTF8.GetBytes(crashKey));
            string completeReport = "[!] Error: " + error;

            if (ex != null)
            {
                completeReport = "\r\nException Message: " + ex.Message + "\r\nException Source: " + ex.Source + "\r\nException StackTrace: " + ex.StackTrace + "\r\nException Method: " + ex.TargetSite.Name + "\r\nException Class: " + ex.TargetSite.DeclaringType.Name;
            }

            byte[] strBytes = Utils.EncryptAES256(Encoding.UTF8.GetBytes(completeReport), crashKey);
            reportPayload = Combine(reportPayload, strBytes);
            System.IO.File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1) + ":\\crash-report.elz", Utils.EncryptAES256(reportPayload, "ELZL_JKERJEKRKLWEJRKWJER_281739817237"));
        }
        catch
        {

        }
    }

    public static byte[] Combine(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];

        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

        return ret;
    }
}