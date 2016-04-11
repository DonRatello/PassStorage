using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PassStorage.Classes
{
    public class Hash
    {
        public static string hash(string password)
        {
            SHA512Managed crypt = new SHA512Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            return crypto.Aggregate(hash, (current, bit) => current + bit.ToString("x2"));
        }

        public static bool check(string password, string hash)
        {
            password = Hash.hash(password);
            return password == hash;
        }
    }
}
