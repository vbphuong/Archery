using System.Buffers.Binary;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Archery.Handlers
{
    public static class PasswordHashHandler
    {
        private static int _iterationCount = 100000;
        private static RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

        public static string HashPassword(string password)
        {
            int saltSize = 128 / 8;
            var salt = new byte[saltSize];
            _randomNumberGenerator.GetBytes(salt);
            var subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, _iterationCount, 256 / 8);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01;

            BinaryPrimitives.WriteInt32BigEndian(outputBytes.AsSpan(1), (int)KeyDerivationPrf.HMACSHA512); // Replace 1st WriteNetworkByteOrder
            BinaryPrimitives.WriteInt32BigEndian(outputBytes.AsSpan(5), (int)_iterationCount);            // Replace 2nd WriteNetworkByteOrder
            BinaryPrimitives.WriteInt32BigEndian(outputBytes.AsSpan(9), (int)saltSize);                 // Replace 3rd WriteNetworkByteOrder

            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);

            return Convert.ToBase64String(outputBytes);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            try
            {
                var hashedPassword = Convert.FromBase64String(hash);
                var keyDerivationPrf = (KeyDerivationPrf)BinaryPrimitives.ReadInt32BigEndian(hashedPassword.AsSpan(1)); // Offset 1
                var iterationCount = (int)BinaryPrimitives.ReadInt32BigEndian(hashedPassword.AsSpan(5));              // Offset 5
                var saltLength = (int)BinaryPrimitives.ReadInt32BigEndian(hashedPassword.AsSpan(9));                 // Offset 9

                if (saltLength < 128 / 8) return false;

                // Extract salt and subkey
                var salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, saltLength);

                var expectedSubkeyLength = 256 / 8;
                var expectedSubkey = new byte[expectedSubkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + saltLength, expectedSubkey, 0, expectedSubkeyLength);

                // Derive new subkey from the password and salt
                var newSubkey = KeyDerivation.Pbkdf2(password, salt, keyDerivationPrf, iterationCount, expectedSubkeyLength);

                // Compare the derived subkey with the expected subkey
                return CryptographicOperations.FixedTimeEquals(newSubkey, expectedSubkey);
            }
            catch
            {
                return false;
            }
        }
    }

    //class Program
    //{
    //    static void Main()
    //    {
    //        string password = "admin123";
    //        string hashedPassword = PasswordHashHandler.HashPassword(password);
    //        Console.WriteLine(hashedPassword);
    //    }
    //}
}