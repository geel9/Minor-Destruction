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
        protected bool Paused
        {
            get
            {
                return Main.PauseManager.Paused;
            }
        }
        public Vector2 EntityPosition = new Vector2();
        public byte Alpha;
        public float Rotation { get; set; }
        public Texture2D SpriteTexture;
        public float Scale = 1;
        public int Layer;
        public int EntityID;

        public static int EntIndex = 0;

        public virtual AABB BoundBox
        {
            get
            {
                if (SpriteTexture != null)
                    return new AABB(
                        (int)EntityPosition.X - (SpriteTexture.Width / 2),
                        (int)EntityPosition.Y - (SpriteTexture.Height / 2),
                        (int)(SpriteTexture.Width * Scale),
                        (int)(SpriteTexture.Height * Scale), Rotation);

                    return new AABB(1, 1, 1, 1, Rotation);
            }
        }

        public Entity()
        {

        }

        public bool HitTest(Entity e)
        {
            if (this.BoundBox.Intersects(e.BoundBox))
                return true;
            if (this.BoundBox.Contains(e.BoundBox))
                return true;
            return e.BoundBox.Contains(this.BoundBox);
        }

        public void addToList()
        {
            EntityID = ++EntIndex;
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

        public virtual void Update(GameTime time, bool serverContext = false)
        {
        }

        public void RotateToPoint(Point point)
        {
            RotateToPoint(ConversionManager.PToV(point));
        }

        public void RotateToPoint(Vector2 point)
        {
            Vector2 dist = EntityPosition - point;
            float rotation = (float)Math.Atan2(dist.Y, dist.X);
            this.Rotation = rotation;
        }

        public void RotateToPoint(Vector2 point, int maxDegrees)
        {
            maxDegrees = Math.Abs(maxDegrees);
            Vector2 dist = EntityPosition - point;
            //   dist = point - point;
            float rotation = (float)Math.Atan2(dist.Y, dist.X);
            float rotDeg = (float)ConversionManager.RadianToDegrees(rotation);
            rotDeg = (float)ConversionManager.circleTo360((double)rotDeg);
            float curRotDeg = (float)ConversionManager.circleTo360(ConversionManager.RadianToDegrees((double)this.Rotation));
            float rotDiff = curRotDeg - rotDeg;
            ConsoleManager.setVariableValue("window_title", "Rot: " + rotDeg);
            //if (rotDeg)
            //{
            //  rotDiff = this.rotation - rotation;
            //}
            float maxRadians = (float)ConversionManager.DegreeToRadians(maxDegrees);
            rotDiff = MathHelper.Clamp(rotDiff, -maxRadians, maxRadians);
            //     this.rotation += rotDiff;
            this.Rotation = (float)ConversionManager.DegreeToRadians(40);
        }

        public void MoveTowardsPoint(Point point, float speed)
        {
            Vector2 realPoint = ConversionManager.PToV(point);
            MoveTowardsPoint(realPoint, speed);
        }

        public void MoveTowardsPoint(Vector2 point, float speed)
        {
            Vector2 dist = point - EntityPosition;
            float Radians = (float)Math.Atan2(dist.Y, dist.X);
            float X = speed * (float)Math.Cos(Radians);
            float Y = speed * (float)Math.Sin(Radians);
            EntityPosition += new Vector2(X, Y);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            Color white = Color.White;
            white.A = Alpha;
            if (ConsoleManager.getVariableBool("draw_hitboxes"))
            {
                DrawManager.Draw_Outline(EntityPosition - (CameraManager.cameraPosition), BoundBox.Width, BoundBox.Height, Color.Yellow, sb);
            }
           // sb.DrawString(AssetManager.GetFont("Console"), entityID.ToString(), entityPosition - new Vector2(0, 10) - CameraManager.cameraPosition, Color.White);
            if (SpriteTexture != null)
                sb.Draw(SpriteTexture, EntityPosition - (CameraManager.cameraPosition), new Rectangle(0, 0, SpriteTexture.Width, SpriteTexture.Height), white, Rotation, new Vector2(SpriteTexture.Width / 2, SpriteTexture.Height / 2), Scale * CameraManager.cameraZoom, SpriteEffects.None, Layer);
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
            return CameraManager.inCamera(EntityPosition, BoundBox);
        }
    }
}
