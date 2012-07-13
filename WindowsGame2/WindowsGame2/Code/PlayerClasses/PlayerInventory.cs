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

        public PlayerInventory(PlayerController player)
        {
            PlayerController = player;
            SetBagSize(25);
        }

        public void SetBagSize(int size)
        {
            Inventory = new ItemStack[_armorSize + size];
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
