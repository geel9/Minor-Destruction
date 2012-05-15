using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MiningGame.Code.Managers
{
    public static class HashManager
    {
        public static string MD5(string toHash)
        {
            
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(toHash);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }
    }
}
