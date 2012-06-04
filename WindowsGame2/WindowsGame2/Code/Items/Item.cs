using System.Collections.Generic;
using System.Linq;

namespace MiningGame.Code.Items
{
    public abstract class Item
    {
        public static List<Item> Items = new List<Item>();

        public static void MakeItems()
        {
            Items.Clear();
            new ItemDirt();
            new ItemRock();
            new ItemCoal();
            new ItemIron();
            new ItemDoor();
            new ItemPlank();
            new ItemDino();
            new ItemBow();
            new ItemSword();
        }

        protected Item()
        {
            Items.Add(this);
        }

        public Item SetValue(int value)
        {
            _itemWorth = value;
            return this;
        }
        
        public Item SetName(string name)
        {
            _itemName = name;
            return this;
        }
       
        public Item SetAsset(string asset)
        {
            _assetName = asset;
            return this;
        }

        public Item SetID(byte id)
        {
            this._itemID = id;
            return this;
        }

        public Item SetDescription(string description)
        {
            this._itemDescription = description;
            return this;
        }

        public Item SetBlockID(short id)
        {
            _blockID = id;
            return this;
        }

        public static Item GetItem(byte id)
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

        public string GetAsset()
        {
            return _assetName;
        }

        public string GetDescription()
        {
            return _itemDescription;
        }

        public byte GetItemID()
        {
            return _itemID;
        }

        public abstract void OnItemUsed(int x, int y);

        public short GetBlockID()
        {
            return _blockID;
        }

        private string _itemName;
        private int _itemWorth;
        private string _assetName;
        private string _itemDescription;
        private byte _itemID;
        private short _blockID = 0;
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
