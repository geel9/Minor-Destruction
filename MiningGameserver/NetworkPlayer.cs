using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using MiningGameServer.Blocks;
using MiningGameServer.ExtensionMethods;
using MiningGameServer.Packets;
using MiningGameServer.Structs;
using MiningGameserver.Entities;
using MiningGameserver.Items;
using ItemStack = MiningGameserver.Structs.ItemStack;

namespace MiningGameServer
{
    public class NetworkPlayer : ServerEntityMoveable
    {
        public byte PlayerID = 0;

        public static int PlayerWidth
        {
            get
            {
                return 11;
            }
        }

        public static int PlayerHeight
        {
            get
            {
                return 23;
            }
        }

        public override AABB BoundBox
        {
            get
            {
                return new AABB((int)EntityPosition.X - PlayerWidth / 2, (int)EntityPosition.Y - PlayerHeight / 2, PlayerWidth, PlayerHeight);
            }
        }

        public NetConnection NetConnection;
        public Vector2 PlayerPosition;
        public List<char> PressedKeys = new List<char>();
        public List<ItemStack> PlayerInventory = new List<ItemStack>();
        private int _jumpTimer, _attackTimer;
        public float PlayerAimAngle = 0;
        public int PlayerInventorySelected;

        public byte MovementFlags, OldMovementFlags;

        public int PlayerHealth = 5;

        public byte[,] PlayerBlockIDCache;
        public byte[,] PlayerBlockMDCache;

        public string PlayerName = "PLAYER";

        public byte UpdateMask;

        private int _timeHeldAttack = 0;

        public int PlayerTeam = 0;

        public NetworkPlayer(byte playerID, NetConnection connection, Vector2 playerPos, string name)
        {
            NetConnection = connection;
            //PlayerEntity = new ServerPlayerEntity(playerPos, playerID, name);
            UpdateMask |= 1;
            UpdateMask |= (int)
            PlayerUpdateFlags.Player_Position_X;
            UpdateMask |= (int)
            PlayerUpdateFlags.Player_Position_Y;

            this.PlayerID = playerID;
            this.PlayerPosition = playerPos;
            this.PlayerName = name;
            EntityVelocity.X = 3;

            PlayerBlockIDCache = new byte[GameServer.WorldSizeX, GameServer.WorldSizeY];
            PlayerBlockMDCache = new byte[GameServer.WorldSizeX, GameServer.WorldSizeY];
        }

        public void UpdateCache()
        {
            var blockPos = new Vector2((int)(EntityPosition.X / GameServer.BlockWidth), (int)(EntityPosition.Y / GameServer.BlockHeight));

            short startX = (short)MathHelper.Clamp((int)blockPos.X - (800 / 16) - 1, 0, GameServer.WorldSizeX);
            short startY = (short)MathHelper.Clamp((int)blockPos.Y - (500 / 16) - 1, 0, GameServer.WorldSizeY);
            short endX = (short)MathHelper.Clamp((int)blockPos.X + (800 / 16) + 1, 0, GameServer.WorldSizeX);
            short endY = (short)MathHelper.Clamp((int)blockPos.Y + (500 / 16) + 1, 0, GameServer.WorldSizeY);

            Packet packet = new Packet();
            short numSending = 0;

            int oneX = 0;
            int oneY = 0;

            for (short x = startX; x < endX; x++)
            {
                for (short y = startY; y < endY; y++)
                {
                    byte cachedByte = PlayerBlockIDCache[x, y];
                    byte cachedByteM = PlayerBlockMDCache[x, y];

                    byte realByte = GameServer.WorldBlocks[x, y];
                    byte realByteM = GameServer.WorldBlocksMetaData[x, y];

                    if (cachedByte == realByte && realByteM == cachedByteM) continue;

                    PlayerBlockIDCache[x, y] = realByte;
                    PlayerBlockMDCache[x, y] = realByteM;

                    oneX = x;
                    oneY = y;

                    packet.writeByte((byte)(x - startX));
                    packet.writeByte((byte)(y - startY));
                    packet.writeByte(realByte);
                    packet.writeByte(realByteM);

                    numSending++;
                }
            }
            if (numSending > 1)
            {
                Packet packet2 = new Packet1SCGameEvent(GameServer.GameEvents.Block_Set_Chunk);
                packet2.writeShort(numSending);
                packet2.writeShort(startX);
                packet2.writeShort(startY);
                packet2.writeBytes(packet.data.ToArray());

                GameServer.ServerNetworkManager.SendPacket(packet2, NetConnection);
            }
            else if (numSending == 1)
            {
                Packet packet2 = new Packet1SCGameEvent(GameServer.GameEvents.Block_Set, (short)oneX, (short)oneY, GameServer.WorldBlocks[oneX, oneY], GameServer.WorldBlocksMetaData[oneX, oneY]);
                GameServer.ServerNetworkManager.SendPacket(packet2, NetConnection);
            }
        }

