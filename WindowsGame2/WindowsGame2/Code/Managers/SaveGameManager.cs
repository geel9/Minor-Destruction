using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiningGame.Code.Structs;
namespace MiningGame.Code.Managers
{
    public static class SaveGameManager
    {
        public static string name = "";


        public static string GetPath()
        {
            return DirectoryManager.SAVEGAMES + name + ".sv";
        }

        public static bool CheckExists(string name)
        {
            return GetFiles().Contains(name);
        }

        public static bool CreateFile(string newName)
        {
            if (CheckExists(newName))
                return false;
            else
            {
                File.Create(DirectoryManager.SAVEGAMES + newName + ".sv");
                SetName(newName);
                ConsoleManager.Log("Created new name " + newName);
                return true;
            }
        }

        public static string[] GetFiles()
        {
            string path = DirectoryManager.SAVEGAMES;
            string[] files = Directory.GetFiles(path, "*.sv");
            string[] ret = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                ret[i] = files[i].Replace(".sv", "").Replace(DirectoryManager.SAVEGAMES, "");
            }
            return ret;
        }

        public static void SetName(string newName)
        {
            name = newName;
        }

        private static string GetUnencryptedContents()
        {
            string Encrypted = FileReaderManager.ReadFileContents(DirectoryManager.SAVEGAMES + name + ".sv");
            string unEncrypted = EncryptionManager.Decrypt(Encrypted, name);
            return unEncrypted;
        }

        public static string GetValue(string keyValue)
        {
            string ret = "";
            if (CheckExists(name))
            {
                string unEncrypted = GetUnencryptedContents();
                ret = ConfigReaderManager.ReadStringKeyValue(unEncrypted, keyValue).value;
            }

            return ret;
        }
        public static List<KeyValue> GetKeyValues()
        {
            List<KeyValue> ret = new List<KeyValue>();
            if (CheckExists(name))
            {
                string unEncrypted = GetUnencryptedContents();
                ret = ConfigReaderManager.ReadStringKeyValues(unEncrypted);
            }

            return ret;
        }

        public static bool KeyValueExists(string keyValue)
        {
            string contents = GetUnencryptedContents();
            return ConfigReaderManager.KeyValueExistsString(contents, keyValue);

        }

        public static void WriteSaveFile(List<KeyValue> kvs)
        {
            string unenc = ConfigWriterManager.GenerateContentFile(kvs);
            string enc = EncryptionManager.Encrypt(unenc, name);
            FileWriterManager.WriteFile(GetPath(), enc);
        }

        public static void SetValue(string keyValue, string newValue)
        {
            if (CheckExists(name))
            {
                ConsoleManager.Log("name exists");
                string contents = GetUnencryptedContents();
                List<KeyValue> kvs = ConfigReaderManager.ReadStringKeyValues(contents);
                if (!KeyValueExists(keyValue))
                {
                    ConsoleManager.Log("Keyvalue " + keyValue + " does not exist.");
                    kvs = ConfigChangerManager.AddKeyValue(kvs, new KeyValue(keyValue, newValue));
                }
                else
                {
                    ConsoleManager.Log("Keyvalue " + keyValue + " exists.");
                    kvs = ConfigChangerManager.ChangeKeyValue(kvs, keyValue, newValue);
                }
                WriteSaveFile(kvs);
            }
        }

        public static void RemoveValue(string keyValue)
        {
            if (CheckExists(name))
            {
                string contents = GetUnencryptedContents();
                List<KeyValue> kvs = ConfigReaderManager.ReadStringKeyValues(contents);
                kvs = ConfigChangerManager.RemoveKeyValue(kvs, keyValue);
                WriteSaveFile(kvs);
            }
        }



    }
}
