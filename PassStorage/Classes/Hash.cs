using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PassStorage.Classes
{
    internal class Hash
    {
        public static string hash(string password)
        {
            SHA512Managed crypt = new SHA512Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte bit in crypto)
            {
                hash += bit.ToString("x2");
            }
            return hash;
        }

        public static bool check(string password, string hash)
        {
            password = Hash.hash(password);

            if (password == hash) return true;
            else return false;
        }
    }
}
