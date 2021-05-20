using System.Security.Cryptography;
using System.Text;

namespace Airslip.Security
{
    public static class Cryptography
    {
        public static string GenerateSHA256String(string value)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder result = new();
            foreach (byte t in hash)
                result.Append(t.ToString("X2"));
            return result.ToString();
        }
    }
}