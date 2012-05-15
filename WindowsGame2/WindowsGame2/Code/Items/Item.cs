using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Items;

namespace MiningGame.Code
{
    public abstract class Item
    {
        public static List<Item> items = new List<Item>();

        public static void makeItems()
        {
            items.Clear();
            Item dirt = new ItemDirt();
            Item rock = new ItemRock();
            Item coal = new ItemCoal();
            Item iron = new ItemIron();
            Item door = new ItemDoor();
            Item plank = new ItemPlank();
            Item dino = new ItemDino();
        }

        public Item()
        {
            items.Add(this);
        }

        public Item setValue(int value)
        {
            this.itemWorth = value;
            return this;
        }
        
        public Item setName(string name)
        {
            itemName = name;
            return this;
        }
       
        public Item setAsset(string asset)
        {
            assetName = asset;
            return this;
        }

        public Item setID(byte id)
        {
            this.itemID = id;
            return this;
        }

        public Item setDescription(string description)
        {
            this.itemDescription = description;
            return this;
        }

        public Item setBlockID(byte id)
        {
            blockID = id;
            return this;
        }

        public static Item getItem(byte id)
        {
            return items.Where(x => x.itemID == id).FirstOrDefault();
        }

        public int getValue()
        {
            return itemWorth;
        }

        public string getName()
        {
            return itemName;
        }

        public string getAsset()
        {
            return assetName;
        }

        public string getDescription()
        {
            return itemDescription;
        }

        public byte getItemID()
        {
            return itemID;
        }

        public abstract void onItemUsed(int x, int y);

        public byte getBlockID()
        {
            return blockID;
        }

        private string itemName;
        private int itemWorth;
        private string assetName;
        private string itemDescription;
        private byte itemID;
        private byte blockID = 0;
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
