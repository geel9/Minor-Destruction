using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using MiningGameServer.Blocks;
using MiningGameServer.Packets;
using MiningGameServer;
using MiningGameServer.Entities;
using MiningGameServer.Items;
using MiningGameServer.Packets;
using MiningGameServer.Structs;

namespace MiningGameServer
{
    public class GameServer
    {
        public static Random Random = new Random();

        public static BlockData[,] WorldBlocks = new BlockData[WorldSizeX, WorldSizeY];

        public static List<NetworkPlayer> NetworkPlayers = new List<NetworkPlayer>();
        public static ServerProjectile[] GameProjectiles = new ServerProjectile[256];

        public static List<ServerEntityDroppedItem> DroppedItems = new List<ServerEntityDroppedItem>();

        public const int WorldSizeX = 500, WorldSizeY = 500;

        //public static List<BlockConnection> BlockConnections = new List<BlockConnection>();
        //public static List<ItemRecipe> ItemRecipes = new List<ItemRecipe>();

        public static List<Vector3> ScheduledUpdates = new List<Vector3>();

        public static int BlockHeight = 16, BlockWidth = 16;
        public static ServerNetworkManager ServerNetworkManager;

        public GameServer(int port)
        {
            ServerNetworkManager = new ServerNetworkManager();
            if (ServerNetworkManager.Host(port))
            {
                for (int x = 0; x < WorldSizeX; x++)
                {
                    for (int y = 0; y < WorldSizeY; y++)
                        WorldBlocks[x, y] = new BlockData();
                }
                GenerateWorld();
                ServerItem.MakeItems();
                Block.GenerateBlocks();
                ServerCommands.Initialize();
                ServerConsole.Log("Hosted successfully on port " + port);
            }
            else
            {
                ServerNetworkManager = null;
                throw new Exception("Could not host on port " + port);
            }
        }

        public void GenerateTree(int x, int y)
        {

        }

        public void GenerateWorld()
        {
            int rand = Random.Next(-10000, 10000);

            //Place the base level of dirt
            for (int x = 0; x < WorldSizeX; x++)
            {
                for (int y = 10; y < WorldSizeY; y++)
                {
                    SetBlock(x, y, 2);
                }
            }

            //Generate caves
            GenerateCaves();

            //Generate trees


            GenerateOres();
        }

        public static void GenerateOres()
        {
            for (int x = 0; x < WorldSizeX; x++)
            {
                for (int y = 15; y < WorldSizeY; y++)
                {
                    int randomDoOre = Random.Next(0, 50);

                    if (randomDoOre == 0)
                    {
                        GenerateOreVein(x, y, 5, x, y);
                    }
                }
            }
        }

        public static void GenerateOreVein(int x, int y, byte id, int startX, int startY)
        {
            short blockID = GetBlockAt(x, y).ID;
            if (blockID == 2)
            {
                SetBlock(x, y, id);
                int dist = (int)Math.Sqrt(((x - startX) * (x - startX)) + ((y - startY) * (y - startY)));
                if (dist <= 10)
                {
                    short blockUp = GetBlockAt(x, y - 1).ID;
                    short blockDown = GetBlockAt(x, y + 1).ID;
                    short blockLeft = GetBlockAt(x - 1, y).ID;
                    short blockRight = GetBlockAt(x + 1, y).ID;

                    if ((blockUp == 0 || blockUp == 2) && Random.Next(0, 5) == 0)
                        GenerateOreVein(x, y - 1, id, startX, startY);
                    if ((blockDown == 0 || blockDown == 2) && Random.Next(0, 5) == 0)
                        GenerateOreVein(x, y + 1, id, startX, startY);
                    if ((blockLeft == 0 || blockLeft == 2) && Random.Next(0, 5) == 0)
                        GenerateOreVein(x - 1, y, id, startX, startY);
                    if ((blockRight == 0 || blockRight == 2) && Random.Next(0, 5) == 0)
                        GenerateOreVein(x + 1, y, id, startX, startY);
                }
                else
                {
                    dist = 1;
                }
            }
        }

