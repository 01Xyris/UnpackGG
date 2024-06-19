using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;

public static class Work
{
    public static string TEMP_KEY;
    public static string TEMP_PATH;
    public static string RES_PATH;
    public static void Search(ModuleDefMD module, Config config)
    {
        SearchString(module, config);
        SearchMethod(module, config);
        SearchResource(module);
    }

    private static void SearchString(ModuleDefMD module, Config config)
    {
        foreach (var type in module.GetTypes())
        {
            foreach (var field in type.Fields)
            {
                if (field.IsStatic && field.FieldType.FullName == "System.String" && field.HasConstant)
                {
                    var value = field.Constant.Value as string;
                    if (value != null)
                    {
                        foreach (var pattern in config.Patterns)
                        {
                            CheckAndPrintMatches(value, field.Name, pattern);
                        }
                    }
                }
            }
        }
    }

    private static void SearchMethod(ModuleDefMD module, Config config)
    {
        foreach (var type in module.GetTypes())
        {
            foreach (var method in type.Methods)
            {
                if (method.HasBody)
                {
                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (instruction.OpCode == OpCodes.Ldstr && instruction.Operand is string stringOperand)
                        {
                            foreach (var pattern in config.Patterns)
                            {
                                CheckAndPrintMatches(stringOperand, method.Name, pattern);
                            }
                        }
                    }
                }
            }
        }
    }

    public static void SearchResource(ModuleDefMD module)
    {
        foreach (var resource in module.Resources)
        {
            Console.WriteLine($"Resource Name: {resource.Name}");
            Console.WriteLine($"Resource Type: {resource.ResourceType}");

            if (resource is EmbeddedResource embeddedResource)
            {
                var resourceData = embeddedResource.CreateReader().ReadBytes((int)embeddedResource.CreateReader().Length);

                if (resourceData.Length > 100 * 1024) 
                {
                    SaveLargeResource(resourceData, resource.Name);
                }
            }
        }
    }

    private static void SaveLargeResource(byte[] resourceData, string resourceName)
    {
        string fileName = $"Unpack_{resourceName}.resources";
        string filePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Res", fileName);

        try
        {
            File.WriteAllBytes(filePath, resourceData);
            Console.WriteLine($"Resource saved to: {filePath}");
           
            ReadAndProcessResource(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save resource: {ex.Message}");
        }
    }

    private static void ReadAndProcessResource(string resourceFilePath)
    {
        using (var reader = new ResourceReader(resourceFilePath))
        {
            IDictionaryEnumerator enumerator = reader.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string resourceName = (string)enumerator.Key;
                object resourceValue = enumerator.Value;

                Console.WriteLine($"Resource Entry Name: {resourceName}");

                if (resourceValue is byte[] resourceData)
                {
                    SaveResourceData(resourceData, resourceName);
                }
                else if (resourceValue is Bitmap bitmap)
                {
                    ProcessBitmap(bitmap, resourceName);
                }
                else
                {
                    Console.WriteLine($"Resource Content: {resourceValue}");
                }
            }
        }
    }

    private static void SaveResourceData(byte[] resourceData, string resourceName)
    {
        try
        {
            string fileName = resourceName;
            string filePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Res", fileName);
            File.WriteAllBytes(filePath, resourceData);
            RES_PATH = filePath;
            Console.WriteLine($"Resource data saved to: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save resource data: {ex.Message}");
        }
    }

    private static void ProcessBitmap(Bitmap bitmap, string resourceName)
    {
        try
        {
            Bitmap croppedBitmap = GaboonGrabber.Unpacker.CropBitmap(bitmap, 150, 150);
            byte[] croppedBitmapData = GaboonGrabber.Unpacker.ConvertBitmapToByteArray(croppedBitmap);

            string croppedFileName = $"cropped_{resourceName}.png";
            string croppedFilePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Res", croppedFileName);
            File.WriteAllBytes(croppedFilePath, croppedBitmapData);
            Console.WriteLine($"Cropped bitmap saved to: {croppedFilePath}");
            TEMP_PATH = croppedFilePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to process bitmap: {ex.Message}");
        }
    }

    private static void CheckAndPrintMatches(string input, string location, string pattern)
    {
        var matches = Regex.Matches(input, pattern);
        if (matches.Count > 0)
        {
            Console.WriteLine($"Location: {location}");
            for (int i = 0; i < Math.Min(matches.Count, 3); i++)
            {
                Match match = matches[i];
                Console.WriteLine(match.Value);
                TEMP_KEY = match.Value;
            }
        }
    }
}
