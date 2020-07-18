using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace Yaba.Infrastructure.Security
{
    public static class SecurityManager
    {
        public static string GeneratePbkdf2Hash(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey
            var hash = KeyDerivation.Pbkdf2(
                        password: password,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA512,
                        iterationCount: 3, // TODO: make this a option (Options pattern)
                        numBytesRequested: 256 / 8);

            var passwordHash = Convert.ToBase64String(hash);
            var passwordSalt = Convert.ToBase64String(salt);

            // TODO: save it in different columns ?
            return $"{passwordSalt}.{passwordHash}";
        }
    }
}