        public static void GenerateTrees()
        {
            for (int x = 0; x < WorldSizeX; x++)
            {
                int highestPoint = 0;
                for (int y = 0; y < WorldSizeY; y++)
                {
                    if (WorldBlocks[x, y].ID == 1)
                    {
                        highestPoint = y;
                        break;
                    }
                }
                int minTreeHeight = Random.Next(0, 3);
                int maxTreeHeight = Random.Next(minTreeHeight, highestPoint - 2);
            }
        }

        public static void GenerateCaves()
        {
            for (int x = 0; x < WorldSizeX; x++)
            {
                for (int y = 15; y < WorldSizeY; y++)
                {
                    int randCaves = Random.Next(0, 500);
                    int randCavesWidth = Random.Next(2, 5);
                    int randCavesHeight = Random.Next(2, 5);

                    if (randCaves == 0)
                    {
                        int caveStartX = (int)MathHelper.Clamp(x - randCavesWidth, 0, x);
                        int caveEndX = (int)MathHelper.Clamp(x + randCavesWidth, x, WorldSizeX);
                        int caveStartY = (int)MathHelper.Clamp(y - randCavesHeight, 0, y);
                        int caveEndY = (int)MathHelper.Clamp(y + randCavesHeight, y, WorldSizeY);
                        for (int x2 = caveStartX; x2 < caveEndX; x2++)
                        {
                            for (int y2 = caveStartY; y2 < caveEndY; y2++)
                            {
                                SetBlock(x2, y2, 0);
                            }
                        }
                    }
                }
            }
        }

        public static void SetBlock(int x, int y, short blockID, bool notify = true, byte metaData = 0)
        {
            if (x < WorldSizeX && y < WorldSizeY && blockID >= 0)
            {
                Block block = GetBlockAt(x, y).Block;
                WorldBlocks[x, y].ID = blockID;
                if (blockID != block.GetBlockID())
                    block.OnBlockRemoved(x, y);
                WorldBlocks[x, y].MetaData = metaData;

                if (blockID != 0)
                    Block.GetBlock(blockID).OnBlockPlaced(x, y, notify);

                //Packet1SCGameEvent pack = new Packet1SCGameEvent((byte)GameEvents.Block_Set, x, y, blockID, metaData);
                // Main.serverNetworkManager.SendPacket(pack);
            }
        }

        public static void SetBlock(NetworkPlayer placer, int x, int y, short blockID, bool notify = true, byte metaData = 0)
        {
            if (x < WorldSizeX && y < WorldSizeY && blockID >= 0)
            {
                Block block = GetBlockAt(x, y).Block;
                WorldBlocks[x, y].ID = blockID;
                if (blockID != block.GetBlockID())
                    block.OnBlockRemoved(x, y);
                WorldBlocks[x, y].MetaData = metaData;

                if (blockID != 0)
                    Block.GetBlock(blockID).OnBlockPlaced(x, y, notify, placer);

                //Packet1SCGameEvent pack = new Packet1SCGameEvent((byte)GameEvents.Block_Set, x, y, blockID, metaData);
                // Main.serverNetworkManager.SendPacket(pack);
            }
        }

        public static void SetBlockMetaData(int x, int y, byte metadata)
        {
            WorldBlocks[x, y].MetaData = metadata;
        }

        internal static BlockData GetBlockAt(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? WorldBlocks[x, y] : new BlockData();
        }

        internal static BlockData GetBlockAt(float x, float y)
        {
            return GetBlockAt((int)x, (int)y);
        }

        public static bool CanWalkThrough(byte id)
        {
            return Block.GetBlock(id).GetBlockWalkThrough();
        }

        public static void ScheduleUpdate(int x, int y, int frames = 5)
        {
            if (!UpdateScheduled(x, y))
            {
                ScheduledUpdates.Add(new Vector3(x, y, frames));
            }
        }

        public static void UnscheduleUpdate(int x, int y)
        {
            Vector3 remove = new Vector3(-1, -1, -1);
            Vector3[] toUnschedule = new Vector3[ScheduledUpdates.Count];
            ScheduledUpdates.CopyTo(toUnschedule);
            foreach (Vector3 v in toUnschedule)
            {
                if (v.X == x && v.Y == y)
                {
                    remove = v;
                    break;
                }
            }
            ScheduledUpdates.Remove(remove);
        }

