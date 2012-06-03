using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
using MiningGame.Code.Managers;
using MiningGame.ExtensionMethods;
using MiningGameServer;
using MiningGameServer.Structs;

namespace MiningGame.Code.Entities
{
    public class ProjectileArrow : EntityProjectile
    {
        public byte PlayerOwner = 0;

        public override byte GetProjectileType()
        {
            return 1;
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
                BlockData blockData = GameWorld.GetBlockAt(newTile.X, newTile.Y);

                if (blockData.ID == 0) continue;

                Block block = blockData.Block;
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
                        break;
                    }
                }
            }
        }


        public override void EntityMovement()
        {
            if (ShouldDestroy) return;
            if (BoundBox.Left < 0 || BoundBox.Top < 0 || BoundBox.Right > GameWorld.BlockWidth * GameWorld.WorldSizeX || BoundBox.Bottom > GameWorld.BlockHeight * GameWorld.WorldSizeY)
            {
                ShouldDestroy = true;
                return;
            }
            //BlockCollision();

            EntityPosition += EntityVelocity;

            //if (EntityVelocity.Y < 6)
                EntityVelocity.Y += EffectOfGravity;


            if (EntityVelocity.X < 0) EntityVelocity.X += AirFriction;
            if (EntityVelocity.X > 0) EntityVelocity.X -= AirFriction;
        }


        public ProjectileArrow(Vector2 Position, short angle, byte owner, int strength = 10)
            : base()
        {
            float angle2 = (float) angle.DToR();
            EntityPosition = Position;
            float strengthX = (float)(strength * Math.Cos(angle2));
            float strengthY = (float)(strength * Math.Sin(angle2));

            EntityVelocity = new Vector2(strengthX, strengthY);
            LastPosition = Position - EntityVelocity;

            EffectOfGravity = 0.34f;

            SpriteTexture = AssetManager.GetTexture("arrow");
            Alpha = 255;
            PlayerOwner = owner;
        }
    }
}
