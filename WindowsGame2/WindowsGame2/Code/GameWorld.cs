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
using MiningGameServer.Entities;
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
        public static List<EntityDroppedItem> DroppedItems = new List<EntityDroppedItem>();

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
            for (int x = 0; x < WorldSizeX; x++)
            {
                for (int y = 0; y < WorldSizeY; y++)
                    WorldBlocks[x, y] = new BlockData();
            }
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
            return GetBlockAt((int)x, (int)y);
        }

        public void Update(GameTime time)
        {
            //ConsoleManager.setVariableValue("window_title", "Blocking: " + InterfaceManager.blocking);
            ThePlayer.Update(time);
            if (ThePlayer.PlayerEntity != null)
                CameraManager.setCameraPositionCenterMin(ThePlayer.PlayerEntity.EntityPosition, Vector2.Zero);

            List<EntityDroppedItem> toRemove = new List<EntityDroppedItem>();
            foreach(EntityDroppedItem item in DroppedItems)
            {
                item.Update(time);
                if(item.ShouldDestroy)
                    toRemove.Add(item);
            }

            foreach (EntityDroppedItem item in toRemove)
                DroppedItems.Remove(item);

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
                Item item = Item.GetItem(i.ItemID);
                inventory += item.GetName() + ": " + i.NumberItems + "\n";
            }
            if (ThePlayer.PlayerInventory.Count == 0)
                inventory += "Nothing!\n";

            ThePlayer.Draw(sb);
            foreach (PlayerEntity oth in OtherPlayers)
            {
                oth.Draw(sb);
            }

            foreach(EntityDroppedItem item in DroppedItems)
                item.Draw(sb);

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

        public static PlayerEntity PlayerOfID(byte ID)
        {
            if (ThePlayer.PlayerEntity.PlayerID == ID)
                return ThePlayer.PlayerEntity;

            return OtherPlayers.FirstOrDefault(pe => pe.PlayerID == ID);
        }

        public static EntityDroppedItem DroppedItemOfID(short ID)
        {
            return DroppedItems.FirstOrDefault(it => it.DroppedItemID == ID);
        }

        public static void HandlePacket(byte packetID, Packet p)
        {
            switch (packetID)
            {
                case 0:
                    string pName = p.ReadString();
                    byte id = p.ReadByte();

                    int posX = p.ReadInt();
                    int posY = p.ReadInt();
                    if (OtherPlayers.Where(pl => pl.PlayerID == id).Count() > 0) return;
                    if (ThePlayer.PlayerEntity.PlayerID != id)
                    {
                        ConsoleManager.Log("New player: " + pName + " id: " + id + " x: " + posX + " y: " + posY);
                        OtherPlayers.Add(new PlayerEntity(new Vector2(posX, posY), id, pName));
                    }
                    else
                    {
                        ThePlayer.PlayerEntity.EntityPosition = new Vector2(posX, posY);
                    }
                    break;
                case 1:
                    HandleGameEvent(p.ReadByte(), p);
                    break;

                case 2:
                    byte type = p.ReadByte();
                    byte ID = p.ReadByte();
                    short X = p.ReadShort();
                    short Y = p.ReadShort();
                    short angle = p.ReadShort();
                    byte strength = p.ReadByte();
                    byte owner = p.ReadByte();
                    GameProjectiles.Add(new ProjectileArrow(new Vector2(X, Y), angle, owner, strength) { ProjectileID = ID });
                    break;

                case 3:
                    byte toRemove = p.ReadByte();
                    EntityProjectile proj = null;
                    foreach (EntityProjectile projectile in GameProjectiles)
                    {
                        if (projectile.ProjectileID == toRemove)
                        {
                            proj = projectile;
                            break;
                        }
                    }
                    if (proj != null) GameProjectiles.Remove(proj);
                    break;

                case 6:
                    byte playerID = p.ReadByte();
                    PlayerEntity player = PlayerOfID(playerID);
                    if (player == null) break;
                    player.OnAttack();
                    break;

                case 7:
                    playerID = p.ReadByte();
                    byte itemID = p.ReadByte();
                    player = PlayerOfID(playerID);
                    if (player == null) break;
                    player.EquippedItem = Item.GetItem(itemID);

                    break;

                case 8:
                    Vector2 pos = p.ReadVectorS();
                    Vector2 velocity = p.ReadVectorS();
                    short droppedID = p.ReadShort();
                    itemID = p.ReadByte();
                    EntityDroppedItem item = DroppedItemOfID(droppedID);
                    if (item != null)
                        break;

                    item = new EntityDroppedItem(pos, velocity, itemID, droppedID);
                    DroppedItems.Add(item);
                    break;

                case 9:
                    droppedID = p.ReadShort();
                    byte pickerUpper = p.ReadByte();
                    player = PlayerOfID(pickerUpper);
                    item = DroppedItemOfID(droppedID);
                    if (item == null || player == null) break;
                    item.MovingTowards = player;
                    break;

                case 10:
                    droppedID = p.ReadShort();
                    item = DroppedItemOfID(droppedID);
                    if (item != null)
                        DroppedItems.Remove(item);
                    break;

                case 200:
                    PlayerUpdating(p);
                    break;

                case 255:
                    id = p.ReadByte();
                    ConsoleManager.Log("My id is " + id);
                    ThePlayer.PlayerEntity = new PlayerEntity(new Vector2(0, 0), id, ConsoleManager.getVariableValue("player_name"));
                    break;
            }
        }

        public static void PlayerUpdating(Packet p)
        {
            int numToUpdate = p.ReadByte();
            List<byte> playersUpdated = new List<byte>();
            List<byte> allPlayers = GameWorld.OtherPlayers.Select(pl => pl.PlayerID).ToList();
            for (int i = 0; i < numToUpdate; i++)
            {
                int playerID = p.ReadByte();
                playersUpdated.Add((byte)playerID);
                PlayerEntity player;
                if (playerID == GameWorld.ThePlayer.PlayerEntity.PlayerID)
                    player = GameWorld.ThePlayer.PlayerEntity;
                else
                {
                    player = GameWorld.OtherPlayers.Where(pl => pl.PlayerID == playerID).FirstOrDefault();
                }
                if (player == null) player = new PlayerEntity(Vector2.Zero, (byte)playerID);
                byte updateMask = p.ReadByte();

                if ((updateMask & (int)PlayerUpdateFlags.Player_Position_X) != 0)
                {
                    short x = p.ReadShort();
                    player.EntityPosition.X = x;
                }
                if ((updateMask & (int)PlayerUpdateFlags.Player_Position_Y) != 0)
                {
                    short y = p.ReadShort();
                    player.EntityPosition.Y = y;
                }
                if ((updateMask & (int)PlayerUpdateFlags.Player_Movement_Flags) != 0)
                {
                    byte flags = p.ReadByte();
                    player.OtherPlayerNetworkFlags = flags;
                    bool leftPress = (flags & (int)PlayerMovementFlag.Left_Pressed) != 0;
                    bool rightPress = (flags & (int)PlayerMovementFlag.Right_Pressed) != 0;
                    bool idle = (flags & (int)PlayerMovementFlag.Idle) != 0;
                    if (leftPress || rightPress)
                    {
                        player.FacingLeft = leftPress && !rightPress;
                        if (!(leftPress && rightPress))
                        {
                            player.TorsoAnimateable.StartLooping("player_run_start", "player_run_end");
                            player.LegsAnimateable.StartLooping("player_run_start", "player_run_end");
                        }
                        else
                        {
                            player.TorsoAnimateable.StartLooping("player_idle", "player_idle");
                            player.LegsAnimateable.StartLooping("player_idle", "player_idle");
                        }
                    }
                    if (idle)
                    {
                        player.TorsoAnimateable.StartLooping("player_idle", "player_idle");
                        player.LegsAnimateable.StartLooping("player_idle", "player_idle");
                    }
                }
            }
        }

        public static void HandleGameEvent(byte eventID, Packet p)
        {
            switch ((GameServer.GameEvents)eventID)
            {
                case GameServer.GameEvents.Block_Set:
                    SetBlock(p.ReadShort(), p.ReadShort(), p.ReadShort(), false, p.ReadByte());
                    break;

                case GameServer.GameEvents.Block_Set_ID:
                    SetBlock(p.ReadShort(), p.ReadShort(), p.ReadShort(), false, 0);
                    break;

                case GameServer.GameEvents.Block_Set_MD:
                    short X = p.ReadShort();
                    short Y = p.ReadShort();
                    SetBlock(X, Y, WorldBlocks[X, Y].ID, false, p.ReadByte());
                    break;

                case GameServer.GameEvents.Block_Set_Chunk:
                    int numSending = p.ReadShort();
                    short startX = p.ReadShort();
                    short startY = p.ReadShort();

                    for (int i = 0; i < numSending; i++)
                    {
                        X = (short)(p.ReadByte() + startX);
                        Y = (short)(p.ReadByte() + startY);
                        short Id = p.ReadShort();
                        byte metadata = p.ReadByte();
                        SetBlock(X, Y, Id, true, metadata);
                    }

                    break;

                case GameServer.GameEvents.Player_Chat:
                    byte playerID = p.ReadByte();
                    bool teamChat = p.ReadBool();
                    string chatText = p.ReadString();
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
                    playerID = p.ReadByte();
                    int x = p.ReadInt();
                    int y = p.ReadInt();
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
                    playerID = p.ReadByte();
                    x = p.ReadShort();
                    y = p.ReadShort();
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
                    byte pId = p.ReadByte();
                    OtherPlayers.Remove(OtherPlayers.Where(pl => pl.PlayerID == pId).FirstOrDefault());
                    ConsoleManager.Log("Player " + pId + " left.");
                    break;

                case GameServer.GameEvents.Player_Inventory_Update:
                    byte index = p.ReadByte();
                    byte id = p.ReadByte();
                    int num = p.ReadInt();
                    if (index < ThePlayer.PlayerInventory.Count)
                    {
                        ThePlayer.PlayerInventory[index] = new ItemStack(num, id);
                    }
                    break;

                case GameServer.GameEvents.Player_Inventory_Add:
                    ItemStack it = p.ReadNT<ItemStack>();
                    ThePlayer.PlayerInventory.Add(it);
                    break;

                case GameServer.GameEvents.Player_Inventory_Remove:
                    ThePlayer.PlayerInventory.RemoveAt(p.ReadByte());
                    break;

                case GameServer.GameEvents.Player_Change_Name:
                    playerID = p.ReadByte();
                    string newName = p.ReadString();

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

        internal static void LightUpAroundRadius(int blockX, int blockY, int radius, int level = 1, int sign = 1)
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
