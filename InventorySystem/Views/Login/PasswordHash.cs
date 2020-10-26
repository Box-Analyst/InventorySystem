using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Views.Login
{
    class PasswordHash
    {
        private string hash;
        public PasswordHash(string password)
        {
            hash = password;
        }
        private string CreateHash()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hash));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void SetHash()
        {
            hash = CreateHash();
        }
        public string GetHash()
        {
            return hash;
        }
    }
}
