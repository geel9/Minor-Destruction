using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace MiningGame.Code.Managers
{
    public static class FileWriterManager
    {

        public static bool WriteFile(string path, string contents)
        {
            try
            {
                TextWriter fw = new StreamWriter(path);
                fw.Write(contents);
                fw.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool AppendFile(string path, string contents)
        {
            try
            {
                string current = FileReaderManager.ReadFileContents(path);
                string newContents = current + contents;
                WriteFile(path, newContents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
