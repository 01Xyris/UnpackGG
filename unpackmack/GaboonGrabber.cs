using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

public class GaboonGrabber : Config
{
    public static string TEMP => Work.TEMP_KEY;
    public static string KEY1;
    public static string KEY2;
    public static string TYRONE;
    public static string RES_PATH => Work.RES_PATH;
    public static string TEMP_PATH => Work.TEMP_PATH;
    public override string Name => "GaboonGrabber";
    public override List<string> Patterns => new List<string>
    {
        @"[0-9A-Fa-f]+_[0-9A-Fa-f]+",
        @"[0-9A-Fa-f]+\+[0-9A-Fa-f]+"
    };

    public override void Unpack(ModuleDefMD module)
    {
        // Falling asleep will make better later (maybe)
        if(TYRONE == "True")
        {
            Unpacker.UnpackPayload(module);
        }
        else
        {
  Unpacker.CheckAndPrintMatches();
        Console.WriteLine("Unpacking GaboonGrabber...");
        Console.WriteLine($"TEMP_PATH -> {TEMP_PATH}");
        string finalFileName = "res3.exe";
        string finalFilePath = Path.Combine(Directory.GetCurrentDirectory(), finalFileName);
        byte[] cropped = File.ReadAllBytes(TEMP_PATH);
        File.WriteAllBytes(finalFilePath, Utility.DecryptBytes(cropped, KEY2));

        var module_res = ModuleDefMD.Load(finalFilePath);
        Unpacker.UnpackPayload(module_res);
        }
      
    }

    public class Unpacker
    {
        public static void CheckAndPrintMatches()
        {
            string hexString = TEMP;
            string[] parts = hexString.Split(new[] { '_', '+' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                KEY1 = StringManipulator.ConvertHexToString(parts[0]);
                Console.WriteLine($"KEY1: {KEY1}");
                KEY2 = StringManipulator.ConvertHexToString(parts[1]);
                Console.WriteLine($"KEY2: {KEY2}");
            }
        }

        public static byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            int byteIndex = 0;
            int width = bitmap.Width;
            int arraySize = width * width * 4;
            byte[] byteArray = new byte[arraySize];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    Array.Copy(BitConverter.GetBytes(bitmap.GetPixel(x, y).ToArgb()), 0, byteArray, byteIndex, 4);
                    byteIndex += 4;
                }
            }

            int dataSize = BitConverter.ToInt32(byteArray, 0);
            byte[] resultArray = new byte[dataSize];
            Array.Copy(byteArray, 4, resultArray, 0, resultArray.Length);

            return resultArray;
        }

        public static Bitmap CropBitmap(Bitmap sourceBitmap, int startX, int startY)
        {
            int croppedWidth = sourceBitmap.Width - startX;
            int croppedHeight = sourceBitmap.Height - startY;
            Bitmap croppedBitmap = new Bitmap(croppedWidth, croppedHeight);

            for (int y = 0; y < croppedHeight; y++)
            {
                for (int x = 0; x < croppedWidth; x++)
                {
                    Color pixelColor = sourceBitmap.GetPixel(x, y);
                    croppedBitmap.SetPixel(x, y, pixelColor);
                }
            }

            return croppedBitmap;
        }

        public static void UnpackPayload(ModuleDefMD module)
        {
            Console.WriteLine("Unpacking GaboonGrabber Final stage...");
            Thread.Sleep(2000);
            string currentFilePath = Directory.GetCurrentDirectory();
            string noFuserExPath = Path.Combine(currentFilePath + @"\NoFuserEx", "NoFuserEx.exe");
            Console.WriteLine("noFuserExPath: " + noFuserExPath);
            string res3Path = Path.Combine(currentFilePath, "res3.exe");
            Console.WriteLine("res3Path: " + res3Path);

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = noFuserExPath,
                Arguments = $"{res3Path} --force-deob",
                UseShellExecute = true,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                process.WaitForExit();
            }
            Console.WriteLine("Unpacking completed.");

            string noFuserExOutputPath = Path.Combine(currentFilePath, "NoFuserEx_Output");

      
            string unpackedFilePath = Path.Combine(noFuserExOutputPath, "res3.exe");
            Work.SearchResource(ModuleDefMD.Load(unpackedFilePath));
            if (File.Exists(unpackedFilePath))
            {
                var unpackedModule = ModuleDefMD.Load(unpackedFilePath);
                SearchString(unpackedModule);
            }
            else
            {
                Console.WriteLine("Unpacked file not found.");
            }
        }

        public static void SearchString(ModuleDefMD module)
        {
            string pattern = @"[a-zA-Z0-9]{11}";  
            Console.WriteLine($"RES_PATH: {RES_PATH} ");
            foreach (var type in module.GetTypes())
            {
                var cctorMethod = type.FindStaticConstructor();
                if (cctorMethod != null && cctorMethod.HasBody)
                {
                    foreach (var instruction in cctorMethod.Body.Instructions)
                    {
                        if (instruction.OpCode == OpCodes.Ldstr && instruction.Operand is string stringOperand)
                        {
                            if (Regex.IsMatch(stringOperand, pattern))
                            {
                                Console.WriteLine($"Found matching string: {stringOperand} in method {cctorMethod.Name}");
                                byte[] result = Utility.Xor(File.ReadAllBytes(RES_PATH), stringOperand);
                                string finalFileName = $"result_{stringOperand}.bin";
                                string finalFilePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Output", finalFileName);
                                File.WriteAllBytes(finalFilePath, result);
                                Console.WriteLine($"Saved result to: {finalFilePath}");
                            }
                        }
                    }
                }
            }
        }
    }
}
