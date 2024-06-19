using System;

public static class ArgumentHandler
{
    public static (string filePath, Config config) ParseArguments(string[] args)
    {
        string filePath = null;
   
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--file" && i + 1 < args.Length)
            {
                filePath = args[i + 1];
            }
  
        }

        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("Invalid arguments. Usage: --file <path>");
        }

        Config config = new GaboonGrabber();
  
        return (filePath, config);
    }
}
