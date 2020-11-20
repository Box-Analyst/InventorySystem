#region copyright

// Copyright (c) Box Analyst. All rights reserved.
// This code is licensed under the GNU AGPLv3 License.

#endregion copyright

using System;
using System.Security.Cryptography;
using System.Text;

namespace InventorySystem.Views.Login
{
    internal class PasswordHash
    {
        private static string hash, salt;
        private static byte[] bytesToHash, saltBytes;

        public PasswordHash(string password)
        {
            hash = password;
            bytesToHash = Encoding.UTF8.GetBytes(hash);
        }

        //Creates the Hash after calling the CreateSalt function
        private static string CreateHash()
        {
            CreateSalt();
            var byteResult = new Rfc2898DeriveBytes(bytesToHash, saltBytes, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(64));
        }

        //Overload for CreateHash that is used to check passwords with the current
        //employee's stored salt
        private static string CreateHash(string salt)
        {
            saltBytes = Convert.FromBase64String(salt);
            var byteResult = new Rfc2898DeriveBytes(bytesToHash, saltBytes, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(64));
        }

        //Creates the salt used to generate a hash for the employee's password
        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            var bytes = new byte[128 / 8];
            rng.GetBytes(bytes);
            saltBytes = bytes;
            salt = Convert.ToBase64String(bytes);
            return salt;
        }

        //Default method for setting a new hash (AddUsers)
        public void SetHash()
        {
            hash = CreateHash();
        }

        //Overload for SetHash for checking passwords with employee's stored salt
        public void SetHash(string salt)
        {
            hash = CreateHash(salt);
        }

        public string GetHash()
        {
            return hash;
        }

        public string GetSalt()
        {
            return salt;
        }
    }
}