        public void Update(GameTime theTime)
        {
            UpdateCache();

            if (_jumpTimer > 0) _jumpTimer--;
            if (_attackTimer > 0) _attackTimer--;

            if ((MovementFlags & (int)PlayerMovementFlag.Jump_Pressed) != 0)
            {
                if (!Falling && _jumpTimer <= 0)
                {
                    EntityVelocity.Y -= 10;
                    _jumpTimer = 20;
                }
            }
            if ((MovementFlags & (int)PlayerMovementFlag.Left_Pressed) != 0)
            {
                EntityVelocity.X = MathHelper.Clamp(EntityVelocity.X - 3, -3, 3);
            }

            if ((MovementFlags & (int)PlayerMovementFlag.Right_Pressed) != 0)
            {
                EntityVelocity.X = MathHelper.Clamp(EntityVelocity.X + 3, -3, 3);
            }

            if ((MovementFlags & (int)PlayerMovementFlag.Attack_Pressed) != 0)
            {
                _timeHeldAttack++;
                if (_timeHeldAttack > 30) _timeHeldAttack = 30;
            }

            if ((OldMovementFlags & (int)PlayerMovementFlag.Attack_Pressed) != 0)
            {
                if ((MovementFlags & (int)PlayerMovementFlag.Attack_Pressed) == 0)
                {
                    if (_attackTimer <= 0)
                    {
                        if (_timeHeldAttack == 0) _timeHeldAttack = 1;
                        _attackTimer = 20;
                        int nextslot = GameServer.GetFreeProjectileSlot();
                        if (nextslot != -1)
                        {
                            short angle = (short) PlayerAimAngle.RToD();

                            var packet = new Packet2SCCreateProjectile((byte)nextslot, 1,
                                                                       (short)EntityPosition.X,
                                                                       (short)
                                                                       ((short)EntityPosition.Y - 10),
                                                                       angle, (byte)((_timeHeldAttack / 5) + 10),
                                                                       PlayerID);
                            GameServer.ServerNetworkManager.SendPacket(packet);

                            GameServer.GameProjectiles[nextslot] =
                                new ServerProjectileArrow(new Vector2(EntityPosition.X,
                                                                EntityPosition.Y - 10), angle,
                                                    PlayerID, (_timeHeldAttack / 5) + 10) { ProjectileID = (byte)nextslot };

                        }
                    }
                    _timeHeldAttack = 0;
                }
            }


            Vector2 oldPos = new Vector2(EntityPosition.X, EntityPosition.Y);

            EntityMovement();

            if (oldPos != EntityPosition)
            {
                UpdateMask |= (int)PlayerUpdateFlags.Player_Update;
                if (oldPos.X != EntityPosition.X)
                    UpdateMask |= (int)PlayerUpdateFlags.Player_Position_X;
                if (oldPos.Y != EntityPosition.Y)
                    UpdateMask |= (int)PlayerUpdateFlags.Player_Position_Y;
            }
            OldMovementFlags = MovementFlags;
        }

        private void PlayerCollisions()
        {
            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p == this) continue;

                AABBResult collide = p.BoundBox.AxisCollide(BoundBox);
                if (!collide.IsIntersecting) continue;

