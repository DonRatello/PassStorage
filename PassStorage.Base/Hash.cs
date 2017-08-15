using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PassStorage.Base
{
    public class Hash
    {
        public static string HashValue(string password)
        {
            SHA512Managed crypt = new SHA512Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            return crypto.Aggregate(hash, (current, bit) => current + bit.ToString("x2"));
        }

        public static bool Check(string password, string hash)
        {
            return HashValue(password).Equals(hash);
        }
    }
}
