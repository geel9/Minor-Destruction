using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;
namespace MiningGame.Code.Managers
{
    public static class ConfigWriterManager
    {

        public static void WriteConfigFile(string path, List<KeyValue> keyvalues)
        {
            string contents = GenerateContentFile(keyvalues);
            FileWriterManager.WriteFile(path, contents);
        }

        public static string GenerateContentFile(List<KeyValue> keyvalues)
        {
            string contents = "";
            foreach (KeyValue kv in keyvalues)
            {
                contents += "\t\"" + kv.name + "\"\t\"" + kv.value + "\"\n";
            }
            return contents;
        }

        public static void RemoveKeyValue(string path, string name)
        {
            if (ConfigReaderManager.KeyValueExistsFile(path, name))
            {
                List<KeyValue> kvs = ConfigReaderManager.ReadFileKeyValues(path);
                List<KeyValue> NewKVs = new List<KeyValue>();
                foreach (KeyValue kv in kvs)
                {
                    if (kv.name != name)
                    {
                        NewKVs.Add(kv);
                    }
                }
                WriteConfigFile(path, NewKVs);
            }
        }

        public static void ChangeKeyValue(string path, string name, string newValue)
        {
            if (ConfigReaderManager.KeyValueExistsFile(path, name))
            {
                List<KeyValue> kvs = ConfigReaderManager.ReadFileKeyValues(path);
                List<KeyValue> NewKVs = new List<KeyValue>();

                foreach (KeyValue kv in kvs)
                {
                    KeyValue k = new KeyValue(kv.name, kv.value);
                    if (k.name == name)
                    {
                        k.value = newValue;
                    }
                    NewKVs.Add(k);
                }
                WriteConfigFile(path, NewKVs);
            }
        }

        public static void AddKeyValue(string path, KeyValue kv)
        {
            if (!ConfigReaderManager.KeyValueExistsFile(path, kv.name))
            {
                List<KeyValue> kvs = ConfigReaderManager.ReadFileKeyValues(path);
                kvs.Add(kv);
                WriteConfigFile(path, kvs);
            }
        }

    }
}
