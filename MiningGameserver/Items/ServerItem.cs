using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiningGameServer;
using MiningGameServer.Items;
using MiningGameServer.Managers;

namespace MiningGameServer.Items
{
    public abstract class ServerItem
    {
        public static List<ServerItem> Items = new List<ServerItem>();

        public static string GenerateItemHTMLTable()
        {
            string pre = "<html><body>" +
                         "<table border='1'>" +
                         "<tr><th>ID</th>" +
                         "<th>Name</th></tr>";
            string end = "</table></body></html>";
            string mid = "";
            ServerItem[] items = Items.ToArray();
            Array.Sort(items, (x, y) => x.GetItemID() - y.GetItemID());

            foreach (ServerItem i in items)
            {
                if (i.GetItemID() == 0) continue;
                mid += "<tr><td>" +
                       i.GetItemID() +
                       "</td><td>" +
                       i.GetName() +
                       "</td></tr>";
            }

            return pre + mid + end;
        }

        public static void MakeItems()
        {
            Items = new List<ServerItem>();
            //Replacing the manual method of adding them
            //With reflection.
            Type[] types = ReflectionManager.GetAllSubClassesOf<ServerItem>();
            foreach(Type t in types)
            {
                ReflectionManager.CallConstructor(t);
            }
        }

        protected ServerItem()
        {
            Items.Add(this);
        }

        public ServerItem SetValue(int value)
        {
            _itemWorth = value;
            return this;
        }
        
        public ServerItem SetName(string name)
        {
            _itemName = name;
            return this;
        }

        public ServerItem SetMaxStack(int max)
        {
            _maxStack = max;
            return this;
        }
     

        public ServerItem SetID(byte id)
        {
            this._itemID = id;
            return this;
        }

        public ServerItem SetDescription(string description)
        {
            this._itemDescription = description;
            return this;
        }

        public ServerItem SetBlockID(short id)
        {
            _blockID = id;
            return this;
        }

        public static ServerItem GetItem(byte id)
        {
            return Items.Where(x => x._itemID == id).FirstOrDefault();
        }

        public int GetValue()
        {
            return _itemWorth;
        }

        public int GetMaxStack()
        {
            return _maxStack;
        }

        public string GetName()
        {
            return _itemName;
        }

        public string GetDescription()
        {
            return _itemDescription;
        }

        public byte GetItemID()
        {
            return _itemID;
        }

        public virtual void OnItemUsed(int x, int y, NetworkPlayer user)
        {
            short block = GameServer.GetBlockAt(x, y).ID;
            if(block == 0 && _blockID != 0)
            {
                GameServer.SetBlock(user, x, y, _blockID);
                user.Inventory.RemoveItems(_itemID, 1);
            }
        }

        public short GetBlockID()
        {
            return _blockID;
        }

        private string _itemName;
        private int _itemWorth;
        private string _itemDescription;
        private byte _itemID;
        private short _blockID = 0;
        private int _maxStack = 20;
        public int InventorySection = 0;
    }
}