        public static bool UpdateScheduled(int x, int y)
        {
            foreach (Vector3 v in ScheduledUpdates)
            {
                if (v.X == x && v.Y == y) return true;
            }
            return false;
        }

        public void Update(GameTime time)
        {
            ServerNetworkManager.Update();

            List<Vector3> toUnschedule = new List<Vector3>();
            Vector3[] scheduled = new Vector3[ScheduledUpdates.Count];
            for (int i = 0; i < ScheduledUpdates.Count; i++)
            {
                Vector3 update = ScheduledUpdates[i];
                if (update.Z-- > 0)
                {
                    ScheduledUpdates[i] = update;
                    continue;
                }
                int nextUpdate = GetBlockAt(update.X, update.Y).Block.OnBlockUpdate((int)update.X, (int)update.Y);
                if (nextUpdate == -1)
                {
                    toUnschedule.Add(update);
                }
                else if (nextUpdate > 0)
                {
                    update.Z = nextUpdate;
                    ScheduledUpdates[i] = update;
                }
            }
            foreach (Vector3 r in toUnschedule)
            {
                UnscheduleUpdate((int)r.X, (int)r.Y);
            }
            foreach (NetworkPlayer t in NetworkPlayers)
            {
                t.Update(time);
            }

            List<ServerEntityDroppedItem> droppedItemsToRemove = new List<ServerEntityDroppedItem>();

            foreach(ServerEntityDroppedItem item in DroppedItems)
            {
                item.Update();
                if(item.ShouldRemove)
                    droppedItemsToRemove.Add(item);
            }
            foreach (ServerEntityDroppedItem item in droppedItemsToRemove)
                DroppedItems.Remove(item);

            for (int i = 0; i < GameProjectiles.Length; i++)
            {
                ServerProjectile p = GameProjectiles[i];
                if (p == null) continue;
                p.Update();
                if (p.ShouldDestroy)
                {
                    Packet3SCRemoveProjectile remove = new Packet3SCRemoveProjectile(p.ProjectileID);
                    ServerNetworkManager.SendPacket(remove);
                    GameProjectiles[i] = null;
                }
            }

            foreach (NetworkPlayer t in NetworkPlayers)
            {
                var playersToUpdate = new List<NetworkPlayer>();
                foreach (NetworkPlayer t2 in NetworkPlayers)
                {
                    int hDist = (int)Math.Abs(t.EntityPosition.X - t2.EntityPosition.X);
                    int vDist = (int)Math.Abs(t.EntityPosition.Y - t2.EntityPosition.Y);
                    if (hDist >= 830 || vDist >= 530) continue;
                    if ((t2.UpdateMask & (int)PlayerUpdateFlags.Player_Update) == 0) continue;
                    playersToUpdate.Add(t2);
                }
                if (playersToUpdate.Count == 0) continue;

                Packet200SCPlayerUpdate packet = new Packet200SCPlayerUpdate();
                packet.WriteByte((byte)playersToUpdate.Count);

                foreach (NetworkPlayer p in playersToUpdate)
                {
                    byte realUpdateMask = p.UpdateMask;

                    packet.WriteByte(p.PlayerID);
                    packet.WriteByte(realUpdateMask);
                    if ((p.UpdateMask & (int)PlayerUpdateFlags.Player_Position_X) != 0)
                    {
                        packet.WriteShort((short)p.EntityPosition.X);
                    }
                    if ((p.UpdateMask & (int)PlayerUpdateFlags.Player_Position_Y) != 0)
                    {
                        packet.WriteShort((short)p.EntityPosition.Y);
                    }

                    if ((p.UpdateMask & (int)PlayerUpdateFlags.Player_Movement_Flags) != 0)
                    {
                        packet.WriteByte(p.MovementFlags);
                    }
                }
                ServerNetworkManager.SendPacket(packet, t.NetConnection);
            }

            foreach (NetworkPlayer t in NetworkPlayers)
                t.UpdateMask = 0;
        }

