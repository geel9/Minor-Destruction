using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGameServer.Structs
{
    public class AABB
    {
        public int Width = 0;
        public int Height = 0;
        public Vector2 Center;

        public int Left
        {
            get { return (int)(Center.X - (Width / 2)); }
        }
        public int Right
        {
            get { return (int)(Center.X + Width / 2); }
        }

        public int Top
        {
            get { return (int)(Center.Y - Height / 2); }
        }

        public int Bottom
        {
            get { return (int)(Center.Y + Height / 2); }
        }

        public AABB(Rectangle r, float rotation = 0)
        {
            Center = new Vector2(r.Center.X, r.Center.Y);
            Width = r.Width;
            Height = r.Height;

            if (rotation == 0) return;

            Vector2 topLeft = new Vector2(r.Left, r.Top);
            Vector2 topRight = new Vector2(r.Right, r.Top);
            Vector2 bottomLeft = new Vector2(topLeft.X, r.Bottom);
            Vector2 bottomRight = new Vector2(topRight.X, bottomLeft.Y);

            topLeft = AABB.RotateAroundOrigin(topLeft, Center, rotation);
            topRight = AABB.RotateAroundOrigin(topRight, Center, rotation);
            bottomLeft = AABB.RotateAroundOrigin(bottomLeft, Center, rotation);
            bottomRight = AABB.RotateAroundOrigin(bottomRight, Center, rotation);

            float topLeftX = topLeft.X;
            float topLeftY = topLeft.Y;

            float bottomRightX = bottomRight.X;
            float bottomRightY = bottomRight.Y;
            if (topRight.X < topLeftX) topLeftX = topRight.X;
            if (bottomLeft.X < topLeftX) topLeftX = bottomLeft.X;
            if (bottomRight.X < topLeftX) topLeftX = bottomRight.X;
            if (topRight.Y < topLeftY) topLeftY = topRight.Y;
            if (bottomLeft.Y < topLeftY) topLeftY = bottomLeft.Y;
            if (bottomRight.Y < topLeftY) topLeftY = bottomRight.Y;

            if (topRight.X > bottomRightX) bottomRightX = topRight.X;
            if (bottomLeft.X > bottomRightX) bottomRightX = bottomLeft.X;
            if (topLeft.X > bottomRightX) bottomRightX = topLeft.X;
            if (topRight.Y > bottomRightY) bottomRightY = topRight.Y;
            if (bottomLeft.Y > bottomRightY) bottomRightY = bottomLeft.Y;
            if (topLeft.Y > bottomRightY) bottomRightY = topLeft.Y;

            Width = (int)(bottomRightX - topLeftX);
            Height = (int)(bottomRightY - topLeftY);
            Center = new Vector2(topLeftX + Width / 2, topLeftY + Height / 2);
        }
        public AABB(float x, float y, float width, float height, float rotation = 0) :
            this(new Rectangle((int)x, (int)y, (int)width, (int)height), rotation)
        {
        }

        public AABBResult AxisCollide(AABB bound2)
        {
            AABB bound1 = this;

            Vector2 bound1HalfWidths = new Vector2(bound1.Width / 2, bound1.Height / 2);
            Vector2 bound2HalfWidths = new Vector2(bound2.Width / 2, bound2.Height / 2);

            float yMove = 0;
            float xMove = 0;
            float multY = (bound1.Center.Y < bound2.Center.Y) ? -1 : 1;
            float multX = (bound1.Center.X < bound2.Center.X) ? -1 : 1;

            int xCDist = (int) Math.Abs(bound2.Center.X - bound1.Center.X);
            int yCDist = (int)Math.Abs(bound2.Center.Y - bound1.Center.Y);
            xMove = bound1HalfWidths.X + bound2HalfWidths.X - xCDist;
            yMove = bound1HalfWidths.Y + bound2HalfWidths.Y - yCDist;
            /*
            if (bound1.Center.X < bound2.Center.X)
            {
                xMove = bound1HalfWidths[0].X - bound2HalfWidths[0].X;
            }
            else if (bound1.Center.X >= bound2.Center.X)
            {
                xMove = bound2HalfWidths[0].X - bound1HalfWidths[0].X;
            }

            if (bound1.Center.Y < bound2.Center.Y)
            {
                yMove = bound1HalfWidths[1].Y - bound2HalfWidths[1].Y;
            }
            else if (bound1.Center.Y >= bound2.Center.Y)
            {
                yMove = bound2HalfWidths[1].Y - bound1HalfWidths[1].Y;
            }*/

            //Not colliding. SAT.
            if (yMove < 0 || xMove < 0) return new AABBResult(0, 0, false, false);

            //if (yMove < xMove && yMove > 0) xMove = 0;
            //else if (xMove < yMove && xMove > 0) yMove = 0;

            return new AABBResult((int) (xMove * multX), (int) (yMove * multY), xMove < yMove, true);
        }

        public static Vector2 RotateAroundOrigin(Vector2 point, Vector2 origin, double angle)
        {
            Vector2 real = point - origin;
            Vector2 ret = Vector2.Zero;

            ret.X = (float)((real.X * Math.Cos(angle)) - (real.Y * Math.Sin(angle)));
            ret.Y = (float)((real.X * Math.Sin(angle)) + (real.Y * Math.Cos(angle)));

            ret += origin;
            return ret;
        }

        public bool Intersects(AABB boundBox)
        {
            return AxisCollide(boundBox).IsIntersecting;
        }

        public bool Contains(AABB boundBox)
        {
            if (boundBox.Width >= Width || boundBox.Height >= Height) return false;


            return boundBox.Left - Left > 0 && Right - boundBox.Right > 0 && boundBox.Top - Top > 0 &&
                   Bottom - boundBox.Bottom > 0;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(Left, Top, Width, Height);
        }
    }
    public struct AABBResult
    {
        public int X, Y;
        public bool XSmaller;
        public bool IsIntersecting;
        public AABBResult(int X, int Y, bool XSmaller, bool intersecting)
        {
            this.X = X;
            this.Y = Y;
            this.XSmaller = XSmaller;
            IsIntersecting = intersecting;
        }
    }
}
