using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TrainingProject.Domain.Logic
{
   internal class CryptographyHelper
    {
        public static string GetSha256Hash(string input)
        {
            var sha256 = SHA256.Create();
            byte[] hashInBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            var result = new StringBuilder();
            foreach (var hashByte in hashInBytes)
            {
                result.Append(hashByte.ToString("x2"));
            }

            return result.ToString();
        }
    }
}
