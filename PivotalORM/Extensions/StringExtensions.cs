using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotalORM
{
    public static class StringExtensions
    {
        public static string Underscored(this string s)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < s.Length; ++i)
            {
                if (ShouldUnderscore(i, s))
                {
                    builder.Append('_');
                }

                //builder.Append(char.ToLowerInvariant(s[i]));
                builder.Append(s[i]);
            }

            return builder.ToString();
            //return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(builder.ToString());
        }

        private static bool ShouldUnderscore(int i, string s)
        {
            if (i == 0 || i >= s.Length || s[i] == '_')
            {
                return false;
            }

            var curr = s[i];
            var prev = s[i - 1];
            var next = i < s.Length - 2 ? s[i + 1] : '_';

            return prev != '_' && ((char.IsUpper(curr) && (char.IsLower(prev) || char.IsLower(next))) ||
                (char.IsNumber(curr) && (!char.IsNumber(prev))));
        }

        public static string ToASCII(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback(string.Empty),
                        new DecoderExceptionFallback()
                        ),
                    Encoding.UTF8.GetBytes(str)
                )
            );
        }

        public static string ToANSI(this string str)
        {
            int defaultCode = 1252;
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return Encoding.GetEncoding(defaultCode).GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        defaultCode,
                        new EncoderReplacementFallback(string.Empty),
                        new DecoderExceptionFallback()
                        ),
                    Encoding.UTF8.GetBytes(str)
                )
            );
        }
    }
}
