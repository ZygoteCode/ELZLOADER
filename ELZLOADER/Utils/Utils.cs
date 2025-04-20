using System.Text;
using System.Security.Cryptography;
using System;
using System.Linq;
using Microsoft.VisualBasic;
using System.Management;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

public static class Utils
{
    public static string STRING_1 = "", DataFolder = "", SpeedHackValue = "";
    public static byte[] BYTE_ARRAY_1 = null;
    public static bool SpeedHackEnabled = false, AllowMultiClient = false, MultiClient = false;
    public static int ElswordProcessID = -1;

    public static bool CanMultiClient()
    {
        return Utils.AllowMultiClient && Utils.MultiClient;
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
            Process.GetCurrentProcess().Kill();
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
            Process.GetCurrentProcess().Kill();
            return null;
        }
    }

    public static long GetTimestamp()
    {
        return ((DateTimeOffset)DateTime.UtcNow.ToUniversalTime()).ToUniversalTime().ToUnixTimeSeconds();
    }

    public static string smethod_9()
    {
        string right = Environment.SystemDirectory.Substring(0, 2) + "\\";
        ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_Volume");

        foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
        {
            ManagementObject managementObject = (ManagementObject)managementBaseObject;

            if (managementObject["Name"].ToString() == right)
            {
                return managementObject["DeviceID"].ToString().Substring(11, 36).Replace("-", "");
            }
        }

        return "";
    }

    public static string smethod_0(string string_0, string string_1)
    {
        string_0 = string.Join("", Enumerable.Repeat<string>(string_0, 10)).Substring(0, string_1.Length);
        string string_2 = smethod_3(smethod_1(string_0.smethod_2()), smethod_1(string_1.smethod_2()));
        int num = new Random().Next(80, 90);
        string text = num.ToString("X");
        int num2 = 1;

        checked
        {
            do
            {
                text += (num + num2).ToString("X");
                num2++;
            }
            while (num2 <= 31);

            string string_3 = num.ToString("X") + smethod_3(smethod_1(string_2.smethod_2()), smethod_1(text)).smethod_2();
            return smethod_1_1(string_3);
        }
    }

    public static string smethod_2(this string string_0)
    {
        return BitConverter.ToString(Encoding.Default.GetBytes(string_0)).Replace("-", "");
    }

    public static string smethod_3(string string_0, string string_1)
    {
        StringBuilder stringBuilder = new StringBuilder();

        checked
        {
            int num = string_1.Length - 1;

            for (int i = 0; i <= num; i++)
            {
                stringBuilder.Append(Strings.ChrW((int)(string_1[i] ^ string_0[i % string_0.Length])));
            }

            return stringBuilder.ToString();
        }
    }

    private static string smethod_1(string string_0)
    {
        if (string_0.Length % 2 == 1)
        {
            string_0 += "0";
        }

        checked
        {
            byte[] array = new byte[(int)Math.Round(unchecked((double)string_0.Length / 2.0 - 1.0)) + 1];
            int num = string_0.Length - 1;

            for (int i = 0; i <= num; i += 2)
            {
                array[(int)Math.Round((double)i / 2.0)] = Convert.ToByte(string_0.Substring(i, 2), 16);
            }

            return Encoding.Default.GetString(array);
        }
    }

    public static string smethod_1_1(string string_0)
    {
        StringBuilder stringBuilder = new StringBuilder();

        checked
        {
            int num = string_0.Length - 1;

            for (int i = 0; i <= num; i += 2)
            {
                string value = string_0.Substring(i, 2);
                stringBuilder.Append(Convert.ToChar(Convert.ToUInt32(value, 16)).ToString());
            }

            return stringBuilder.ToString();
        }
    }
}