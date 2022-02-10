using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PaperTrading.Helper
{
    public static class Helpers
    {
        public static string Sha256(this string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string SplitNumber3Digits(this decimal num, bool hasDecimal = true)
        {
            var res = (num - num % (decimal)1).ToString("N0");

            if (hasDecimal)
                res += (num % (decimal)1).ToString().Remove(0, 1);

            return res;
        }
    }
}
