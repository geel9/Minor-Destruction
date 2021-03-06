﻿using System.Collections.Generic;
using GeeUI.Managers;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
using MiningGame.Code.Managers;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;

namespace MiningGame.Code.Entities
{
    public class EntityMoveable : Entity
    {
        private const int EntityWidth = 16, EntityHeight = 16;

        public Vector2 EntityVelocity = Vector2.Zero;

        internal int TimeFalling;
        internal bool Falling;

        internal List<Vector2> RectangleHitsTiles(ShapeAABB rect)
        {
            List<Vector2> ret = new List<Vector2>();
            //PToV simply turns a Point into a Vector2
            Vector2 rectCenter = rect.Center;

            Vector2 topLeftHit = GameWorld.AbsoluteToTile(new Vector2(rect.Left, rect.Top));
            topLeftHit.X = (topLeftHit.X > 0) ? topLeftHit.X - 1 : topLeftHit.X;
            topLeftHit.Y = (topLeftHit.Y > 0) ? topLeftHit.Y - 1 : topLeftHit.Y;

            Vector2 bottomRightHit = GameWorld.AbsoluteToTile(new Vector2(rect.Right, rect.Bottom));
            bottomRightHit.X = (bottomRightHit.X < GameWorld.WorldSizeX) ? bottomRightHit.X + 1 : bottomRightHit.X;
            bottomRightHit.Y = (bottomRightHit.Y < GameWorld.WorldSizeY) ? bottomRightHit.Y + 1 : bottomRightHit.Y;
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
            return new Vector2(BoundBox.Center.X / GameWorld.BlockSize, (BoundBox.Bottom - 1) / GameWorld.BlockSize);
        }

        public virtual void EntityMovement()
        {
            if (BoundBox.Left < 0) EntityPosition.X = BoundBox.Width / 2 + 1;
            if (BoundBox.Top < 0) EntityPosition.Y = BoundBox.Height / 2 + 1;
            if (BoundBox.Right > GameWorld.BlockSize * GameWorld.WorldSizeX) EntityPosition.X = GameWorld.BlockSize * GameWorld.WorldSizeX - (BoundBox.Width / 2);
            if (BoundBox.Bottom > GameWorld.BlockSize * GameWorld.WorldSizeY) EntityPosition.Y = GameWorld.BlockSize * GameWorld.WorldSizeY - (BoundBox.Height / 2);
            List<Vector2> tilesHitting = RectangleHitsTiles(BoundBox);

            //Didn't want to make a new BoundBox so this'll do. Gets the tiles the player will be in with his velocity.
            EntityPosition += EntityVelocity;
            List<Vector2> newTilesHitting = RectangleHitsTiles(BoundBox);

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

                ShapeAABB thisAABB = BoundBox;
                ShapeAABB blockAABB = new ShapeAABB(blockBB);
                AABBCollisionResult collide = thisAABB.CollideAABB(blockAABB);

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
            //EntityPosition += EntityVelocity;

            //Ropes
            short blockID2 = GameWorld.GetBlockAt(GetEntityTile().X, GetEntityTile().Y).ID;

            if (EntityVelocity.Y < 6 && blockID2 != 6) EntityVelocity.Y += 1; // Gravity! This is a really nice side effect: The code for not allowing the player to go through a block downwards already exists, so I just need to add this one line to add gravity!

            if (blockID2 == 6)
            {
                if (EntityVelocity.Y > 0) EntityVelocity.Y--;
                if (EntityVelocity.Y < 0) EntityVelocity.Y++;
            }

            if (EntityVelocity.X < 0) EntityVelocity.X += 1;
            if (EntityVelocity.X > 0) EntityVelocity.X -= 1;

            if (EntityVelocity.Y != 1) Falling = true;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            List<Vector2> hits = RectangleHitsTiles(BoundBox);
            foreach (Vector2 vec in hits)
            {
                //DrawManager.DrawOutline(new Vector2(vec.X * GameWorld.BlockSize + (GameWorld.BlockSize / 2), vec.Y * GameWorld.BlockSize + (GameWorld.BlockSize / 2)) - CameraManager.cameraPosition, GameWorld.BlockSize, GameWorld.BlockSize, Color.Red, sb);
            }

            base.Draw(sb);
        }

        public override void Update(GameTime time, bool serverContext = false)
        {
            if (Falling) TimeFalling++;
            base.Update(time);
        }
    }
}
