using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Entities;
using System.IO;
using MiningGame.Code.Interfaces;
using MiningGame.Code.Structs;
using Microsoft.Xna.Framework.Input;
using MiningGame.Code.Server;
using YogUILibrary.Managers;

namespace MiningGame.Code
{
    public class GameWorld : UpdatableAndDrawable
    {
        public const int WorldSizeX = GameServer.WorldSizeX, WorldSizeY = GameServer.WorldSizeY;
        public const int BlockWidth = 16, BlockHeight = 16, PlayerVision = 4;
        public static Random random = new Random();

        public static List<PlayerEntity> otherPlayers = new List<PlayerEntity>();
        public static PlayerController thePlayer = new PlayerController();

        public static byte[,] worldBlocks = new byte[WorldSizeX, WorldSizeY];
        public static byte[,] worldBlocksMetaData = new byte[WorldSizeX, WorldSizeY];

        public static int[, ,] worldBlocksSeen = new int[WorldSizeX, WorldSizeY, 2];

        public static void LoadBlocks()
        {
            Block.AllBlocks.Clear();
            string path = DirectoryManager.BLOCKS;
            List<string> codes = new List<string>();
            string[] files = Directory.GetFiles(path).Where(name => name.Contains(".bl")).ToArray();
            foreach (string i in files)
            {
                string contents = FileReaderManager.ReadFileContents(i);
                codes.Add(contents);
            }
            List<Block> blocks = CodeExecutorManager.BuildBlock(codes.ToArray(), files.Select(s => s.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "")).ToArray());

            foreach (Block b in blocks)
            {
                if (b != null) b.finalizeBlock();
            }
        }

