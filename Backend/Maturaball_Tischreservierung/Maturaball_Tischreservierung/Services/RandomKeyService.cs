using System.Security.Cryptography;

namespace Maturaball_Tischreservierung.Services
{
    public class RandomKeyService
    {
        public string GetRandomKey(int length)
        {
            // chars to bytes
            int randomByteCount = (int)Math.Floor(length * 6d / 8d);
            byte[] randomBytes = RandomNumberGenerator.GetBytes(randomByteCount);
            return Convert.ToBase64String(randomBytes);
        }
    }
}