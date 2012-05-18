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

        public byte[,] PlayerBlockIDCache;
        public byte[,] PlayerBlockMDCache;

        public bool FacingLeft
        {
            get { return PlayerEntity.FacingLeft; }
            set { PlayerEntity.FacingLeft = value; }
        }

        public Vector2 Position
        {
            get { return PlayerEntity.EntityPosition; }
            set { PlayerEntity.EntityPosition = value; }
        }

        public byte UpdateMask = 0;

        public int PlayerTeam = 0;

        public NetworkPlayer(byte playerID, NetConnection connection, Vector2 playerPos, string name)
        {
            NetConnection = connection;
            PlayerEntity = new PlayerEntity(playerPos, playerID, name);
            UpdateMask |= 1;
            UpdateMask |= (int)
            PlayerUpdateFlags.Player_Position_X;
            UpdateMask |= (int)
            PlayerUpdateFlags.Player_Position_Y;

            PlayerBlockIDCache = new byte[GameServer.WorldSizeX, GameServer.WorldSizeY];
            PlayerBlockMDCache = new byte[GameServer.WorldSizeX, GameServer.WorldSizeY];
        }

        public void UpdateCache()
        {
            var blockPos = new Vector2((int)(Position.X / GameWorld.BlockWidth), (int)(Position.Y / GameWorld.BlockHeight));

            int startX = (int)MathHelper.Clamp((int)blockPos.X - (800 / 16) - 1, 0, GameWorld.WorldSizeX);
            int startY = (int)MathHelper.Clamp((int)blockPos.Y - (500 / 16) - 1, 0, GameWorld.WorldSizeY);
            int endX = (int)MathHelper.Clamp((int)blockPos.X + (500 / 16) + 1, 0, GameWorld.WorldSizeX);
            int endY = (int)MathHelper.Clamp((int)blockPos.Y + (800 / 16) + 1, 0, GameWorld.WorldSizeY);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    byte cachedByte = PlayerBlockIDCache[x, y];
                    byte cachedByteM = PlayerBlockMDCache[x, y];

                    byte realByte = GameServer.WorldBlocks[x, y];
                    byte realByteM = GameServer.WorldBlocksMetaData[x, y];

                    if (cachedByte == realByte && realByteM == cachedByteM) continue;

                    PlayerBlockIDCache[x, y] = realByte;
                    PlayerBlockMDCache[x, y] = realByteM;

                    Packet1CSGameEvent packet = null;

                    if (cachedByte != realByte && cachedByteM != realByteM)
                    {
                        packet = new Packet1CSGameEvent(GameServer.GameEvents.Block_Set, (short)x, (short)y, realByte, realByteM);
                    }
                    else if (cachedByte == realByte && cachedByteM != realByteM)
                    {
                        packet = new Packet1CSGameEvent(GameServer.GameEvents.Block_Set_MD, (short)x, (short)y, realByteM);
                    }
                    else if(cachedByteM == realByteM && cachedByte != realByte)
                    {
                        packet = new Packet1CSGameEvent(GameServer.GameEvents.Block_Set_ID, (short)x, (short)y, realByte);
                    }
                    if (packet == null) continue;
                    Main.serverNetworkManager.SendPacket(packet, NetConnection);
                }
            }
        }

        public void Update(GameTime theTime)
        {
            UpdateCache();

            if (_jumpTimer > 0) _jumpTimer--;
            if (PlayerEntity == null) return;

            bool facingLeft = FacingLeft;
            if (PressedKeys.Contains('w'))
            {
                if (!PlayerEntity.Falling && _jumpTimer <= 0)
                {
                    PlayerEntity.EntityVelocity.Y -= 10;
                    _jumpTimer = 20;
                }
            }
            if (PressedKeys.Contains('a'))
            {
                PlayerEntity.EntityVelocity.X = MathHelper.Clamp(PlayerEntity.EntityVelocity.X - 3, -3, 3);
                FacingLeft = true;
            }

            if (PressedKeys.Contains('d'))
            {
                PlayerEntity.EntityVelocity.X = MathHelper.Clamp(PlayerEntity.EntityVelocity.X + 3, -3, 3);
                FacingLeft = false;
            }

            Vector2 oldPos = new Vector2(PlayerEntity.EntityPosition.X, PlayerEntity.EntityPosition.Y);
            PlayerEntity.Update(theTime, true);
            if (oldPos != PlayerEntity.EntityPosition)
            {
                UpdateMask |= (int)PlayerUpdateFlags.Player_Update;
                if (oldPos.X != PlayerEntity.EntityPosition.X)
                    UpdateMask |= (int)PlayerUpdateFlags.Player_Position_X;
                if (oldPos.Y != PlayerEntity.EntityPosition.Y)
                    UpdateMask |= (int)PlayerUpdateFlags.Player_Position_Y;
            }
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
