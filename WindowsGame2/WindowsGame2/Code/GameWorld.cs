using System;
using System.Collections.Generic;
using System.Linq;
using GeeUI.Managers;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Items;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Entities;
using MiningGame.Code.Interfaces;
using MiningGameServer;
using MiningGameServer.Packets;

namespace MiningGame.Code
{
    public class GameWorld : UpdatableAndDrawable
    {
        public const int WorldSizeX = GameServer.WorldSizeX, WorldSizeY = GameServer.WorldSizeY;
        public const int BlockWidth = 16, BlockHeight = 16, PlayerVision = 4;
        public static Random Random = new Random();

        public static List<PlayerEntity> OtherPlayers = new List<PlayerEntity>();
        public static PlayerController ThePlayer = new PlayerController();

        public static List<EntityProjectile> GameProjectiles = new List<EntityProjectile>();

        public static BlockData[,] WorldBlocks = new BlockData[WorldSizeX, WorldSizeY];

        public static void LoadBlocks()
        {
            Block.AllBlocks.Clear();
            Block.GenerateBlocks();
        }

        public GameWorld()
        {
            addToList();
            ThePlayer.Start();
        }

        public static List<Vector2> LineIntersections(Vector2 p1, Vector2 p2)
        {
            List<Vector2> ret = new List<Vector2>();
            bool steep = Math.Abs(p2.Y - p1.Y) > Math.Abs(p2.X - p1.X);
            if (steep)
            {
                Vector2 swap1 = new Vector2(p1.X, p1.Y);
                Vector2 swap2 = new Vector2(p2.X, p2.Y);
                p1.X = swap1.Y;
                p1.Y = swap1.X;
                p2.X = swap2.Y;
                p2.Y = swap2.X;
            }
            if (p1.X > p2.X)
            {
                Vector2 swap1 = new Vector2(p1.X, p1.Y);
                Vector2 swap2 = new Vector2(p2.X, p2.Y);
                p1.X = swap2.X;
                p2.X = swap1.X;
                p1.Y = swap2.Y;
                p2.Y = swap1.Y;
            }
            int deltax = (int)(p2.X - p1.X);
            int deltay = (int)Math.Abs(p2.Y - p1.Y);
            int error = deltax / 2;
            int ystep;
            int y = (int)p1.Y;
            if (p1.Y < p2.Y) ystep = 1; else ystep = -1;
            for (int x = (int)p1.X; x < p2.X; x++)
            {
                ret.Add(steep ? new Vector2(y, x) : new Vector2(x, y));
                error -= deltay;
                if (error < 0)
                {
                    y += ystep;
                    error += deltax;
                }
            }
            return ret;
        }

        public static bool IsBlockLit(int x, int y)
        {
            return true;
        }

        public static int GetBlockLitLevel(int x, int y)
        {
            return 2;
        }

        public static bool IsBlockFullyLit(int x, int y)
        {
            return true;
            Vector2 playerTile = AbsoluteToTile(ThePlayer.PlayerEntity.EntityPosition);
            float xDist = Math.Abs(playerTile.X - x);
            float yDist = Math.Abs(playerTile.Y - y);
            return true || (xDist < PlayerVision && yDist < PlayerVision);
        }

        public static int ShouldRenderBlock(int x, int y)
        {
            return 2;
            //return getBlockLitLevel(x, y);
        }

        public static BlockData GetBlockAt(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? WorldBlocks[x, y] : new BlockData();
        }

        public static BlockData GetBlockAt(float x, float y)
        {
            return GetBlockAt((int) x, (int) y);
        }

        public void Update(GameTime time)
        {
            //ConsoleManager.setVariableValue("window_title", "Blocking: " + InterfaceManager.blocking);
            ThePlayer.Update(time);
            if (ThePlayer.PlayerEntity != null)
                CameraManager.setCameraPositionCenterMin(ThePlayer.PlayerEntity.EntityPosition, Vector2.Zero);
            foreach (EntityProjectile p in GameProjectiles)
                p.Update(time);
        }

