﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGameServer.Shapes
{
    public class ShapeAABB : Shape
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

        public ShapeAABB(Rectangle r, float rotation = 0)
        {
            Center = new Vector2(r.Center.X, r.Center.Y);
            Width = r.Width;
            Height = r.Height;

            if (rotation == 0) return;

            Vector2 topLeft = new Vector2(r.Left, r.Top);
            Vector2 topRight = new Vector2(r.Right, r.Top);
            Vector2 bottomLeft = new Vector2(topLeft.X, r.Bottom);
            Vector2 bottomRight = new Vector2(topRight.X, bottomLeft.Y);

            topLeft = ShapeAABB.RotateAroundOrigin(topLeft, Center, rotation);
            topRight = ShapeAABB.RotateAroundOrigin(topRight, Center, rotation);
            bottomLeft = ShapeAABB.RotateAroundOrigin(bottomLeft, Center, rotation);
            bottomRight = ShapeAABB.RotateAroundOrigin(bottomRight, Center, rotation);

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
        public ShapeAABB(float x, float y, float width, float height, float rotation = 0) :
            this(new Rectangle((int)x, (int)y, (int)width, (int)height), rotation)
        {
        }

        public override AABBCollisionResult CollideAABB(ShapeAABB bound2)
        {
            ShapeAABB bound1 = this;

            Vector2 bound1HalfWidths = new Vector2(bound1.Width / 2, bound1.Height / 2);
            Vector2 bound2HalfWidths = new Vector2(bound2.Width / 2, bound2.Height / 2);

            float yMove = 0;
            float xMove = 0;
            float multY = (bound1.Center.Y < bound2.Center.Y) ? -1 : 1;
            float multX = (bound1.Center.X < bound2.Center.X) ? -1 : 1;

            int xCDist = (int)Math.Abs(bound2.Center.X - bound1.Center.X);
            int yCDist = (int)Math.Abs(bound2.Center.Y - bound1.Center.Y);
            xMove = bound1HalfWidths.X + bound2HalfWidths.X - xCDist;
            yMove = bound1HalfWidths.Y + bound2HalfWidths.Y - yCDist;
            //Not colliding. SAT.
            if (yMove < 0 || xMove < 0) return new AABBCollisionResult(0, 0, false, false);

            //if (yMove < xMove && yMove > 0) xMove = 0;
            //else if (xMove < yMove && xMove > 0) yMove = 0;

            return new AABBCollisionResult((int)(xMove * multX), (int)(yMove * multY), xMove < yMove, true);
        }

        public override RayCollisionResult CollideRay(ShapeRay collidingWith)
        {
            throw new NotImplementedException();
        }

        public override SegmentCollisionResult CollideLineSegment(ShapeLineSegment collidingWith)
        {
            throw new NotImplementedException();
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

        public bool Intersects(ShapeAABB boundBox)
        {
            return CollideAABB(boundBox).IsIntersecting;
        }

        public bool Contains(ShapeAABB boundBox)
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
    public struct AABBCollisionResult
    {
        public int X, Y;
        public bool XSmaller;
        public bool IsIntersecting;
        public Vector2 Projection
        {
            get
            {
                if (X == Y)
                    return new Vector2(X, Y);
                return XSmaller ? new Vector2(X, 0) : new Vector2(0, Y);
            }
        }

        //For velocities. Put 0 in the field that is smaller.
        public Vector2 MultProjection
        {
            get
            {
                if (X == Y)
                    return new Vector2(0, 0);
                return XSmaller ? new Vector2(0, 1) : new Vector2(1, 0);
            }
        }

        public AABBCollisionResult(int X, int Y, bool XSmaller, bool intersecting)
        {
            this.X = X;
            this.Y = Y;
            this.XSmaller = XSmaller;
            IsIntersecting = intersecting;
        }
    }
}
