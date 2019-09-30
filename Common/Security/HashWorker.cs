using System;
using System.Security.Cryptography;
using System.Text;

namespace Common.Security
{
    public static class HashWorker
    {
        /// <summary>
        ///     Hashes a Password with a given Salt or uses a Randome Salt if none was specified. The Hashalgorith used is SHA512
        /// </summary>
        /// <param name="password">The Password to be hashed</param>
        /// <param name="salt">A Salt, if none is specified a new Randome Salt will be created</param>
        /// <returns>Returns a Tuple with the hashed Password as Item1 and the Salt as Item2</returns>
        public static Tuple<string, string> HashPassword(this string password, string salt = null)
        {
            //generate randome salt
            if (salt == null)
            {
                var rng = new Random(DateTime.Now.Millisecond);
                var saltBytes = new byte[16];
                rng.NextBytes(saltBytes);
                salt = Convert.ToBase64String(saltBytes);
            }

            //generate salted and hashed password
            var sha = SHA512.Create();
            var saltedPassword = password + salt;
            var saltedHashedPassword =
                Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPassword)));

            return new Tuple<string, string>(saltedHashedPassword, salt);
        }

        /// <summary>
        ///     Validateds if a password matches the hashed Password with a gived Salt
        /// </summary>
        /// <param name="password">The plaintext password</param>
        /// <param name="hashedPassword">The hashed and salted password</param>
        /// <param name="salt">The salt used</param>
        /// <returns>Returns true if the passwords match</returns>
        public static bool ValidatePassword(this string password, string hashedPassword, string salt)
        {
            var saltedHashedPassword = password.HashPassword(salt).Item1;

            return saltedHashedPassword == hashedPassword;
        }
    }
}