        public void Draw(SpriteBatch sb)
        {
            if (ThePlayer.PlayerEntity == null) return;
            Vector2 playerTile = AbsoluteToTile(ThePlayer.PlayerEntity.EntityPosition);
            Rectangle cameraBounds = CameraManager.cameraBoundBox;
            int blockStartX = (int)MathHelper.Clamp(cameraBounds.Left / BlockWidth, 0, WorldSizeX - 1);
            int blockStartY = (int)MathHelper.Clamp(cameraBounds.Top / BlockHeight, 0, WorldSizeY - 1);
            int blockEndX = (int)MathHelper.Clamp((cameraBounds.Right / BlockWidth) + 1, 0, WorldSizeX);
            int blockEndY = (int)MathHelper.Clamp((cameraBounds.Bottom / BlockHeight) + 1, 0, WorldSizeY);

            Vector2 mousePos = InputManager.GetMousePosV();
            Vector2 blockMouseOver = AbsoluteToTile(mousePos);
            blockMouseOver.X = MathHelper.Clamp(blockMouseOver.X, 0, WorldSizeX - 1);
            blockMouseOver.Y = MathHelper.Clamp(blockMouseOver.Y, 0, WorldSizeY - 1);
            BlockData mouseOverID = WorldBlocks[(int)blockMouseOver.X, (int)blockMouseOver.Y];

            Vector2 curDig = ThePlayer.CurMining;
            float digMult = MathHelper.Clamp(1f - (float)((float)ThePlayer.DigPct / 100f), 0, 1);
            for (int x = blockStartX; x < blockEndX; x++)
            {
                for (int y = blockStartY; y < blockEndY; y++)
                {
                    Color backColor = Color.SandyBrown;
                    Texture2D backTexture = AssetManager.GetTexture("background_1");
                    if (y < 10)
                    {
                        backColor = Main.BackColor;
                    }
                    Vector2 drawPos = new Vector2(x * BlockWidth + (BlockWidth / 2), y * BlockHeight + (BlockHeight / 2));
                    drawPos -= CameraManager.cameraPosition;
                    BlockData blockID = WorldBlocks[x, y];
                    int render = ShouldRenderBlock(x, y);
                    if (render > 0)
                    {
                        if (y < 10)
                            DrawManager.DrawBox(drawPos, BlockWidth, BlockHeight, backColor, sb);
                        else
                        {
                            sb.Draw(backTexture, drawPos - new Vector2(BlockWidth / 2, BlockHeight / 2), Color.White);
                        }

                        if (blockID.ID != 0)
                        {
                            Block block = blockID.Block;
                            bool flag = (x == curDig.X && y == curDig.Y);
                            bool flagHide = block.GetBlockHide() && render == 1;

                            if (block.GetBlockRenderSpecial() || y < 10)
                            {
                                BlockRenderer renderer = block.RenderBlock(x, y, sb);
                                if (renderer != null)
                                    sb.Draw(renderer.Texture, drawPos, null, Color.White, renderer.Rotation, new Vector2(BlockWidth / 2, BlockHeight / 2), (flag ? digMult : 1), renderer.Effects, 0);
                            }
                            else
                            {
                                Color color = block.GetBlockRenderColor();
                                //DrawManager.Draw_Box(drawPos, blockWidth * (flag ? digMult : 1), blockHeight * (flag ? digMult : 1), color, sb, 0f);
                            }
                        }
                    }
                    else
                    {
                        //if (blockID != 0)
                        DrawManager.DrawBox(drawPos, BlockWidth, BlockHeight, Color.Black, sb, 0f);
                    }
                }
            }
            string inventory = "\nInventory: \n";

            Item inI = ThePlayer.GetPlayerItemInHand();
            if (inI != null)
            {
                inventory = "\n\nItem in hand: " + inI.GetName() + "\n" + inventory;
            }
            else
            {
                inventory = "\n\nItem in hand: nothing\n" + inventory;
            }
            foreach (ItemStack i in ThePlayer.PlayerInventory)
            {
                Item item = Item.GetItem(i.itemID);
                inventory += item.GetName() + ": " + i.numberItems + "\n";
            }
            if (ThePlayer.PlayerInventory.Count == 0)
                inventory += "Nothing!\n";

            ThePlayer.Draw(sb);
            foreach (PlayerEntity oth in OtherPlayers)
            {
                oth.Draw(sb);
            }

            foreach (EntityProjectile projectile in GameProjectiles)
            {
                projectile.Draw(sb);
            }
        }

