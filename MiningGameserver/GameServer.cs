using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using MiningGameServer.Blocks;
using MiningGameServer.Packets;
using MiningGameserver.Entities;
using MiningGameserver.Items;

namespace MiningGameServer
{
    public class GameServer
    {
        public static Random Random = new Random();

        public static byte[,] WorldBlocks = new byte[WorldSizeX, WorldSizeY];
        public static byte[,] WorldBlocksMetaData = new byte[WorldSizeX, WorldSizeY];

        public static List<NetworkPlayer> NetworkPlayers = new List<NetworkPlayer>();
        public static ServerProjectile[] GameProjectiles = new ServerProjectile[256];

        public const int WorldSizeX = 500, WorldSizeY = 500;

        //public static List<BlockConnection> BlockConnections = new List<BlockConnection>();
        //public static List<ItemRecipe> ItemRecipes = new List<ItemRecipe>();

        public static List<Vector3> ScheduledUpdates = new List<Vector3>();

        public static int BlockHeight = 16, BlockWidth = 16;
        public static ServerNetworkManager ServerNetworkManager;

        public GameServer(int port)
        {
            GenerateWorld();
            ServerNetworkManager = new ServerNetworkManager();
            ServerNetworkManager.Host(port);
            ServerItem.MakeItems();
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
                for (int y = 11; y < WorldSizeY; y++)
                {
                    SetBlock(x, y, 2);
                }
            }

