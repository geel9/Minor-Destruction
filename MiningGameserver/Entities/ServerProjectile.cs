﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Blocks;
using MiningGameServer.ExtensionMethods;
using MiningGameServer.Structs;
using MiningGameServer;

namespace MiningGameserver.Entities
{
    public class ServerProjectile : ServerEntity
    {
        public Vector2 EntityVelocity = Vector2.Zero;
        public float EffectOfGravity = 0.5f;
        public float AirFriction;

        public byte ProjectileID;

        public bool ShouldDestroy = false;


        public new float Rotation
        {
            get
            {
                return base.Rotation;
            }
            set
            {
                if (ShouldDestroy) return;
                base.Rotation = value;
            }
        }

        internal Vector2 LastPosition = Vector2.Zero;

        internal List<Vector2> RectangleHitsTiles(AABB rect)
        {
            List<Vector2> ret = new List<Vector2>();
            //PToV simply turns a Point into a Vector2
            Vector2 rectCenter = rect.Center;

            Vector2 topLeftHit = GameServer.AbsoluteToTile(new Vector2(rect.Left, rect.Top));
            Vector2 bottomRightHit = GameServer.AbsoluteToTile(new Vector2(rect.Right - 2, rect.Bottom - 2));
            //From the top of the rectangle to the bottom, find the tiles that the rectangle intersects and add them.

            for (int y = (int)topLeftHit.Y; y <= bottomRightHit.Y; y++)
            {
                for (int x = (int)topLeftHit.X; x <= bottomRightHit.X; x++)
                {
                    if (!ret.Contains(new Vector2(x, y))) ret.Add(new Vector2(x, y));
                }
            }
            return ret;
        }

        public Vector2 GetEntityTile()
        {
            return new Vector2(BoundBox.Center.X / GameServer.BlockWidth, (BoundBox.Bottom - 1) / GameServer.BlockHeight);
        }

        public virtual void EntityMovement()
        {
            if (ShouldDestroy) return;
            if (BoundBox.Left < 0 || BoundBox.Top < 0 || BoundBox.Right > GameServer.BlockWidth * GameServer.WorldSizeX || BoundBox.Bottom > GameServer.BlockHeight * GameServer.WorldSizeY)
            {
                ShouldDestroy = true;
                return;
            }
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

            EntityPosition += EntityVelocity;

            if (EntityVelocity.Y < 6)
                EntityVelocity.Y += EffectOfGravity;


            if (EntityVelocity.X < 0) EntityVelocity.X += AirFriction;
            if (EntityVelocity.X > 0) EntityVelocity.X -= AirFriction;
        }

        public override void Update()
        {

            EntityMovement();
            Rotation = (float)((float)Math.Atan2(EntityPosition.Y - LastPosition.Y, EntityPosition.X - LastPosition.X) +
                                90f.DToR());
            LastPosition = EntityPosition;
        }

        public virtual byte GetProjectileType()
        {
            return 0;
        }
    }
}