        public static void HandleGameEvent(byte eventID, Packet p, NetworkPlayer player)
        {
            switch ((GameEvents)eventID)
            {
                case GameEvents.Player_KeyPress:
                    char characterPressing = p.ReadChar();
                    bool isPressing = p.ReadBool();
                    break;

                case GameEvents.Player_Use_Item:
                    short x = p.ReadShort();
                    short y = p.ReadShort();
                    ServerItem itemInHand = player.GetPlayerItemInHand();
                    if (itemInHand == null) break;
                    itemInHand.OnItemUsed(x, y, player);
                    break;

                case GameEvents.Player_Use_Block:
                    x = p.ReadShort();
                    y = p.ReadShort();
                    Block b = GetBlockAt(x, y).Block;
                    b.OnBlockUsed(x, y);
                    break;

                case GameEvents.Player_Inventory_Selection_Change:
                    player.PlayerInventorySelected = p.ReadByte();
                    player.SendEquippedItemUpdate();
                    break;

                case GameEvents.Player_Chat:
                    bool teamChat = p.ReadBool();
                    string chatText = p.ReadString();

                    ServerConsole.Log(player.PlayerName + ": " + chatText);

                    Packet1SCGameEvent pack = new Packet1SCGameEvent(GameEvents.Player_Chat, (byte)player.PlayerID, (bool)teamChat, chatText);
                    ServerNetworkManager.SendPacket(pack);
                    break;

                case GameEvents.Player_Change_Name:
                    string newName = p.ReadString();
                    if (newName.Length > 0)
                    {
                        player.PlayerName = newName;
                        pack = new Packet1SCGameEvent(GameEvents.Player_Change_Name, (byte)player.PlayerID, newName);
                        ServerNetworkManager.SendPacket(pack);
                    }
                    break;
            }
        }

        public static int GetFreeProjectileSlot()
        {
            for (int i = 0; i < GameProjectiles.Length; i++)
            {
                if (GameProjectiles[i] == null) return i;
            }
            return -1;
        }

        public static void SendMessageToAll(string message)
        {
            Packet1CSGameEvent pack = new Packet1CSGameEvent(GameEvents.Player_Chat, (byte)0, false, message);
            ServerNetworkManager.SendPacket(pack);
        }

        public static short GetFreeDroppedItemIndex()
        {
            List<short> possibleIDs = new List<short>();
            List<short> takenIDs = new List<short>();
            for (short i = 0; i < short.MaxValue; i++)
                possibleIDs.Add(i);

            for (short i = 0; i < DroppedItems.Count; i++)
            {
                ServerEntityDroppedItem item = DroppedItems[i];
                if (item == null)
                    continue;
                takenIDs.Add(item.DroppedItemID);
            }
            short[] retIDs = possibleIDs.Except(takenIDs).ToArray();
            if (retIDs.Length == 0)
                return -1;
            return retIDs[0];
        }

        public static void DropItem(ItemStack stack, Vector2 position)
        {
            short index = GetFreeDroppedItemIndex();
            if (index == -1)
                return;

            Vector2 velocity = new Vector2(Random.Next(-5, 6), -3);

            var item = new ServerEntityDroppedItem((int)position.X, (int)position.Y, velocity, stack, index);
            DroppedItems.Add(item);

            Packet8SCItemDropped pack = new Packet8SCItemDropped(position, velocity, index, stack.ItemID);
            ServerNetworkManager.SendPacket(pack);
        }

        public static Vector2 AbsoluteToTile(Vector2 tile)
        {
            return new Vector2((int)(tile.X / BlockWidth), (int)(tile.Y / BlockHeight));
        }

        public enum GameEvents : byte
        {
            Player_Join,
            Player_Leave,
            Block_Set,
            Block_Set_ID,
            Block_Set_MD,
            Block_Set_Chunk,
            Player_Chat,
            Player_KeyPress,
            Player_Position,
            Player_Inventory_Selection_Change,
            Player_Inventory_Update,
            Player_Inventory_Add,
            Player_Inventory_Remove,
            Player_Use_Item,
            Player_Use_Block,
            Player_Aim_And_Position,
            Player_Direction,
            Player_Animation,
            Player_Change_Name
        }
    }
}
