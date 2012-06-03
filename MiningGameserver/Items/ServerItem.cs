using System.Collections.Generic;
using System.Linq;
using MiningGameServer;
using MiningGameServer.Items;

namespace MiningGameserver.Items
{
    public abstract class ServerItem
    {
        public static List<ServerItem> Items = new List<ServerItem>();

        public static void MakeItems()
        {
            Items.Clear();
            new ServerItemDirt();
            new ServerItemRock();
            new ServerItemCoal();
            new ServerItemIron();
            new ServerItemDoor();
            new ServerItemPlank();
            new ServerItemDino();
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

        public ServerItem SetBlockID(byte id)
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
            }
        }

        public byte GetBlockID()
        {
            return _blockID;
        }

        private string _itemName;
        private int _itemWorth;
        private string _itemDescription;
        private byte _itemID;
        private byte _blockID = 0;
    }

    public struct ItemStack
    {
        public int numberItems;
        public byte itemID;

        public ItemStack(int n = 0, byte b = 0)
        {
            numberItems = n;
            itemID = b;
        }
    }
}
