using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using MiningGameServer.Items;
using MiningGameServer.Packets;
using MiningGameServer.Structs;

namespace MiningGameServer.PlayerClasses
{
    public class PlayerInventory
    {
        public ItemStack[] Inventory = new ItemStack[103];
        private int _bagSize = 25;
        private int _armorSize = 3;
        public int PlayerInventorySelected = 0;

        public NetworkPlayer NetworkPlayer;

        public PlayerInventory(NetworkPlayer player)
        {
            NetworkPlayer = player;
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

        public void RemoveItems(byte itemID, int numToRemove)
        {
            if (GetNumItemInInventory(itemID) == 0) return;
            ItemStack i = GetPlayerItemStackFromInventory(itemID);
            ServerItem serverItem = i.Item;
            int index = Array.IndexOf(Inventory, i);
            i.NumberItems -= numToRemove;

            if (i.NumberItems <= 0)
            {
                Packet p = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Remove, (byte)index);
                GameServer.ServerNetworkManager.SendPacket(p, NetworkPlayer.NetConnection);
                Inventory[index] = new ItemStack();
                if (i.NumberItems < 0)
                {
                    RemoveItems(itemID, -i.NumberItems);
                }
            }
            else
            {
                Inventory[index] = i;
                Packet pack = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Update, (byte)index, (byte)i.ItemID, i.NumberItems);
                GameServer.ServerNetworkManager.SendPacket(pack, NetworkPlayer.NetConnection);
            }
        }
        public void PickupItem(ItemStack item)
        {
            ServerItem serverItem = item.Item;
            ItemStack stack2 = new ItemStack();

            int start = 3;
            int end = Inventory.Length;


            if (item.NumberItems > serverItem.GetMaxStack())
            {
                item.NumberItems -= serverItem.GetMaxStack();
                stack2 = new ItemStack(item.NumberItems - serverItem.GetMaxStack(), serverItem.GetItemID());
            }

            for (int i = start; i < end; i++)
            {
                ItemStack it = Inventory[i];
                if (it.ItemID == 0)
                {
                    Inventory[i] = item;
                    Packet pack = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Update, (byte)i, (byte)Inventory[i].ItemID, Inventory[i].NumberItems);
                    GameServer.ServerNetworkManager.SendPacket(pack, NetworkPlayer.NetConnection);
                    if (stack2.ItemID != 0)
                    {
                        PickupItem(stack2);
                    }
                    return;
                }

                ServerItem sItem = it.Item;
                int maxStack = sItem.GetMaxStack();

                if (it.ItemID == item.ItemID)
                {
                    if (maxStack == it.NumberItems) continue;
                    int newTotal = it.NumberItems + item.NumberItems;
                    if(newTotal > maxStack)
                    {
                        int newCur = newTotal - maxStack;
                        int newNext = it.NumberItems - newCur;
                        it.NumberItems = newCur;
                        PickupItem(new ItemStack(newNext, item.ItemID));
                    }

                    Inventory[i] = new ItemStack(it.NumberItems + item.NumberItems, it.ItemID);
                    Packet pack = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Update, (byte)i,
                                                         (byte)Inventory[i].ItemID,
                                                         Inventory[i].NumberItems);
                    GameServer.ServerNetworkManager.SendPacket(pack, NetworkPlayer.NetConnection);
                    return;
                }
            }
        }

        public void RemoveItemAt(int slot)
        {
            Inventory[slot] = new ItemStack(0, 0);

            Packet p = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Remove, (byte)slot);
            GameServer.ServerNetworkManager.SendPacket(p, NetworkPlayer.NetConnection);

        }

        public ServerItem GetPlayerItemInHand()
        {
            return ServerItem.GetItem(Inventory[PlayerInventorySelected].ItemID);
        }

        public ItemStack GetPlayerItemStackInHand()
        {
            return Inventory[PlayerInventorySelected];
        }
    }
}
