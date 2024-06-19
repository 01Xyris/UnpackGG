using System;
using System.Text;

public static class StringManipulator
{
    public static string ConvertHexToString(string hexString)
    {
        StringBuilder result = new StringBuilder(hexString.Length / 2);
        checked
        {
            int length = hexString.Length - 2;
            int index = 0;
            while (true)
            {
                bool continueLoop = index <= length;
                if (!continueLoop)
                {
                    break;
                }
                AppendHexChar(result, hexString, index);
                index += 2;
            }
        }
        return result.ToString();
    }

    internal static char ConvertIntToChar(int intValue)
    {
        return Convert.ToChar(intValue);
    }

    private static StringBuilder AppendHexChar(StringBuilder stringBuilder, string hexString, int startIndex)
    {
        return stringBuilder.Append(ConvertIntToChar((int)Convert.ToUInt32(hexString.Substring(startIndex, 2), 16)));
    }
}
