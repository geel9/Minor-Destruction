using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;
namespace MiningGame.Code.Managers
{
    public static class ConfigChangerManager
    {
        public static List<KeyValue> RemoveKeyValue(List<KeyValue> listKV, string name)
        {
            List<KeyValue> kvs = new List<KeyValue>();
            foreach (KeyValue kv in listKV)
            {
                if (kv.name != name)
                {
                    kvs.Add(kv);
                }
            }
            return kvs;
        }

        public static List<KeyValue> ChangeKeyValue(List<KeyValue> listKV, string name, string newValue)
        {
            List<KeyValue> kvs = new List<KeyValue>();
            foreach (KeyValue kv in listKV)
            {
                KeyValue k = new KeyValue();
                if (kv.name == name)
                {
                    k.value = newValue;
                    k.name = name;
                }
                else
                {
                    k.value = kv.value;
                    k.name = kv.name;
                }
                kvs.Add(k);
            }
            return kvs;
        }

        public static List<KeyValue> AddKeyValue(List<KeyValue> listKV, KeyValue key)
        {
            List<KeyValue> kvs = listKV;
            bool exists = false;
            foreach (KeyValue kv in kvs)
            {
                if (kv.name == key.name)
                {
                    exists = true;
                }
            }

            if (!exists)
            {
                kvs.Add(key);
            }
            return kvs;
        }


    }
}
