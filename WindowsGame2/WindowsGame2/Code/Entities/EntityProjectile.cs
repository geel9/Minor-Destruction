using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
using MiningGame.Code.Managers;
using MiningGame.Code.Structs;
using YogUILibrary.Managers;

namespace MiningGame.Code.Entities
{
    public class EntityProjectile : Entity
    {
        public Vector2 EntityVelocity = Vector2.Zero;
        public float EffectOfGravity = 0.5f;
        public float AirFriction;

        public byte ProjectileID;

        public bool ShouldDestroy = false;

        public EntityProjectile()
        {
        }

        internal Vector2 LastPosition = Vector2.Zero;

        internal List<Vector2> RectangleHitsTiles(AABB rect)
        {
            List<Vector2> ret = new List<Vector2>();
            //PToV simply turns a Point into a Vector2
            Vector2 rectCenter = rect.Center;

            Vector2 topLeftHit = GameWorld.AbsoluteToTile(new Vector2(rect.Left, rect.Top));
            Vector2 bottomRightHit = GameWorld.AbsoluteToTile(new Vector2(rect.Right - 2, rect.Bottom - 2));
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
            return new Vector2(BoundBox.Center.X / GameWorld.BlockWidth, (BoundBox.Bottom - 1) / GameWorld.BlockHeight);
        }

        public void EntityMovement()
        {
            if (ShouldDestroy) return;
            if (BoundBox.Left < 0 || BoundBox.Top < 0 || BoundBox.Right > GameWorld.BlockWidth * GameWorld.WorldSizeX || BoundBox.Bottom > GameWorld.BlockHeight * GameWorld.WorldSizeY)
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
                byte blockID = GameWorld.GetBlockIDAt(newTile.X, newTile.Y);

                if (blockID == 0) continue;

                Block block = Block.GetBlock(blockID);
                bool walkThrough = block.GetBlockWalkThrough();

                //A wall
                Rectangle blockBB = block.GetBlockBoundBox((int)newTile.X, (int)newTile.Y);

                AABB thisAABB = newRectTest;
                AABB blockAABB = new AABB(blockBB);
                Vector2 collide = thisAABB.AxisCollide(blockAABB);
                if (collide.X != 0)
                {
                    bool right = (collide.X < 0);
                    if (!walkThrough)
                    {
                        EntityVelocity.X = 0;
                        EntityPosition.X = (newEntityPosition.X + collide.X);
                        ShouldDestroy = true;
                    }
                }
                if (collide.Y != 0 && newTile.Y >= highestY)
                {
                    bool up = (collide.Y > 0);

                    if (!walkThrough)
                    {
                        EntityVelocity.Y = 0;
                        EntityPosition.Y = (newEntityPosition.Y + collide.Y);
                        ShouldDestroy = true;
                    }
                }
            }

            EntityPosition += EntityVelocity;

            if (EntityVelocity.Y < 6)
                EntityVelocity.Y += EffectOfGravity;


            if (EntityVelocity.X < 0) EntityVelocity.X += AirFriction;
            if (EntityVelocity.X > 0) EntityVelocity.X -= AirFriction;
        }

        public new void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            List<Vector2> hits = RectangleHitsTiles(BoundBox);
            foreach (Vector2 vec in hits)
            {
                //DrawManager.Draw_Outline(new Vector2(vec.X * GameWorld.BlockWidth + (GameWorld.BlockWidth / 2), vec.Y * GameWorld.BlockHeight + (GameWorld.BlockHeight / 2)) - CameraManager.cameraPosition, GameWorld.BlockWidth, GameWorld.BlockHeight, Color.Red, sb);
            }
            Rotation = (float)((float)Math.Atan2(EntityPosition.Y - LastPosition.Y, EntityPosition.X - LastPosition.X) +
                                ConversionManager.DegreeToRadians(90));

            LastPosition = EntityPosition;

            base.Draw(sb);
        }

        public override void Update(GameTime time)
        {
            EntityMovement();
        }

        public virtual byte GetProjectileType()
        {
            return 0;
        }
    }
}