        public static Vector2 AbsoluteToTile(Vector2 tile)
        {
            return new Vector2((int)(tile.X / BlockWidth), (int)(tile.Y / BlockHeight));
        }

        public static void SetBlock(int x, int y, short blockID, bool notify = true, byte metaData = 0)
        {
            if (x < WorldSizeX && y < WorldSizeY && blockID >= 0)
            {
                if (blockID != GetBlockAt(x, y).ID && GetBlockAt(x, y).ID != 0) GetBlockAt(x, y).Block.OnBlockRemoved(x, y);
                WorldBlocks[x, y].MetaData = metaData;
                WorldBlocks[x, y].ID = blockID;
                if (blockID != 0) Block.GetBlock(blockID).OnBlockPlaced(x, y, notify);
            }
        }

        public static void SetBlockMetaData(int x, int y, byte metadata)
        {
            WorldBlocks[x, y].MetaData = metadata;
        }

        public static void HandleGameEvent(byte eventID, Packet p)
        {
            switch ((GameServer.GameEvents)eventID)
            {
                case GameServer.GameEvents.Block_Set:
                    SetBlock(p.readShort(), p.readShort(), p.readShort(), false, p.readByte());
                    break;

                case GameServer.GameEvents.Block_Set_ID:
                    SetBlock(p.readShort(), p.readShort(), p.readShort(), false, 0);
                    break;

                case GameServer.GameEvents.Block_Set_MD:
                    short X = p.readShort();
                    short Y = p.readShort();
                    SetBlock(X, Y, WorldBlocks[X, Y].ID, false, p.readByte());
                    break;

                case GameServer.GameEvents.Block_Set_Chunk:
                    int numSending = p.readShort();
                    short startX = p.readShort();
                    short startY = p.readShort();

                    for (int i = 0; i < numSending; i++)
                    {
                        X = (short)(p.readByte() + startX);
                        Y = (short)(p.readByte() + startY);
                        short Id = p.readShort();
                        byte metadata = p.readByte();
                        SetBlock(X, Y, Id, true, metadata);
                    }

                    break;

                case GameServer.GameEvents.Player_Chat:
                    byte playerID = p.readByte();
                    bool teamChat = p.readBool();
                    string chatText = p.readString();
                    if (playerID == ThePlayer.PlayerEntity.PlayerID)
                    {
                        ChatInterface.AddChat(new ChatEntry(ThePlayer.PlayerEntity.PlayerName, chatText, Color.White, teamChat));
                    }
                    else if (playerID != 0)
                    {
                        foreach (PlayerEntity pe in OtherPlayers)
                        {
                            if (pe.PlayerID == playerID)
                            {
                                ChatInterface.AddChat(new ChatEntry(pe.PlayerName, chatText, Color.White, teamChat));
                            }
                        }
                    }
                    else
                    {
                        ChatInterface.AddChat(new ChatEntry("Server", chatText, Color.Blue, false));
                    }
                    break;

                case GameServer.GameEvents.Player_Position:
                    playerID = p.readByte();
                    int x = p.readInt();
                    int y = p.readInt();
                    if (playerID == ThePlayer.PlayerEntity.PlayerID)
                    {
                        ThePlayer.PlayerEntity.EntityPosition = new Vector2(x, y);
                    }
                    else
                    {
                        foreach (PlayerEntity pe in OtherPlayers)
                        {
                            if (pe.PlayerID == playerID)
                            {
                                pe.EntityPosition = new Vector2(x, y);
                            }
                        }
                    }
                    break;

                case GameServer.GameEvents.Player_Aim_And_Position:
                    playerID = p.readByte();
                    x = p.readShort();
                    y = p.readShort();
                    if (playerID == ThePlayer.PlayerEntity.PlayerID)
                    {
                        ThePlayer.PlayerEntity.EntityPosition = new Vector2(x, y);
                    }
                    else
                    {
                        foreach (PlayerEntity pe in OtherPlayers)
                        {
                            if (pe.PlayerID == playerID)
                            {
                                pe.EntityPosition = new Vector2(x, y);
                            }
                        }
                    }
                    break;

                case GameServer.GameEvents.Player_Leave:
                    byte pId = p.readByte();
                    OtherPlayers.Remove(OtherPlayers.Where(pl => pl.PlayerID == pId).FirstOrDefault());
                    ConsoleManager.Log("Player " + pId + " left.");
                    break;

                case GameServer.GameEvents.Player_Inventory_Update:
                    byte index = p.readByte();
                    byte id = p.readByte();
                    int num = p.readInt();
                    if (index < ThePlayer.PlayerInventory.Count)
                    {
                        ThePlayer.PlayerInventory[index] = new ItemStack(num, id);
                    }
                    break;

                case GameServer.GameEvents.Player_Inventory_Add:
                    byte itemID = p.readByte();
                    int itemNum = p.readInt();
                    ThePlayer.PlayerInventory.Add(new ItemStack(itemNum, itemID));
                    break;

                case GameServer.GameEvents.Player_Inventory_Remove:
                    ThePlayer.PlayerInventory.RemoveAt(p.readByte());
                    break;

                    case GameServer.GameEvents.Player_Change_Name:
                    playerID = p.readByte();
                    string newName = p.readString();

                    if (playerID == ThePlayer.PlayerEntity.PlayerID)
                    {
                        ChatInterface.AddChat(new ChatEntry("Server", ThePlayer.PlayerEntity.PlayerName + " has changed name to " + newName, Color.Blue, false));
                        ThePlayer.PlayerEntity.PlayerName = newName;
                    }
                    else
                    {
                        foreach (PlayerEntity pe in OtherPlayers)
                        {
                            if (pe.PlayerID == playerID)
                            {
                                ChatInterface.AddChat(new ChatEntry("Server", pe.PlayerName + " has changed name to " + newName, Color.Blue, false));
                                pe.PlayerName = newName;
                            }
                        }
                    }

                    break;

            }
        }

