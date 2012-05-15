using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Managers
{
    public static class EncryptionManager
    {
        public static int getSumOfChars(string chars)
        {
            int sum = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                sum += chars[i];
            }
            return sum;
        }

        public static int getOffsetFromKeySum(int sum)
        {
            if (sum % 3 == 0)
            {
                return sum / 3;
            }
            else
            {
                return sum / 2;
            }
        }

        public static string Encrypt(string toEncrypt, string key)
        {
            List<char> chars = new List<char>();
            List<int> ints = new List<int>();
            string ret = "";
            string newKey = HashManager.MD5(key);
            string totalSumString = getSumOfChars(key).ToString() + getSumOfChars(newKey).ToString();
            foreach (char c in totalSumString)
            {
                string c2 = c.ToString();
                ints.Add(Convert.ToInt32(c2));
            }
            for (int i = 0; i < toEncrypt.Length; i++)
            {
                chars.Add(toEncrypt[i]);
            }
            int im = 0;
            Random r = new Random();
            foreach (char c in chars)
            {
                if (im >= ints.Count)
                    im = 0;
                int todo = ints[im];
                char b = new char();
                bool doing = false;
                if (todo % 3 == 0)
                {
                    b = (char)r.Next(0, 300 + todo);
                    doing = true;
                }
                ret += (char)(c + todo);
                if (doing)
                    ret += (char)b;
                im++;
            }
            return ret;
        }

        public static string Decrypt(string encrypted, string key)
        {
            List<char> chars = new List<char>();
            List<int> ints = new List<int>();
            string ret = "";
            string newKey = HashManager.MD5(key);
            string totalSumString = getSumOfChars(key).ToString() + getSumOfChars(newKey).ToString();
            foreach (char c in totalSumString)
            {
                string c2 = c.ToString();
                ints.Add(Convert.ToInt32(c2));
            }
            for (int i = 0; i < encrypted.Length; i++)
            {
                chars.Add(encrypted[i]);
            }
            int im = 0;
            bool skipping = false;
            foreach (char c in chars)
            {
                if (!skipping)
                {
                    if (im >= ints.Count)
                        im = 0;
                    int todo = ints[im];
                    skipping = (todo % 3 == 0);
                    ret += (char)(c - todo);
                    im++;
                }
                else
                {
                    skipping = false;
                }
            }
            return ret;
        }

    }
}