                if (collide.XSmaller)
                {
                    EntityVelocity.X = 0;
                    EntityPosition.X -= collide.X;
                }
                else
                {
                    EntityVelocity.Y = 0;
                    EntityPosition.Y -= collide.Y;
                }
            }
        }

        private void BlockCollisions()
        {
            List<Vector2> newTiles = RectangleHitsTiles(BoundBox);

            foreach (Vector2 newTile in newTiles)
            {
                byte blockID = GameServer.GetBlockIDAt(newTile.X, newTile.Y);

                if (blockID == 0) continue;

                Block block = Block.GetBlock(blockID);
                bool walkThrough = block.GetBlockWalkThrough();

                //A wall
                Rectangle blockBB = block.GetBlockBoundBox((int)newTile.X, (int)newTile.Y);

                AABB thisAABB = BoundBox;
                AABB blockAABB = new AABB(blockBB);
                AABBResult collide = thisAABB.AxisCollide(blockAABB);

                if (!collide.IsIntersecting) continue;

                if (collide.XSmaller)
                {
                    bool right = (collide.X < 0);
                    if (!walkThrough)
                    {
                        EntityVelocity.X = 0;
                        EntityPosition.X += collide.X;
                    }
                    block.OnBlockTouched((int)newTile.X, (int)newTile.Y, right ? 3 : 1, this);
                }
                else
                {
                    bool up = (collide.Y > 0);
                    //if (up && EntityVelocity.Y < 0) continue;
                    if (!walkThrough)
                    {
                        EntityVelocity.Y = 0;
                        EntityPosition.Y += collide.Y;
                        if (!up)
                        {
                            TimeFalling = 0;
                            Falling = false;
                        }
                    }
                    block.OnBlockTouched((int)newTile.X, (int)newTile.Y, up ? 2 : 0, this);
                }
            }
        }

        public override void EntityMovement()
        {
            if (BoundBox.Left < 0) EntityPosition.X = BoundBox.Width / 2 + 1;
            if (BoundBox.Top < 0) EntityPosition.Y = BoundBox.Height / 2 + 1;
            if (BoundBox.Right > GameServer.BlockWidth * GameServer.WorldSizeX) EntityPosition.X = GameServer.BlockWidth * GameServer.WorldSizeX - (BoundBox.Width / 2);
            if (BoundBox.Bottom > GameServer.BlockHeight * GameServer.WorldSizeY) EntityPosition.Y = GameServer.BlockHeight * GameServer.WorldSizeY - (BoundBox.Height / 2);

            //Didn't want to make a new BoundBox so this'll do. Gets the tiles the player will be in with his velocity.
            EntityPosition += EntityVelocity;

            if (EntityVelocity != Vector2.Zero)
                PlayerCollisions();

            BlockCollisions();

            //Ropes
            byte blockID2 = GameServer.GetBlockIDAt(GetEntityTile().X, GetEntityTile().Y);

            if (EntityVelocity.Y < 6 && blockID2 != 6) EntityVelocity.Y += 1; // Gravity! This is a really nice side effect: The code for not allowing the player to go through a block downwards already exists, so I just need to add this one line to add gravity!

            if (blockID2 == 6)
            {
                EntityVelocity = EntityVelocity.ApproachZeroY();
            }

            EntityVelocity = EntityVelocity.ApproachZeroX();

            if (EntityVelocity.Y != 1) Falling = true;
        }

        public bool HasItem(byte id)
        {
            return GetPlayerItemStackFromInventory(id).ItemID == id;
        }

        public ItemStack GetPlayerItemStackFromInventory(byte id)
        {
            return PlayerInventory.Where(x => x.ItemID == id).FirstOrDefault();
        }

        public int GetNumItemInInventory(byte id)
        {
            return PlayerInventory.Where(x => x.ItemID == id).FirstOrDefault().NumberItems;
        }

        public void RemoveItems(byte itemID, int numToRemove)
        {
            if (GetNumItemInInventory(itemID) < numToRemove) return;
            ItemStack i = GetPlayerItemStackFromInventory(itemID);
            int index = PlayerInventory.IndexOf(i);
            i.NumberItems -= numToRemove;
            if (i.NumberItems == 0)
            {
                if (index < PlayerInventorySelected) PlayerInventorySelected++;
                PlayerInventory.RemoveAt(index);
                Packet p = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Remove, (byte)index);
                GameServer.ServerNetworkManager.SendPacket(p, NetConnection);

            }
            else
            {
                PlayerInventory[index] = i;
                Packet pack = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Update, (byte)index, (byte)i.ItemID, i.NumberItems);
                GameServer.ServerNetworkManager.SendPacket(pack, NetConnection);
            }
        }

        public ServerItem GetPlayerItemInHand()
        {
            if (PlayerInventorySelected >= PlayerInventory.Count) PlayerInventorySelected = -1;
            if (PlayerInventorySelected == -1) return null;
            return ServerItem.GetItem(PlayerInventory[PlayerInventorySelected].ItemID);
        }

        public void PickupItem(ItemStack item)
        {
            for (int i = 0; i < PlayerInventory.Count; i++)
            {
                ItemStack it = PlayerInventory[i];
                if (it.ItemID == item.ItemID)
                {
                    PlayerInventory[i] = new ItemStack(it.NumberItems + item.NumberItems, it.ItemID);
                    Packet pack = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Update, (byte)i, (byte)PlayerInventory[i].ItemID, PlayerInventory[i].NumberItems);
                    GameServer.ServerNetworkManager.SendPacket(pack, NetConnection);
                    return;
                }
            }
            PlayerInventory.Add(item);
            Packet p = new Packet1SCGameEvent(GameServer.GameEvents.Player_Inventory_Add, (byte)item.ItemID, item.NumberItems);
            GameServer.ServerNetworkManager.SendPacket(p, NetConnection);
        }
    }
}
