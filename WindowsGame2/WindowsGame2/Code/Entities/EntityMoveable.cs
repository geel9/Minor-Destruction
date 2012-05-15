using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
using MiningGame.Code.Structs;
using YogUILibrary.Managers;

namespace MiningGame.Code.Entities
{
    public class EntityMoveable : Entity
    {
        private const int entityWidth = 16, entityHeight = 16;

        public Vector2 entityVelocity = Vector2.Zero;

        internal int timeFalling = 0;
        internal bool falling = false;

        internal List<Vector2> rectangleHitsTiles(Rectangle rect)
        {
            List<Vector2> ret = new List<Vector2>();
            //PToV simply turns a Point into a Vector2
            Vector2 rectCenter = ConversionManager.PToV(rect.Center);

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

        public Vector2 getEntityTile()
        {
            return new Vector2(BoundBox.Center.X / GameWorld.BlockWidth, (BoundBox.Bottom - 1) / GameWorld.BlockHeight);
        }

        public void entityMovement()
        {
            if (BoundBox.Left < 0) entityPosition.X = BoundBox.Width / 2 + 1;
            if (BoundBox.Top < 0) entityPosition.Y = BoundBox.Height / 2 + 1;
            if (BoundBox.Right > GameWorld.BlockWidth * GameWorld.WorldSizeX) entityPosition.X = GameWorld.BlockWidth * GameWorld.WorldSizeX - (BoundBox.Width / 2);
            if (BoundBox.Bottom > GameWorld.BlockHeight * GameWorld.WorldSizeY) entityPosition.Y = GameWorld.BlockHeight * GameWorld.WorldSizeY - (BoundBox.Height / 2);
            List<Vector2> tilesHitting = rectangleHitsTiles(BoundBox);

            //Didn't want to make a new BoundBox so this'll do. Gets the tiles the player will be in with his velocity.
            entityPosition += entityVelocity;
            Vector2 newEntityPosition = entityPosition;
            Rectangle newRectTest = BoundBox;
            List<Vector2> newTilesHitting = rectangleHitsTiles(newRectTest);
            entityPosition -= entityVelocity;

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

                Block block = Block.getBlock(blockID);
                bool walkThrough = block.getBlockWalkThrough();

                //A wall
                Rectangle blockBB = block.getBlockBoundBox()((int)newTile.X, (int)newTile.Y);

                AxisAlignedBoundBox thisAABB = new AxisAlignedBoundBox(newRectTest);
                AxisAlignedBoundBox blockAABB = new AxisAlignedBoundBox(blockBB);
                Vector2 collide = thisAABB.axisCollide(blockAABB);
                if (collide.X != 0)
                {
                    bool right = (collide.X < 0);
                    if (!walkThrough)
                    {
                        entityVelocity.X = 0;
                        entityPosition.X = (newEntityPosition.X + collide.X);
                    }
                    block.getBlockOnTouched()((int)newTile.X, (int)newTile.Y, right ? 3 : 1, this);
                }
                if (collide.Y != 0 && newTile.Y >= highestY)
                {
                    bool up = (collide.Y > 0);

                    if (!walkThrough)
                    {
                        entityVelocity.Y = 0;
                        entityPosition.Y = (newEntityPosition.Y + collide.Y);
                        if (!up)
                        {
                            timeFalling = 0;
                            falling = false;
                        }
                    }
                    block.getBlockOnTouched()((int)newTile.X, (int)newTile.Y, up ? 2 : 0, this);
                }
            }
            entityPosition += entityVelocity;

            //Ropes
            byte blockID2 = GameWorld.GetBlockIDAt(getEntityTile().X, getEntityTile().Y);

            if (entityVelocity.Y < 6 && blockID2 != 6) entityVelocity.Y += 1; // Gravity! This is a really nice side effect: The code for not allowing the player to go through a block downwards already exists, so I just need to add this one line to add gravity!

            if (blockID2 == 6)
            {
                if (entityVelocity.Y > 0) entityVelocity.Y--;
                if (entityVelocity.Y < 0) entityVelocity.Y++;
            }

            if (entityVelocity.X < 0) entityVelocity.X += 1;
            if (entityVelocity.X > 0) entityVelocity.X -= 1;

            if (entityVelocity.Y != 1) falling = true;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            List<Vector2> hits = rectangleHitsTiles(BoundBox);
            foreach (Vector2 vec in hits)
            {
                DrawManager.Draw_Outline(new Vector2(vec.X * GameWorld.BlockWidth + (GameWorld.BlockWidth / 2), vec.Y * GameWorld.BlockHeight + (GameWorld.BlockHeight / 2)) - CameraManager.cameraPosition, GameWorld.BlockWidth, GameWorld.BlockHeight, Color.Red, sb);
            }

            base.Draw(sb);
        }

        public override void Update(GameTime time)
        {
            if (falling) timeFalling++;
            base.Update(time);
        }
    }
}
