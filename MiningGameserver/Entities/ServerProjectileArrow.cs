using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer;
using MiningGameServer.Blocks;
using MiningGameServer.Structs;

namespace MiningGameserver.Entities
{
    class ServerProjectileArrow : ServerProjectile
    {
        public byte PlayerOwner = 0;

        public override byte GetProjectileType()
        {
            return 1;
        }

        public override AABB BoundBox
        {
            get
            {
                return new AABB(EntityPosition.X, EntityPosition.Y, 4, 16, Rotation);
            }
        }

        public void BlockCollision()
        {
            List<Vector2> tilesHitting = RectangleHitsTiles(BoundBox);

            //Didn't want to make a new BoundBox so this'll do. Gets the tiles the player will be in with his velocity.
            EntityPosition += EntityVelocity;
            Vector2 newEntityPosition = EntityPosition;
            AABB newRectTest = BoundBox;
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
                byte blockID = GameServer.GetBlockIDAt(newTile.X, newTile.Y);

                if (blockID == 0) continue;

                Block block = Block.GetBlock(blockID);
                bool walkThrough = block.GetBlockWalkThrough();

                //A wall
                Rectangle blockBB = block.GetBlockBoundBox((int)newTile.X, (int)newTile.Y);

                AABB thisAABB = newRectTest;
                AABB blockAABB = new AABB(blockBB);
                AABBResult collide = thisAABB.AxisCollide(blockAABB);
                if (!collide.IsIntersecting) continue;


                if (collide.XSmaller)
                {
                    bool right = (collide.X < 0);
                    if (!walkThrough)
                    {
                        EntityVelocity.X = 0;
                        EntityPosition.X = (newEntityPosition.X + collide.X);
                        ShouldDestroy = true;
                        GameServer.SetBlock((int)newTile.X, (int)newTile.Y, 0);
                        break;
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
                        GameServer.SetBlock((int)newTile.X, (int)newTile.Y, 0);
                        break;
                    }
                }
            }
        }

        public void PlayerCollision()
        {
            EntityPosition += EntityVelocity;
            Vector2 newEntityPosition = EntityPosition;
            AABB newRectTest = BoundBox;
            EntityPosition -= EntityVelocity;

            NetworkPlayer playerOwner = new NetworkPlayer(PlayerOwner, null, Vector2.Zero, "PLAYER");
            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p.PlayerID == PlayerOwner)
                {
                    playerOwner = p;
                    break;
                }
            }

            foreach (NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p.PlayerID == PlayerOwner) continue;
                if (p.BoundBox.Intersects(newRectTest))
                {
                    ShouldDestroy = true;
                    p.PlayerHealth--;
                    if (p.PlayerHealth <= 0)
                    {
                        p.PlayerHealth = 5;
                        p.EntityPosition = new Vector2(50, 50);
                        GameServer.SendMessageToAll(playerOwner.PlayerName + " killed " + p.PlayerName + ".");
                    }
                    else
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


        public ServerProjectileArrow(Vector2 Position, float angle, byte owner, int strength = 10)
            : base()
        {
            EntityPosition = Position;
            float strengthX = (float)(strength * Math.Cos(angle));
            float strengthY = (float)(strength * Math.Sin(angle));

            EntityVelocity = new Vector2(strengthX, strengthY);
            LastPosition = Position - EntityVelocity;

            EffectOfGravity = 0.34f;
            PlayerOwner = owner;
        }
    }
}

