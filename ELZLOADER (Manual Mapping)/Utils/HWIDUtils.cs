using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

public class HWIDUtils
{
    public static string GetHWID()
    {
        return GetHash(GetCPUID() + GetBIOSID() + GetBaseID() + GetDeviceID() + ProductID() + GetSystemUUID());
    }

    public static string GetDeviceID()
    {
        try
        {
            string right = Environment.SystemDirectory.Substring(0, 2) + "\\";
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_Volume");

            try
            {
                try
                {
                    foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
                    {
                        ManagementObject managementObject = (ManagementObject)managementBaseObject;

                        if (managementObject["Name"].ToString() == right)
                        {
                            return managementObject["DeviceID"].ToString().Substring(11, 36).Replace("-", "");
                        }
                    }
                }
                catch
                {

                }
            }
            catch
            {

            }
        }
        catch
        {

        }

        return "";
    }

    public static string GetSystemUUID()
    {
        try
        {
            var mos = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
            var mbsList = mos.Get();
            string systemId = string.Empty;

            foreach (ManagementBaseObject mo in mbsList)
            {
                systemId = mo["UUID"] as string;
            }

            return systemId;
        }
        catch
        {

        }

        return "";
    }

    public static string ProductID()
    {
        try
        {
            return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion").GetValue("ProductId").ToString();
        }
        catch
        {
            return "";
        }
    }

    private static string GetHash(string s)
    {
        MD5 sec = new MD5CryptoServiceProvider();
        UTF8Encoding enc = new UTF8Encoding();
        byte[] bt = enc.GetBytes(s);
        return GetHexString(sec.ComputeHash(bt));
    }

    private static string GetHexString(byte[] bt)
    {
        string s = string.Empty;

        for (int i = 0; i < bt.Length; i++)
        {
            byte b = bt[i];
            int n, n1, n2;
            n = (int)b;
            n1 = n & 15;
            n2 = (n >> 4) & 15;

            if (n2 > 9)
            {
                s += ((char)(n2 - 10 + (int)'A')).ToString();
            }
            else
            {
                s += n2.ToString();
            }

            if (n1 > 9)
            {
                s += ((char)(n1 - 10 + (int)'A')).ToString();
            }
            else
            {
                s += n1.ToString();
            }

            if ((i + 1) != bt.Length && (i + 1) % 2 == 0)
            {
                s += "-";
            }
        }

        return s;
    }

    private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
    {
        try
        {
            string result = "";

            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {

                        }
                    }
                }
            }

            return result;
        }
        catch
        {

        }

        return "";
    }

    private static string identifier(string wmiClass, string wmiProperty)
    {
        try
        {
            string result = "";

            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {

                    }
                }
            }

            return result;
        }
        catch
        {
            return "";
        }
    }
    private static string GetCPUID()
    {
        string retVal = identifier("Win32_Processor", "UniqueId");

        if (retVal == "")
        {
            retVal = identifier("Win32_Processor", "ProcessorId");

            if (retVal == "")
            {
                retVal = identifier("Win32_Processor", "Name");

                if (retVal == "")
                {
                    retVal = identifier("Win32_Processor", "Manufacturer");
                }

                retVal += identifier("Win32_Processor", "MaxClockSpeed");
            }
        }

        return retVal;
    }

    public static string GetBIOSID()
    {
        return identifier("Win32_BIOS", "Manufacturer") + identifier("Win32_BIOS", "SMBIOSBIOSVersion") + identifier("Win32_BIOS", "IdentificationCode") + identifier("Win32_BIOS", "SerialNumber") + identifier("Win32_BIOS", "ReleaseDate") + identifier("Win32_BIOS", "Version");
    }

    private static string GetDiskID()
    {
        return identifier("Win32_DiskDrive", "Model") + identifier("Win32_DiskDrive", "Manufacturer") + identifier("Win32_DiskDrive", "Signature") + identifier("Win32_DiskDrive", "TotalHeads");
    }

    public static string GetBaseID()
    {
        return identifier("Win32_BaseBoard", "Model") + identifier("Win32_BaseBoard", "Manufacturer") + identifier("Win32_BaseBoard", "Name") + identifier("Win32_BaseBoard", "SerialNumber");
    }
}