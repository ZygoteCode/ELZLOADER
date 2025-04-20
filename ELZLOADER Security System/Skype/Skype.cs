using System.Diagnostics;

public class Skype
{ 
    public static void Main(string[] args)
    {
        try
        {
            string totalArgs = "";

            foreach (string arg in args)
            {
                if (totalArgs == "")
                {
                    totalArgs = arg;
                }
                else
                {
                    totalArgs += " " + arg;
                }
            }

            int processId = int.Parse(totalArgs.Split(' ')[0]);
            totalArgs = totalArgs.Replace(processId.ToString() + " ", "");
            Process process = Process.GetProcessById(processId);
            Lunar.LibraryMapper mapper = new Lunar.LibraryMapper(process, totalArgs, Lunar.MappingFlags.DiscardHeaders);
            mapper.MapLibrary();
            mapper = null;
            process = null;
        }
        catch
        {

        }

        GC.Collect();
        Process.GetCurrentProcess().Kill();
    }
}