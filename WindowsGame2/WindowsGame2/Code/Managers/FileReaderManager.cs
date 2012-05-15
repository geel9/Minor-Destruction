using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace MiningGame.Code.Managers
{
    public static class FileReaderManager
    {

        public static string ReadFileContents(string path)
        {
            try
            {
                TextReader tr = new StreamReader(path);
                string contents = tr.ReadToEnd();
                tr.Close();
                return contents;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string[] ReadFileLines(string path)
        {
            return ReadFileContents(path).Replace("\r","").Split('\n');
        }
    }
}
