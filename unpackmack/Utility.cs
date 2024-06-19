using System;
using System.Text;

public static class Utility
{
    internal static Encoding GetBigEndianUnicodeEncoding()
    {
        return Encoding.BigEndianUnicode;
    }

    public static byte[] Xor(byte[] byte_0, string string_0)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(string_0);
        for (int i = 0; i <= byte_0.Length; i++)
        {
            byte_0[i % byte_0.Length] = Convert.ToByte((Convert.ToInt32((int)(byte_0[i % byte_0.Length] ^ bytes[i % bytes.Length])) - Convert.ToInt32(byte_0[(i + 1) % byte_0.Length]) + 256) % 256);
        }
        Array.Resize<byte>(ref byte_0, byte_0.Length - 1);
        return byte_0;
    }

    public static byte[] DecryptBytes(byte[] byte_0, string string_0)
    {
        byte[] bytes = GetBigEndianUnicodeEncoding().GetBytes(string_0);
        int num = (int)(byte_0[byte_0.Length - 1] ^ 112);
        byte[] array = new byte[byte_0.Length + 1];
        int num2 = 0;
        int num3 = 0;
        for (; ; )
        {
            int num4;
            if (num3 > byte_0.Length - 1)
            {
                num4 = 1;

                goto IL_62;
            }
            else
            {
                int num6 = (int)byte_0[num3] ^ num ^ (int)bytes[num2];
                array[num3] = (byte)num6;
                int num7 = num2;
                int num8 = num7;
                if (num8 != string_0.Length - 1)
                {
                    num2++;
                }
                else
                {
                    num2 = 0;
                    num4 = 0;

                    goto IL_62;

                }
            }
        IL_6F:
            num3++;
            continue;
        IL_62:
            switch (num4)
            {
                default:
                    goto IL_6F;
                case 1:
                    goto IL_94;
            }
        }
    IL_94:
        Array.Resize<byte>(ref array, byte_0.Length - 1);
        return array;
    }
}
