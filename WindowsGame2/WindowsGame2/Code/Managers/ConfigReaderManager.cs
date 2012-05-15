using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MiningGame.Code.Structs;
namespace MiningGame.Code.Managers
{
    public static class ConfigReaderManager
    {

        public static List<KeyValue> ReadFileKeyValues(string path)
        {
            return ReadStringKeyValues(FileReaderManager.ReadFileContents(path));
        }

        public static KeyValue ReadFileKeyValue(string path, string name)
        {
            return ReadStringKeyValue(FileReaderManager.ReadFileContents(path), name);
        }

        public static KeyValue ReadStringKeyValue(string contents, string name)
        {
            foreach (KeyValue kv in ReadStringKeyValues(contents))
            {
                if (kv.name == name)
                {
                    return kv;
                }
            }
            return new KeyValue("", "");
        }

        public static bool KeyValueExistsFile(string path, string name)
        {
            return KeyValueExistsString(FileReaderManager.ReadFileContents(path), name);
        }

        public static bool KeyValueExistsString(string contents, string name)
        {
            if (ReadStringKeyValue(contents, name).value != "" || ReadStringKeyValue(contents, name).name != "")
            {
                return true;
            }
            return false;
        }

        public static List<KeyValue> ReadStringKeyValues(string contents)
        {
            List<KeyValue> ret = new List<KeyValue>();
            string regex = "[\\t ]+?\"(.+?)\"[\\t ]+?\"(.+?)\"";
            Regex matcher = new Regex(regex);
            MatchCollection mc = matcher.Matches(contents);
            foreach (Match m in mc)
            {
                KeyValue kv = new KeyValue(m.Groups[1].Value, m.Groups[2].Value);
                ret.Add(kv);
            }
            return ret;
        }

    }
}
