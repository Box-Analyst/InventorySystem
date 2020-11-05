using System.Security.Cryptography;
using System.Text;

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
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
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