        public GameWorld()
        {
            addToList();
            thePlayer.Start();
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

        public static bool isBlockLit(int x, int y)
        {
            return worldBlocksSeen[x, y, 0] > 0 || worldBlocksSeen[x, y, 1] > 0;
        }

        public static int getBlockLitLevel(int x, int y)
        {
            if (isBlockFullyLit(x, y)) return 2;
            if (worldBlocksSeen[x, y, 1] > 0) return 1;
            return 0;
        }

        public static bool isBlockFullyLit(int x, int y)
        {
            Vector2 playerTile = AbsoluteToTile(thePlayer.playerEntity.entityPosition);
            float xDist = Math.Abs(playerTile.X - x);
            float yDist = Math.Abs(playerTile.Y - y);
            return worldBlocksSeen[x, y, 0] > 0 || (xDist < PlayerVision && yDist < PlayerVision);
        }

        public static int shouldRenderBlock(int x, int y)
        {
            return 2;
            //return getBlockLitLevel(x, y);
        }

        public static byte GetBlockIDAt(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? worldBlocks[x, y] : (byte)0;
        }

        public static byte GetBlockIDAt(float x, float y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? worldBlocks[(int)x, (int)y] : (byte)0;
        }

        public static Block GetBlockAt(int x, int y)
        {
            return Block.getBlock(GetBlockIDAt(x, y));
        }

        public static Block GetBlockAt(float x, float y)
        {
            return Block.getBlock(GetBlockIDAt(x, y));
        }

        public static byte GetBlockMDAt(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? worldBlocksMetaData[x, y] : (byte)0;
        }

        public static byte GetBlockMDAt(float x, float y)
        {
            return (x >= 0 && y >= 0 && x < WorldSizeX && y < WorldSizeY) ? worldBlocksMetaData[(int)x, (int)y] : (byte)0;
        }

        public static bool CanWalkThrough(byte id)
        {
            return Block.getBlock(id).getBlockWalkThrough();
        }

        public void Update(Microsoft.Xna.Framework.GameTime time)
        {
            //ConsoleManager.setVariableValue("window_title", "Blocking: " + InterfaceManager.blocking);
            thePlayer.Update(time);
            if (thePlayer.playerEntity != null)
                CameraManager.setCameraPositionCenterMin(thePlayer.playerEntity.entityPosition, Vector2.Zero);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (thePlayer.playerEntity == null) return;
            Vector2 playerTile = AbsoluteToTile(thePlayer.playerEntity.entityPosition);
            Rectangle cameraBounds = CameraManager.cameraBoundBox;
            int blockStartX = (int)MathHelper.Clamp(cameraBounds.Left / BlockWidth, 0, WorldSizeX - 1);
            int blockStartY = (int)MathHelper.Clamp(cameraBounds.Top / BlockHeight, 0, WorldSizeY - 1);
            int blockEndX = (int)MathHelper.Clamp((cameraBounds.Right / BlockWidth) + 1, 0, WorldSizeX - 1);
            int blockEndY = (int)MathHelper.Clamp((cameraBounds.Bottom / BlockHeight) + 1, 0, WorldSizeY - 1);

            Vector2 mousePos = InputManager.GetMousePosV();
            Vector2 blockMouseOver = AbsoluteToTile(mousePos);
            blockMouseOver.X = MathHelper.Clamp(blockMouseOver.X, 0, WorldSizeX - 1);
            blockMouseOver.Y = MathHelper.Clamp(blockMouseOver.Y, 0, WorldSizeY - 1);
            byte mouseOverID = worldBlocks[(int)blockMouseOver.X, (int)blockMouseOver.Y];

            Vector2 curDig = thePlayer.curMining;
            float digMult = MathHelper.Clamp(1f - (float)((float)thePlayer.digPct / 100f), 0, 1);
            for (int x = blockStartX; x < blockEndX; x++)
            {
                for (int y = blockStartY; y < blockEndY; y++)
                {
                    Color backColor = Color.SandyBrown;
                    Texture2D backTexture = AssetManager.GetTexture("background_1");
                    if (y < 10)
                    {
                        backColor = Main.backColor;
                    }
                    Vector2 drawPos = new Vector2(x * BlockWidth + (BlockWidth / 2), y * BlockHeight + (BlockHeight / 2));
                    drawPos -= CameraManager.cameraPosition;
                    byte blockID = worldBlocks[x, y];
                    int render = shouldRenderBlock(x, y);
                    if (render > 0)
                    {
                        if (y < 10)
                            DrawManager.Draw_Box(drawPos, BlockWidth, BlockHeight, backColor, sb);
                        else
                        {
                            sb.Draw(backTexture, drawPos - new Vector2(BlockWidth / 2, BlockHeight / 2), Color.White);
                        }

                        if (blockID != 0)
                        {
                            Block block = Block.getBlock(blockID);
                            bool flag = (x == curDig.X && y == curDig.Y);
                            bool flagHide = block.getBlockHide() && render == 1;

                            if (block.getBlockRenderSpecial() || y < 10)
                            {
                                Texture2D blockTexture = block.getRenderBlock()(x, y, sb);
                                if (blockTexture != null)
                                    sb.Draw(blockTexture, drawPos, null, Color.White, 0f, new Vector2(BlockWidth / 2, BlockHeight / 2), (flag ? digMult : 1), SpriteEffects.None, 0);
                            }
                            else
                            {
                                Color color = block.getBlockRenderColor();
                                //DrawManager.Draw_Box(drawPos, blockWidth * (flag ? digMult : 1), blockHeight * (flag ? digMult : 1), color, sb, 0f);
                            }
                        }
                    }
                    else
                    {
                        //if (blockID != 0)
                        DrawManager.Draw_Box(drawPos, BlockWidth, BlockHeight, Color.Black, sb, 0f);
                    }
                }
            }
            string inventory = "\nInventory: \n";

            Item inI = thePlayer.getPlayerItemInHand();
            if (inI != null)
            {
                inventory = "\n\nItem in hand: " + inI.getName() + "\n" + inventory;
            }
            else
            {
                inventory = "\n\nItem in hand: nothing\n" + inventory;
            }
            foreach (ItemStack i in thePlayer.playerInventory)
            {
                Item item = Item.getItem(i.itemID);
                inventory += item.getName() + ": " + i.numberItems + "\n";
            }
            if (thePlayer.playerInventory.Count == 0) inventory += "Nothing!\n";
            thePlayer.Draw(sb);
            /*
            if (curBlockConnecting != new Point(-1, -1))
            {
                Vector2 pos = ConversionManager.PToV(curBlockConnecting);
                pos += new Vector2(0.5f, 0.5f);
                pos *= new Vector2(blockWidth, blockHeight);
                pos -= CameraManager.cameraPosition;
                DrawManager.Draw_Line(pos, InputManager.GetMousePosV(), Color.White, sb);

            }*/
            foreach (BlockConnection bc in GameServer.BlockConnections)
            {
                Vector2 point1 = bc.blockConnection1.blockPosition;
                Vector2 point2 = bc.blockConnection2.blockPosition;
                point1 += new Vector2(0.5f, 0.5f);
                point1 *= new Vector2(BlockWidth, BlockHeight);
                point1 -= CameraManager.cameraPosition;
                point2 += new Vector2(0.5f, 0.5f);
                point2 *= new Vector2(BlockWidth, BlockHeight);
                point2 -= CameraManager.cameraPosition;
                DrawManager.Draw_Line(point1, point2, Color.Red, sb);
            }
            foreach (PlayerEntity oth in otherPlayers)
            {
                oth.Draw(sb);
            }
        }

        public static Vector2 AbsoluteToTile(Vector2 tile)
        {
            return new Vector2((int)(tile.X / BlockWidth), (int)(tile.Y / BlockHeight));
        }

        public static void setBlock(int x, int y, byte blockID, bool notify = true, byte metaData = 0)
        {
            if (x < WorldSizeX && y < WorldSizeY && blockID >= 0)
            {
                if (blockID != GetBlockIDAt(x, y) && GetBlockIDAt(x, y) != 0) Block.getBlock(GetBlockIDAt(x, y)).getBlockRemoved(x, y)(x, y);
                worldBlocksMetaData[x, y] = metaData;
                worldBlocks[x, y] = blockID;
                if (blockID != 0) Block.getBlock(blockID).getBlockPlaced(x, y, notify)(x, y, notify);
            }
        }

        public static void setBlockMetaData(int x, int y, byte metadata)
        {
            worldBlocksMetaData[x, y] = metadata;
        }

        public static void HandleGameEvent(byte eventID, Packet p)
        {
            switch ((GameServer.GameEvents)eventID)
            {
                case GameServer.GameEvents.Block_Set:
                    setBlock(p.readInt(), p.readInt(), p.readByte(), false, p.readByte());
                    break;

                case GameServer.GameEvents.Player_Chat:
                    byte playerID = p.readByte();
                    bool teamChat = p.readBool();
                    string chatText = p.readString();
                    if (playerID == thePlayer.playerEntity.PlayerID)
                    {
                        ChatInterface.chatEntries.Add(new ChatEntry(thePlayer.playerEntity.PlayerName, chatText, Color.White, teamChat));
                    }
                    else if (playerID != 0)
                    {
                        foreach (PlayerEntity pe in otherPlayers)
                        {
                            if (pe.PlayerID == playerID)
                            {
                                ChatInterface.chatEntries.Add(new ChatEntry(pe.PlayerName, chatText, Color.White, teamChat));
                            }
                        }
                    }
                    else
                    {
                        ChatInterface.chatEntries.Add(new ChatEntry("Server", chatText, Color.Blue, false));
                    }
                    break;

                case GameServer.GameEvents.Player_Position:
                    playerID = p.readByte();
                    int x = p.readInt();
                    int y = p.readInt();
                    if (playerID == thePlayer.playerEntity.PlayerID)
                    {
                        thePlayer.playerEntity.entityPosition = new Vector2(x, y);
                    }
                    else
                    {
                        foreach (PlayerEntity pe in otherPlayers)
                        {
                            if (pe.PlayerID == playerID)
                            {
                                pe.entityPosition = new Vector2(x, y);
                            }
                        }
                    }
                    break;

                case GameServer.GameEvents.Player_Aim_And_Position:
                    playerID = p.readByte();
                    x = p.readShort();
                    y = p.readShort();
                    if (playerID == thePlayer.playerEntity.PlayerID)
                    {
                        thePlayer.playerEntity.entityPosition = new Vector2(x, y);
                    }
                    else
                    {
                        foreach (PlayerEntity pe in otherPlayers)
                        {
                            if (pe.PlayerID == playerID)
                            {
                                pe.entityPosition = new Vector2(x, y);
                            }
                        }
                    }
                    break;

                case GameServer.GameEvents.Player_Leave:
                    byte pId = p.readByte();
                    otherPlayers.Remove(otherPlayers.Where(pl => pl.PlayerID == pId).FirstOrDefault());
                    ConsoleManager.Log("Player " + pId + " left.");
                    break;

                case GameServer.GameEvents.Player_Inventory_Update:
                    byte index = p.readByte();
                    byte id = p.readByte();
                    int num = p.readInt();
                    if (index < thePlayer.playerInventory.Count)
                    {
                        thePlayer.playerInventory[index] = new ItemStack(num, id);
                    }
                    break;

                case GameServer.GameEvents.Player_Inventory_Add:
                    byte itemID = p.readByte();
                    int itemNum = p.readInt();
                    thePlayer.playerInventory.Add(new ItemStack(itemNum, itemID));
                    break;

                case GameServer.GameEvents.Player_Inventory_Remove:
                    thePlayer.playerInventory.RemoveAt(p.readByte());
                    break;

            }
        }

        internal static void lightUpAroundRadius(int blockX, int blockY, int radius, int level = 1, int sign = 1)
        {
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
                        worldBlocksSeen[x, y, level] = 1;
                    if (level == 0)
                        worldBlocksSeen[x, y, level] += sign;
                }
            }
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY + 1; y < endY; y++)
                {
                    if (level == 1)
                        worldBlocksSeen[x, y, level] = 1;
                    if (level == 0)
                        worldBlocksSeen[x, y, level] += sign;
                }
            }
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX + 1; x < endX; x++)
                {
                    if (level == 1)
                        worldBlocksSeen[x, y, level] = 1;
                    if (level == 0)
                        worldBlocksSeen[x, y, level] += sign;
                }
            }
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
