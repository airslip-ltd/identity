using System;
using System.Security.Cryptography;

namespace Airslip.Security
{
    public static class RefreshToken
    {
        public static string Generate()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator numberGenerator = RandomNumberGenerator.Create();
            numberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}