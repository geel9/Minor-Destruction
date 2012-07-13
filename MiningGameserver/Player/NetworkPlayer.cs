using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using MiningGameServer.ExtensionMethods;
using MiningGameServer.Packets;
using MiningGameServer.Player;
using MiningGameServer.PlayerClasses;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;
using MiningGameServer;
using MiningGameServer.Blocks;
using MiningGameServer.Entities;
using MiningGameServer.Items;
using MiningGameServer.Packets;
using MiningGameServer.Shapes;
using ItemStack = MiningGameServer.Structs.ItemStack;

namespace MiningGameServer
{
    public class NetworkPlayer : ServerEntityMoveable
    {
        public byte PlayerID = 0;

        public int PlayerWidth
        {
            get { return PClass.BoundBox.Width; }
        }

        public int PlayerHeight
        {
            get { return PClass.BoundBox.Height; }
        }

        public override ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB((int)EntityPosition.X - PlayerWidth / 2, (int)EntityPosition.Y - PlayerHeight / 2, PlayerWidth, PlayerHeight);
            }
        }

        public NetConnection NetConnection;
        public List<char> PressedKeys = new List<char>();

        public PlayerInventory Inventory;
        public PlayerClass PClass;

        private int _jumpTimer;
        public int AttackTimer;
        public float PlayerAimAngle = 0;
        public bool FacingLeft = false;

        public bool LeftPressed
        {
            get { return (MovementFlags & (int)PlayerMovementFlag.Left_Pressed) != 0; }
        }

        public bool RightPressed
        {
            get { return (MovementFlags & (int)PlayerMovementFlag.Right_Pressed) != 0; }
        }

        public bool OldLeftPressed
        {
            get { return (OldMovementFlags & (int)PlayerMovementFlag.Left_Pressed) != 0; }
        }

        public bool OldRightPressed
        {
            get { return (OldMovementFlags & (int)PlayerMovementFlag.Right_Pressed) != 0; }
        }

        public bool AttackPressed
        {
            get { return (MovementFlags & (int)PlayerMovementFlag.Attack_Pressed) != 0; }
        }

        public bool OldAttackPressed
        {
            get { return (OldMovementFlags & (int)PlayerMovementFlag.Attack_Pressed) != 0; }
        }

        public byte MovementFlags, OldMovementFlags;

        public int PlayerHealth = 5;

        public BlockData[,] PlayerBlockCache;

        public string PlayerName = "PLAYER";

        public byte UpdateMask;

        private int _timeHeldAttack = 0;

        public int PlayerTeam = 0;

        public NetworkPlayer(byte playerID, NetConnection connection, Vector2 playerPos, string name)
        {
            NetConnection = connection;
            //PlayerEntity = new ServerPlayerEntity(playerPos, playerID, name);
            UpdateMask |= 1;
            UpdateMask |= (int)PlayerUpdateFlags.Player_Position;

            this.PlayerID = playerID;
            this.EntityPosition = playerPos;
            this.PlayerName = name;
            EntityVelocity.X = 3;

            PlayerBlockCache = new BlockData[GameServer.WorldSizeX, GameServer.WorldSizeY];
            
            Inventory = new PlayerInventory(this);

            PClass = new PlayerClassDestroyer(this);

            for (int x = 0; x < GameServer.WorldSizeX; x++)
                for (int y = 0; y < GameServer.WorldSizeY; y++)
                    PlayerBlockCache[x, y] = new BlockData();
        }

        public void UpdateCache()
        {
            var blockPos = new Vector2((int)(EntityPosition.X / GameServer.BlockSize), (int)(EntityPosition.Y / GameServer.BlockSize));

            short startX = (short)MathHelper.Clamp((int)blockPos.X - (800 / GameServer.BlockSize) - 1, 0, GameServer.WorldSizeX);
            short startY = (short)MathHelper.Clamp((int)blockPos.Y - (500 / GameServer.BlockSize) - 1, 0, GameServer.WorldSizeY);
            short endX = (short)MathHelper.Clamp((int)blockPos.X + (800 / GameServer.BlockSize) + 1, 0, GameServer.WorldSizeX);
            short endY = (short)MathHelper.Clamp((int)blockPos.Y + (500 / GameServer.BlockSize) + 1, 0, GameServer.WorldSizeY);

            Packet packet = new Packet();
            short numSending = 0;

            int oneX = 0;
            int oneY = 0;

            for (short x = startX; x < endX; x++)
            {
                for (short y = startY; y < endY; y++)
                {
                    short cachedID = PlayerBlockCache[x, y].ID;
                    byte cachedMD = PlayerBlockCache[x, y].MetaData;

                    short realID = GameServer.WorldBlocks[x, y].ID;
                    byte realMD = GameServer.WorldBlocks[x, y].MetaData;

                    if (cachedID == realID && realMD == cachedMD) continue;

                    PlayerBlockCache[x, y].ID = realID;
                    PlayerBlockCache[x, y].MetaData = realMD;

                    oneX = x;
                    oneY = y;

                    packet.WriteByte((byte)(x - startX));
                    packet.WriteByte((byte)(y - startY));
                    packet.WriteShort(realID);
                    packet.WriteByte(realMD);

                    numSending++;
                }
            }
            if (numSending > 1)
            {
                Packet packet2 = new Packet1SCGameEvent(GameServer.GameEvents.Block_Set_Chunk);
                packet2.WriteShort(numSending);
                packet2.WriteShort(startX);
                packet2.WriteShort(startY);
                packet2.WriteBytes(packet.data.ToArray());

                GameServer.ServerNetworkManager.SendPacket(packet2, NetConnection);
            }
            else if (numSending == 1)
            {
                Packet packet2 = new Packet1SCGameEvent(GameServer.GameEvents.Block_Set, (short)oneX, (short)oneY, GameServer.WorldBlocks[oneX, oneY].ID, GameServer.WorldBlocks[oneX, oneY].MetaData);
                GameServer.ServerNetworkManager.SendPacket(packet2, NetConnection);
            }
        }

        public void Update(GameTime theTime)
        {
            UpdateCache();

            if (PlayerHealth <= 0)
            {
                PlayerHealth = 5;
                EntityPosition = new Vector2(50, 50);
                EntityVelocity = Vector2.Zero;
            }
            else if (_jumpTimer > 0) _jumpTimer--;
            if (AttackTimer > 0)
            {
                AttackTimer--;
                if (AttackTimer == 5)
                {
                    UpdateMask |= (int)PlayerUpdateFlags.Player_Update;
                    UpdateMask |= (int)PlayerUpdateFlags.Player_Movement_Flags;
                    MovementFlags |= (int)PlayerMovementFlag.Idle;
                }
            }

            if ((MovementFlags & (int)PlayerMovementFlag.Jump_Pressed) != 0)
            {
                if (!Falling && _jumpTimer <= 0)
                {
                    EntityVelocity.Y -= 10;
                    _jumpTimer = 20;
                }
            }

            float playerRunSpeed = PClass.GetPlayerWalkVelocity();
            if (LeftPressed)
            {
                EntityVelocity.X = MathHelper.Clamp(EntityVelocity.X - playerRunSpeed, -playerRunSpeed, playerRunSpeed);
                FacingLeft = true;
            }

            if (RightPressed)
            {
                EntityVelocity.X = MathHelper.Clamp(EntityVelocity.X + playerRunSpeed, -playerRunSpeed, playerRunSpeed);
                FacingLeft = false;
            }

            if ((OldLeftPressed || OldRightPressed) && (!RightPressed && !LeftPressed))
            {
                UpdateMask |= (int)PlayerUpdateFlags.Player_Update;
                UpdateMask |= (int)PlayerUpdateFlags.Player_Movement_Flags;
                MovementFlags |= (int)PlayerMovementFlag.Idle;
            }

            if (AttackPressed)
            {
                _timeHeldAttack++;
                if (_timeHeldAttack > 30)
                    _timeHeldAttack = 30;
            }

            if (OldAttackPressed && !AttackPressed)
            {
                if (AttackTimer <= 0)
                {
                    if (_timeHeldAttack == 0)
                        _timeHeldAttack = 1;
                    AttackTimer = 20;
                    Attack();
                }
                _timeHeldAttack = 0;
            }


            Vector2 oldPos = new Vector2(EntityPosition.X, EntityPosition.Y);

            PClass.Update_PrePhys(theTime);

            EntityMovement();

            PClass.Update_PostPhys(theTime);

            if (oldPos != EntityPosition)
            {
                UpdateMask |= (int)PlayerUpdateFlags.Player_Update;
                UpdateMask |= (int)PlayerUpdateFlags.Player_Position;
            }
            OldMovementFlags = MovementFlags;
        }

        private void Attack()
        {
            ServerItem inHand = Inventory.GetPlayerItemInHand();
            if (inHand != null)
            {
                Packet6SCPlayerAttack packet6 = new Packet6SCPlayerAttack(PlayerID);
                GameServer.ServerNetworkManager.SendPacket(packet6);
                if (inHand is ServerItemBow)
                {
                    AttackArrows();
                }
                else if (inHand is ServerItemSword)
                {
                    AttackSword();
                }
            }
        }

        private void AttackSword()
        {
            int leftX = FacingLeft ? BoundBox.Left - 15 : BoundBox.Right;
            ShapeAABB bound = new ShapeAABB(new Rectangle(leftX, (int)BoundBox.Top + 5, 15, PlayerHeight - 6));
            List<Vector2> newTiles = RectangleHitsTiles(bound);

            foreach (Vector2 v in newTiles)
            {
                BlockData block = GameServer.GetBlockAt(v.X, v.Y);
                if (block.ID != 0)
                {
                    ShapeAABB bound2 = new ShapeAABB(block.Block.GetBlockBoundBox((int)v.X, (int)v.Y));
                    if (bound2.Intersects(bound))
                    {
                        GameServer.SetBlock((int)v.X, (int)v.Y, 0);
                    }
                }
            }

            foreach (NetworkPlayer player in GameServer.NetworkPlayers)
            {
                if (player == this) continue;
                if (player.BoundBox.Intersects(bound) || player.BoundBox.Contains(bound) || bound.Contains(player.BoundBox))
                {
                    GameServer.SendMessageToAll(PlayerName + " hit " + player.PlayerName);
                    player.PlayerHealth--;
                    player.EntityVelocity.Y = -4;
                    player.EntityVelocity.X = FacingLeft ? -5 : 5;
                }
            }
        }

        private void AttackArrows()
        {
            int nextslot = GameServer.GetFreeProjectileSlot();
            if (nextslot != -1)
            {
                short angle = (short)PlayerAimAngle.RToD();

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

        private void PlayerCollisions()
        {
            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p == this) continue;

                AABBCollisionResult collide = p.BoundBox.CollideAABB(BoundBox);
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
                BlockData blockData = GameServer.GetBlockAt(newTile.X, newTile.Y);

                if (blockData.ID == 0) continue;

                Block block = blockData.Block;
                bool walkThrough = block.GetBlockWalkThrough();

                //A wall
                Rectangle blockBB = block.GetBlockBoundBox((int)newTile.X, (int)newTile.Y);

                ShapeAABB thisAABB = BoundBox;
                ShapeAABB blockAABB = new ShapeAABB(blockBB);
                AABBCollisionResult collide = thisAABB.CollideAABB(blockAABB);

                if (!collide.IsIntersecting || collide.X == 0 || collide.Y == 0) continue;

                if (!walkThrough)
                {
                    int side = 0;



                    if (collide.Projection.Y > 0)
                        side = 2;
                    if (collide.Projection.Y < 0)
                        side = 0;
                    if (collide.Projection.X > 0)
                        side = 1;
                    if (collide.Projection.X < 0)
                        side = 3;

                    if (collide.Projection.Y < 0)
                    {
                        TimeFalling = 0;
                        Falling = false;
                    }

                    EntityVelocity *= collide.MultProjection;
                    EntityPosition += collide.Projection;
                    block.OnBlockTouched((int)newTile.X, (int)newTile.Y, side, this);
                }
                /*else if (collide.XSmaller)
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
                }*/
            }
        }

        public override void EntityMovement()
        {
            if (BoundBox.Left < 0) EntityPosition.X = BoundBox.Width / 2 + 1;
            if (BoundBox.Top < 0) EntityPosition.Y = BoundBox.Height / 2 + 1;
            if (BoundBox.Right > GameServer.BlockSize * GameServer.WorldSizeX) EntityPosition.X = GameServer.BlockSize * GameServer.WorldSizeX - (BoundBox.Width / 2);
            if (BoundBox.Bottom > GameServer.BlockSize * GameServer.WorldSizeY) EntityPosition.Y = GameServer.BlockSize * GameServer.WorldSizeY - (BoundBox.Height / 2);

            //Didn't want to make a new BoundBox so this'll do. Gets the tiles the player will be in with his velocity.

            EntityPosition += EntityVelocity;

            if (EntityVelocity != Vector2.Zero)
                PlayerCollisions();

            BlockCollisions();



            if (EntityVelocity.Y < 6)
                EntityVelocity.Y += 1; // Gravity! This is a really nice side effect: The code for not allowing the player to go through a block downwards already exists, so I just need to add this one line to add gravity!

            EntityVelocity = EntityVelocity.ApproachZeroX();

            if (EntityVelocity.Y != 1)
                Falling = true;
        }

        

        public void SendEquippedItemUpdate()
        {
            ServerItem i = Inventory.GetPlayerItemInHand();
            byte itemID = 0;
            if (i != null)
                itemID = i.GetItemID();

            Packet7SCPlayerCurItemChanged packet7 = new Packet7SCPlayerCurItemChanged(PlayerID, itemID);
            GameServer.ServerNetworkManager.SendPacket(packet7);
        }
        
        

        public void SetPlayerEquippedSlot(int slot)
        {
            int oldSlot = Inventory.PlayerInventorySelected;
            Inventory.PlayerInventorySelected = slot;
            SendEquippedItemUpdate();
        }


        public void DropItem()
        {
            ItemStack inHand = Inventory.GetPlayerItemStackInHand();
            if (inHand.ItemID == 0)
                return;
            Vector2 velocity = new Vector2(5, -2);
            int xPos = BoundBox.Right;
            if (FacingLeft)
            {
                velocity.X *= -1;
                xPos = BoundBox.Left;
            }
            GameServer.DropItem(inHand, new Vector2(xPos, EntityPosition.Y), velocity, this);
            Inventory.RemoveItemAt(Inventory.PlayerInventorySelected);
        }

        
    }
}