        internal static void lightUpAroundRadius(int blockX, int blockY, int radius, int level = 1, int sign = 1)
        {/*
            if (blockX - radius < 0 || blockX + radius >= GameServer.WorldSizeX || blockY - radius < 0 || blockY + radius >= GameServer.WorldSizeY) return;
            Vector2 playerTile = new Vector2(blockX, blockY);
            int startX = (int)MathHelper.Clamp(playerTile.X - radius, 0, playerTile.X);
            int startY = (int)MathHelper.Clamp(playerTile.Y - radius, 0, playerTile.Y);
            int endX = (int)MathHelper.Clamp(playerTile.X + radius, playerTile.X - radius, WorldSizeX - 1);
            int endY = (int)MathHelper.Clamp(playerTile.Y + radius, playerTile.Y - radius, WorldSizeY - 1);
            for (int x = startX + 1; x < endX; x++)
            {
                for (int y = startY + 1; y < endY; y++)
                {
                    if (level == 1)
                        WorldBlocksSeen[x, y, level] = 1;
                    if (level == 0)
                        WorldBlocksSeen[x, y, level] += sign;
                }
            }
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY + 1; y < endY; y++)
                {
                    if (level == 1)
                        WorldBlocksSeen[x, y, level] = 1;
                    if (level == 0)
                        WorldBlocksSeen[x, y, level] += sign;
                }
            }
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX + 1; x < endX; x++)
                {
                    if (level == 1)
                        WorldBlocksSeen[x, y, level] = 1;
                    if (level == 0)
                        WorldBlocksSeen[x, y, level] += sign;
                }
            }*/
        }

        #region interfaces
        public void addToUpdateList()
        {
            Main.updatables.Add(this);
        }

        public void addToList()
        {
            addToDrawList();
            addToUpdateList();
        }

        public void removeFromList()
        {
            removeFromDrawList();
            removeFromUpdateList();
        }

        public void removeFromUpdateList()
        {
            Main.updatables.Remove(this);
        }

        public void addToDrawList()
        {
            Main.drawables.Add(this);
        }

        public void removeFromDrawList()
        {
            Main.drawables.Remove(this);
        }

        public bool inCamera()
        {
            return true;
        }
        #endregion
    }
}
