using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer;
using MiningGameServer.Blocks;
using MiningGameServer.ExtensionMethods;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;

namespace MiningGameServer.Entities
{
    class ServerProjectileArrow : ServerProjectile
    {
        public byte PlayerOwner = 0;

        public override byte GetProjectileType()
        {
            return 1;
        }

        public override ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB(EntityPosition.X, EntityPosition.Y, 4, 16, Rotation);
            }
        }

        public void BlockCollision()
        {
            List<Vector2> tilesHitting = RectangleHitsTiles(BoundBox);

            //Didn't want to make a new BoundBox so this'll do. Gets the tiles the player will be in with his velocity.
            EntityPosition += EntityVelocity;
            Vector2 newEntityPosition = EntityPosition;
            ShapeAABB newRectTest = BoundBox;
            List<Vector2> newTilesHitting = RectangleHitsTiles(newRectTest);
            EntityPosition -= EntityVelocity;

            //The amount of tiles the player will be entering with his new position.
            List<Vector2> newTiles = newTilesHitting;

            int highestY = 0;
            for (int i = 0; i < newTiles.Count; i++)
            {
                if (newTiles[i].Y > highestY) highestY = (int)newTiles[i].Y;
            }

            foreach (Vector2 newTile in newTiles)
            {

                BlockData blockData = GameServer.GetBlockAt(newTile.X, newTile.Y);

                if (blockData.ID == 0) continue;

                Block block = blockData.Block;
                bool walkThrough = block.GetBlockWalkThrough();

                //A wall
                Rectangle blockBB = block.GetBlockBoundBox((int)newTile.X, (int)newTile.Y);

                ShapeAABB thisAABB = newRectTest;
                ShapeAABB blockAABB = new ShapeAABB(blockBB);
                AABBCollisionResult collide = thisAABB.CollideAABB(blockAABB);
                if (!collide.IsIntersecting) continue;


                if (collide.XSmaller)
                {
                    bool right = (collide.X < 0);
                    if (!walkThrough)
                    {
                        EntityVelocity.X = 0;
                        EntityPosition.X = (newEntityPosition.X + collide.X);
                        ShouldDestroy = true;
                    }
                }
                else
                {
                    bool up = (collide.Y > 0);

                    if (!walkThrough)
                    {
                        EntityVelocity.Y = 0;
                        EntityPosition.Y = (newEntityPosition.Y + collide.Y);
                        ShouldDestroy = true;   
                    }
                }

                if(ShouldDestroy)
                {
                    byte dropID = blockData.Block.GetItemDrop((int)newTile.X, (int)newTile.Y);
                    int num = blockData.Block.GetItemDropNum((int)newTile.X, (int)newTile.Y);

                    GameServer.SetBlock((int)newTile.X, (int)newTile.Y, 0);

                    if (dropID != 0 && num > 0)
                    {
                        ItemStack stack = new ItemStack(num, dropID);
                        Vector2 pos = newTile * GameServer.BlockHeight;
                        pos += new Vector2(GameServer.BlockHeight / 2);
                        GameServer.DropItem(stack, pos);
                    }
                    break;
                }
            }
        }

        public void PlayerCollision()
        {
            EntityPosition += EntityVelocity;
            Vector2 newEntityPosition = EntityPosition;
            ShapeAABB newRectTest = BoundBox;
            EntityPosition -= EntityVelocity;

            NetworkPlayer playerOwner = null;
            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p.PlayerID == PlayerOwner)
                {
                    playerOwner = p;
                    break;
                }
            }
            if(playerOwner == null)
            {
                ShouldDestroy = true;
                return;
            }
            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p.PlayerID == PlayerOwner) continue;
                if (p.BoundBox.Intersects(newRectTest))
                {
                    ShouldDestroy = true;
                    p.EntityVelocity += EntityVelocity / 5;
                    p.PlayerHealth--;
                    GameServer.SendMessageToAll(playerOwner.PlayerName + " hit " + p.PlayerName + ". Their new health: " + p.PlayerHealth);
                    break;
                }
            }
        }

        public override void EntityMovement()
        {
            if (ShouldDestroy) return;
            if (BoundBox.Left < 0 || BoundBox.Top < 0 || BoundBox.Right > GameServer.BlockWidth * GameServer.WorldSizeX || BoundBox.Bottom > GameServer.BlockHeight * GameServer.WorldSizeY)
            {
                ShouldDestroy = true;
                return;
            }
            PlayerCollision();
            if (!ShouldDestroy)
                BlockCollision();

            EntityPosition += EntityVelocity;

            //if (EntityVelocity.Y < 6)
            EntityVelocity.Y += EffectOfGravity;


            if (EntityVelocity.X < 0) EntityVelocity.X += AirFriction;
            if (EntityVelocity.X > 0) EntityVelocity.X -= AirFriction;
        }


        public ServerProjectileArrow(Vector2 Position, short angle, byte owner, int strength = 10)
            : base()
        {
            float angle2 = angle.DToR();
            EntityPosition = Position;
            float strengthX = (float)(strength * Math.Cos(angle2));
            float strengthY = (float)(strength * Math.Sin(angle2));

            EntityVelocity = new Vector2(strengthX, strengthY);
            LastPosition = Position - EntityVelocity;

            EffectOfGravity = 0.34f;
            PlayerOwner = owner;
        }
    }
}

