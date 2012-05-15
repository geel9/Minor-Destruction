using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Structs;
using YogUILibrary.Managers;
namespace MiningGame.Code.Entities
{
    public class Entity : Animateable, UpdatableAndDrawable
    {
        protected bool paused
        {
            get
            {
                return Main.PauseManager.Paused;
            }
        }
        public Vector2 entityPosition = new Vector2();
        public byte alpha;
        public float rotation;
        public Texture2D SpriteTexture;
        public float scale;
        public int layer;
        public int entityID;

        public static int entIndex = 0;

        public virtual Rectangle BoundBox
        {
            get
            {
                if (SpriteTexture != null)
                    return new Rectangle(
                        (int)entityPosition.X - (int)(SpriteTexture.Width * scale) / 2,
                        (int)entityPosition.Y - (int)(SpriteTexture.Width * scale) / 2,
                        (int)(SpriteTexture.Width * scale),
                        (int)(SpriteTexture.Height * scale));
                else
                    return new Rectangle(1, 1, 1, 1);
            }
        }

        public AxisAlignedBoundBox AABoundBox
        {
            get
            {
                return new AxisAlignedBoundBox(BoundBox);
            }
        }

        public Entity()
        {

        }

        public bool hitTest(Entity e)
        {
            if (this.BoundBox.Intersects(e.BoundBox))
                return true;
            if (this.BoundBox.Contains(e.BoundBox))
                return true;
            if (e.BoundBox.Contains(this.BoundBox))
                return true;
            return false;
        }

        public void addToList()
        {
            entityID = ++entIndex;
            addToUpdateList();
            addToDrawList();
        }

        public void removeFromList()
        {
            Main.drawables.Remove(this);
            Main.updatables.Remove(this);
        }

        public virtual void Update(GameTime time)
        {
        }

        public void RotateToPoint(Point point)
        {
            RotateToPoint(ConversionManager.PToV(point));
        }

        public void RotateToPoint(Vector2 point)
        {
            Vector2 dist = entityPosition - point;
            float rotation = (float)Math.Atan2(dist.Y, dist.X);
            this.rotation = rotation;
        }

        public void RotateToPoint(Vector2 point, int maxDegrees)
        {
            maxDegrees = Math.Abs(maxDegrees);
            Vector2 dist = entityPosition - point;
            //   dist = point - point;
            float rotation = (float)Math.Atan2(dist.Y, dist.X);
            float rotDeg = (float)ConversionManager.RadianToDegrees(rotation);
            rotDeg = (float)ConversionManager.circleTo360((double)rotDeg);
            float curRotDeg = (float)ConversionManager.circleTo360(ConversionManager.RadianToDegrees((double)this.rotation));
            float rotDiff = curRotDeg - rotDeg;
            ConsoleManager.setVariableValue("window_title", "Rot: " + rotDeg);
            //if (rotDeg)
            //{
            //  rotDiff = this.rotation - rotation;
            //}
            float maxRadians = (float)ConversionManager.DegreeToRadians(maxDegrees);
            rotDiff = MathHelper.Clamp(rotDiff, -maxRadians, maxRadians);
            //     this.rotation += rotDiff;
            this.rotation = (float)ConversionManager.DegreeToRadians(40);
        }

        public void MoveTowardsPoint(Point point, float speed)
        {
            Vector2 realPoint = ConversionManager.PToV(point);
            MoveTowardsPoint(realPoint, speed);
        }

        public void MoveTowardsPoint(Vector2 point, float speed)
        {
            Vector2 dist = point - entityPosition;
            float Radians = (float)Math.Atan2(dist.Y, dist.X);
            float X = speed * (float)Math.Cos(Radians);
            float Y = speed * (float)Math.Sin(Radians);
            entityPosition += new Vector2(X, Y);
        }

        public virtual void Draw(SpriteBatch sb)
        {

            Color white = Color.White;
            white.A = alpha;
            if (ConsoleManager.getVariableBool("draw_hitboxes"))
            {
                DrawManager.Draw_Outline(entityPosition - (CameraManager.cameraPosition), BoundBox.Width, BoundBox.Height, Color.Yellow, sb);
            }
           // sb.DrawString(AssetManager.GetFont("Console"), entityID.ToString(), entityPosition - new Vector2(0, 10) - CameraManager.cameraPosition, Color.White);
            if (SpriteTexture != null)
                sb.Draw(SpriteTexture, entityPosition - (CameraManager.cameraPosition), new Rectangle(0, 0, SpriteTexture.Width, SpriteTexture.Height), white, rotation, new Vector2(SpriteTexture.Width / 2, SpriteTexture.Height / 2), scale * CameraManager.cameraZoom, SpriteEffects.None, layer);
        }

        public void addToUpdateList()
        {
            if (!Main.updatables.Contains(this)) Main.updatables.Add(this);
            else
                throw new Exception("Item already in update list");
        }

        public void addToDrawList()
        {
            if (!Main.drawables.Contains(this)) Main.drawables.Add(this);
            else
                throw new Exception("Item already in draw list: " + this);
        }

        public void removeFromUpdateList()
        {
            Main.updatables.Remove(this);
        }

        public void removeFromDrawList()
        {
            Main.drawables.Remove(this);
        }

        public bool inCamera()
        {
            return CameraManager.inCamera(entityPosition, BoundBox);
        }
    }
}
