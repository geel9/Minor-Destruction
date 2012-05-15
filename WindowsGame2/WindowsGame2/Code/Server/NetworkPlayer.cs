using System.Collections.Generic;
using System.Linq;
using MiningGame.Code.Items;
using MiningGame.Code.Structs;
using MiningGame.Code.Packets;
using Microsoft.Xna.Framework;
using MiningGame.Code.Entities;
using Lidgren.Network;
namespace MiningGame.Code.Server
{
    public class NetworkPlayer
    {
        public NetConnection NetConnection;
        public Vector2 PlayerPosition;
        public PlayerEntity PlayerEntity = null;
        public List<char> PressedKeys = new List<char>();
        public List<ItemStack> PlayerInventory = new List<ItemStack>();
        private int _jumpTimer = 0;
        public short PlayerAimAngle = 0;
        public int PlayerInventorySelected;

        public int PlayerTeam = 0;

        public NetworkPlayer(byte playerID, NetConnection connection, Vector2 playerPos, string name)
        {
            this.NetConnection = connection;
            this.PlayerEntity = new PlayerEntity(playerPos, playerID, name);
        }

        public void Update(GameTime theTime)
        {
            if (_jumpTimer > 0) _jumpTimer--;
            if (PlayerEntity == null) return;
            if (PressedKeys.Contains('a'))
            {
                PlayerEntity.EntityVelocity.X = MathHelper.Clamp(PlayerEntity.EntityVelocity.X - 3, -3, 3);
            }
            if (PressedKeys.Contains('w'))
            {
                if (!PlayerEntity.Falling && _jumpTimer <= 0)
                {
                    PlayerEntity.EntityVelocity.Y -= 10;
                    _jumpTimer = 20;
                }
            }
            if (PressedKeys.Contains('d'))
            {
                PlayerEntity.EntityVelocity.X = MathHelper.Clamp(PlayerEntity.EntityVelocity.X + 3, -3, 3);
            }
            PlayerEntity.Update(theTime, true);
            
            Packet1SCGameEvent packet = new Packet1SCGameEvent(GameServer.GameEvents.Player_Aim_And_Position, (byte)PlayerEntity.PlayerID, (short)PlayerEntity.EntityPosition.X, (short)PlayerEntity.EntityPosition.Y);
            Main.serverNetworkManager.SendPacket(packet);
        }

        public bool HasItem(byte id)
        {
            return GetPlayerItemStackFromInventory(id).itemID == id;
        }

        public ItemStack GetPlayerItemStackFromInventory(byte id)
        {
            return PlayerInventory.Where(x => x.itemID == id).FirstOrDefault();
        }

        public int GetNumItemInInventory(byte id)
        {
            return PlayerInventory.Where(x => x.itemID == id).FirstOrDefault().numberItems;
        }

        public void RemoveItems(byte itemID, int numToRemove)
        {
            if (GetNumItemInInventory(itemID) < numToRemove) return;
            ItemStack i = GetPlayerItemStackFromInventory(itemID);
            int index = PlayerInventory.IndexOf(i);
            i.numberItems -= numToRemove;
            if (i.numberItems == 0)
            {
                if (index < PlayerInventorySelected) PlayerInventorySelected++;
                PlayerInventory.RemoveAt(index);
                Packet p = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Remove, (byte)index);
                Main.serverNetworkManager.SendPacket(p, NetConnection);

            }
            else
            {
                PlayerInventory[index] = i;
                Packet pack = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Update, (byte)index, (byte)i.itemID, i.numberItems);
                Main.serverNetworkManager.SendPacket(pack, NetConnection);
            }
        }

        public Item GetPlayerItemInHand()
        {
            if (PlayerInventorySelected >= PlayerInventory.Count) PlayerInventorySelected = -1;
            if (PlayerInventorySelected == -1) return null;
            return Item.GetItem(PlayerInventory[PlayerInventorySelected].itemID);
        }

        public void PickupItem(ItemStack item)
        {
            for (int i = 0; i < PlayerInventory.Count; i++)
            {
                ItemStack it = PlayerInventory[i];
                if (it.itemID == item.itemID)
                {
                    PlayerInventory[i] = new ItemStack(it.numberItems + item.numberItems, it.itemID);
                    Packet pack = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Update, (byte)i, (byte)PlayerInventory[i].itemID, PlayerInventory[i].numberItems);
                    Main.serverNetworkManager.SendPacket(pack, NetConnection);
                    return;
                }
            }
            PlayerInventory.Add(item);
            Packet p = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Add, (byte)item.itemID, item.numberItems);
            Main.serverNetworkManager.SendPacket(p, NetConnection);
        }
    }
}
