using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DMWEditor
{
    public static class Hash
    {
        public static string Calculate(byte[] arr)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] ba = md5.ComputeHash(arr);
                StringBuilder hashedSB = new StringBuilder(ba.Length * 2);
                foreach (byte b in ba)
                    hashedSB.AppendFormat("{0:x2}", b);
                return hashedSB.ToString().ToUpper();
            }
        }
    }
}