            //Generate the top layer of grass.
            for (int x = 0; x < WorldSizeX; x++)
            {
                int rand2 = Random.Next(0, 3);
                rand2 = 0;
                for (int y = 10; y >= 10 - rand2; y--)
                {
                    SetBlock(x, y, 2);
                }
                SetBlock(x, 10 - rand2, 2);
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
            byte blockID = GetBlockIDAt(x, y);
            if (blockID == 2)
            {
                SetBlock(x, y, id);
                int dist = (int)Math.Sqrt(((x - startX) * (x - startX)) + ((y - startY) * (y - startY)));
                if (dist <= 10)
                {
                    byte blockUp = GetBlockIDAt(x, y - 1);
                    byte blockDown = GetBlockIDAt(x, y + 1);
                    byte blockLeft = GetBlockIDAt(x - 1, y);
                    byte blockRight = GetBlockIDAt(x + 1, y);

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
                    if (WorldBlocks[x, y] == 1)
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

        public static void SetBlock(int x, int y, byte blockID, bool notify = true, byte metaData = 0)
        {
            if (x < WorldSizeX && y < WorldSizeY && blockID >= 0)
            {
                if (blockID != GetBlockIDAt(x, y) && GetBlockIDAt(x, y) != 0) Block.GetBlock(GetBlockIDAt(x, y)).OnBlockRemoved(x, y);
                WorldBlocksMetaData[x, y] = metaData;
                WorldBlocks[x, y] = blockID;
                if (blockID != 0) Block.GetBlock(blockID).OnBlockPlaced(x, y, notify);

                //Packet1SCGameEvent pack = new Packet1SCGameEvent((byte)GameEvents.Block_Set, x, y, blockID, metaData);
                // Main.serverNetworkManager.SendPacket(pack);
            }
        }

        public static void SetBlockMetaData(int x, int y, byte metadata)
        {
            WorldBlocksMetaData[x, y] = metadata;
            //Packet1SCGameEvent pack = new Packet1SCGameEvent((byte)GameEvents.Block_Set, x, y, WorldBlocks[x, y], metadata);
            //Main.serverNetworkManager.SendPacket(pack);
        }

        /*public static void LoadRecipes()
        {
            ItemRecipes.Clear();
            string path = DirectoryManager.RECIPES;
            string[] files = Directory.GetFiles(path).Where(f => f.Contains(".rcp")).Select(x => x.Replace(path, "")).ToArray<string>();

            foreach (string file in files)
            {
                string contents = FileReaderManager.ReadFileContents(path + file);
                List<ItemRecipe> l = loadRecipe(contents);
                ItemRecipes.AddRange(l.Except(ItemRecipes));
            }
        }*/

        /*public static List<ItemRecipe> loadRecipe(string contents)
        {
            ItemRecipe curRecipe = new ItemRecipe();
            List<ItemRecipe> ret = new List<ItemRecipe>();
            ItemStack currentStack = new ItemStack();
            bool inString = false;
            bool doneReturn = false;
            bool inComment = false;
            string currentToken = "";

            string decimals = "0123456789";

            foreach (char c in contents)
            {
                if (c == '#')
                {
                    inComment = true;
                }
                if (c == '\n')
                {
                    inComment = false; continue;
                }
                if (inComment) continue;
                if (c == '"')
                {
                    inString = !inString;
                    if (!inString)
                    {
                        curRecipe.recipeName = currentToken;
                        currentToken = "";
                    }
                    continue;
                }

                if (inString)
                {
                    currentToken += c;
                }
                else
                {
                    if (c == '{')
                    {
                        curRecipe = new ItemRecipe();
                    }

                    else if (c == '}')
                    {
                        ret.Add(curRecipe);
                        currentStack = new ItemStack();
                        currentToken = "";
                        curRecipe = new ItemRecipe();
                        doneReturn = false;
                    }

                    else if (decimals.Contains(c))
                    {
                        currentToken += c;
                    }

                    else if (c == '*')
                    {
                        int id = Convert.ToInt32(currentToken.Replace(" ", ""));
                        currentStack.itemID = (byte)id;
                        currentToken = "";
                    }

                    else if (c == ';')
                    {
                        if (currentStack.itemID != 0)
                        {
                            int num = Convert.ToInt32(currentToken.Replace(" ", ""));
                            currentStack.numberItems = num;

                            if (doneReturn)
                            {
                                curRecipe.requiredItems.Add(currentStack);
                            }
                            else
                            {
                                curRecipe.returnItems = currentStack;
                                doneReturn = true;
                            }

                            currentStack = new ItemStack();
                            currentToken = "";
                        }
                    }
                }
            }

            return ret;
        }*/

        public static byte GetBlockIDAt(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? WorldBlocks[x, y] : (byte)255;
        }

        public static byte GetBlockIDAt(float x, float y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? WorldBlocks[(int)x, (int)y] : (byte)0;
        }

        public static Block GetBlockAt(int x, int y)
        {
            return Block.GetBlock(GetBlockIDAt(x, y));
        }

        public static Block GetBlockAt(float x, float y)
        {
            return Block.GetBlock(GetBlockIDAt(x, y));
        }

        public static byte GetBlockMDAt(int x, int y)
        {
            if (x < 0 || x >= WorldSizeX || y < 0 || y >= WorldSizeY) return 0;
            byte ret = WorldBlocksMetaData[x, y];
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? ret : (byte)0;
        }

        public static byte GetBlockMDAt(float x, float y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? WorldBlocksMetaData[(int)x, (int)y] : (byte)0;
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
                int nextUpdate = GetBlockAt(update.X, update.Y).OnBlockUpdate((int)update.X, (int)update.Y);
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
                packet.writeByte((byte)playersToUpdate.Count);

                foreach (NetworkPlayer p in playersToUpdate)
                {
                    byte realUpdateMask = p.UpdateMask;

                    packet.writeByte(p.PlayerID);
                    packet.writeByte(realUpdateMask);
                    if ((p.UpdateMask & (int)PlayerUpdateFlags.Player_Position_X) != 0)
                    {
                        packet.writeShort((short)p.EntityPosition.X);
                    }

                    if ((p.UpdateMask & (int)PlayerUpdateFlags.Player_Position_Y) != 0)
                    {
                        packet.writeShort((short)p.EntityPosition.Y);
                    }

                    if ((p.UpdateMask & (int)PlayerUpdateFlags.Player_Movement_Flags) != 0)
                    {
                        packet.writeByte(p.MovementFlags);
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
                    char characterPressing = p.readChar();
                    bool isPressing = p.readBool();
                    break;

                case GameEvents.Player_Inventory_Selection_Change:
                    player.PlayerInventorySelected = p.readByte();
                    break;

                case GameEvents.Player_Chat:
                    bool teamChat = p.readBool();
                    string chatText = p.readString();
                    Packet1SCGameEvent pack = new Packet1SCGameEvent(GameEvents.Player_Chat, (byte)player.PlayerID, (bool)teamChat, chatText);
                    ServerNetworkManager.SendPacket(pack);
                    break;

                    case GameEvents.Player_Change_Name:
                    string newName = p.readString();
                    if(newName.Length > 0)
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
