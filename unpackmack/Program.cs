using System;
using System.IO;
using dnlib.DotNet;

class XyrisUnpack
{
    static void Main(string[] args)
    {
        try
        {
            var (filePath, config) = ArgumentHandler.ParseArguments(args);

            var module = ModuleDefMD.Load(filePath);

            Console.WriteLine($"Processing config: {config.Name}");
            config.Settings(module);
            config.Unpack(module);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
