using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Items;
using MiningGameServer.Packets;

namespace MiningGame.Code.PlayerClasses
{
    public class PlayerInventory
    {
        public ItemStack[] Inventory = new ItemStack[103];
        private int _bagSize = 25;
        private int _armorSize = 3;
        public int PlayerInventorySelected = 0;
        public PlayerController PlayerController;

        private int EndOfRaw
        {
            get { return _armorSize + _bagSize; }
        }

        private int FreeRawSpace
        {
            get
            {
                for (int i = _armorSize; i <= EndOfRaw; i++)
                {
                    ItemStack cur = Inventory[i];
                    if (cur.ItemID == 0)
                    {
                        return EndOfRaw - i;
                    }
                }
                return 0;
            }
        }

        private int FreeRefinedSpace
        {
            get
            {
                for (int i = EndOfRaw + 1; i <= EndOfRaw + _bagSize + 1; i++)
                {
                    ItemStack cur = Inventory[i];
                    if (cur.ItemID == 0)
                    {
                        return (EndOfRaw + _bagSize + 1) - i;
                    }
                }
                return 0;
            }
        }

        public PlayerInventory(PlayerController player)
        {
            PlayerController = player;
            SetBagSize(25);
        }

        public void SetBagSize(int size)
        {
            Inventory = new ItemStack[_armorSize + (2 * size)];
            for (int i = 0; i < Inventory.Length; i++)
            {
                Inventory[i] = new ItemStack();
            }
            _bagSize = size;
        }

        public bool HasItem(byte id)
        {
            return GetPlayerItemStackFromInventory(id).ItemID == id;
        }

        public ItemStack GetPlayerItemStackFromInventory(byte id)
        {
            foreach (ItemStack i in Inventory)
            {
                if (i.ItemID == id) return i;
            }
            return new ItemStack();
        }

        public int GetNumItemInInventory(byte id)
        {
            return GetPlayerItemStackFromInventory(id).NumberItems;
        }

        public void SetItem(int slot, ItemStack item)
        {
            Inventory[slot] = item;
        }

        public void RemoveItemAt(int slot)
        {
            Inventory[slot] = new ItemStack();

        }

        public Item GetPlayerItemInHand()
        {
            return Item.GetItem(Inventory[PlayerInventorySelected].ItemID);
        }

        public ItemStack GetPlayerItemStackInHand()
        {
            return Inventory[PlayerInventorySelected];
        }
    }
}
