using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Prefix.Id
{
    public static class Base32
    {
        private const string Separator = "-";
        private static readonly Dictionary<char, int> CharMap = new Dictionary<char, int>();
        private static readonly char[] Digits;
        private static readonly int Mask;
        private static readonly int Shift;

        static Base32()
        {
            Digits = "abcdefghijklmnopqrstuvwxyz234567".ToCharArray();
            Mask = Digits.Length - 1;
            Shift = NumberOfTrailingZeros(Digits.Length);
            for (var i = 0; i < Digits.Length; i++) CharMap[Digits[i]] = i;
        }

        public static byte[] Decode(string encoded)
        {
            encoded = encoded.Trim().Replace(Separator, "");
            encoded = Regex.Replace(encoded, "[=]*$", "");
            encoded = encoded.ToLower();
            
            if (encoded.Length == 0) return Array.Empty<byte>();
            int encodedLength = encoded.Length;
            int outLength = encodedLength * Shift / 8;
            var result = new byte[outLength];
            var buffer = 0;
            var next = 0;
            var bitsLeft = 0;
            foreach (char c in encoded)
            {
                if (!CharMap.ContainsKey(c)) throw new DecodingException("Illegal character: " + c);
                buffer <<= Shift;
                buffer |= CharMap[c] & Mask;
                bitsLeft += Shift;
                if (bitsLeft >= 8)
                {
                    result[next++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }

            return result;
        }

        public static string Encode(byte[] data, bool padOutput = false)
        {
            if (data.Length == 0) return "";

            // SHIFT is the number of bits per output character, so the length of the
            // output is the length of the input multiplied by 8/SHIFT, rounded up.
            if (data.Length >= 1 << 28) throw new ArgumentOutOfRangeException(nameof(data));

            int outputLength = (data.Length * 8 + Shift - 1) / Shift;
            var result = new StringBuilder(outputLength);

            int buffer = data[0];
            var next = 1;
            var bitsLeft = 8;
            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < Shift)
                {
                    if (next < data.Length)
                    {
                        buffer <<= 8;
                        buffer |= data[next++] & 0xff;
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = Shift - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }

                int index = Mask & (buffer >> (bitsLeft - Shift));
                bitsLeft -= Shift;
                result.Append(Digits[index]);
            }

            if (padOutput)
            {
                int padding = 8 - result.Length % 8;
                if (padding > 0) result.Append(new string('=', padding == 8 ? 0 : padding));
            }

            return result.ToString();
        }

        private static int NumberOfTrailingZeros(int i)
        {
            // HD, Figure 5-14
            if (i == 0) return 32;
            var n = 31;
            int y = i << 16;
            if (y != 0)
            {
                n -= 16;
                i = y;
            }

            y = i << 8;
            if (y != 0)
            {
                n -= 8;
                i = y;
            }

            y = i << 4;
            if (y != 0)
            {
                n -= 4;
                i = y;
            }

            y = i << 2;
            if (y != 0)
            {
                n -= 2;
                i = y;
            }

            return n - (int)((uint)(i << 1) >> 31);
        }

        private class DecodingException : Exception
        {
            public DecodingException(string message) : base(message)
            { }
        }
    }
}
