using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGameServer.Shapes
{
    public class ShapeLineSegment : Shape
    {
        public double Slope;
        public double B;
        public Vector2 Start;
        public Vector2 End;

        //Highest Endpoint
        public Vector2 HighestEP
        {
            get { return Start.Y > End.Y ? Start : End; }
        }

        //Lowest Endpoint
        public Vector2 LowestEP
        {
            get { return Start.Y > End.Y ? End : Start; }
        }

        public ShapeLineSegment(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;

            Slope = (start.Y - end.Y) / (start.X - end.X);
            B = start.Y - (Slope * start.X);
        }

        public override AABBCollisionResult CollideAABB(ShapeAABB collidingWith)
        {
            throw new NotImplementedException();
        }

        public override RayCollisionResult CollideRay(ShapeRay collidingWith)
        {
            throw new NotImplementedException();
        }

        public override SegmentCollisionResult CollideLineSegment(ShapeLineSegment other)
        {
            //We can just do a line-line intersection, as if both Segments are Lines,
            //and then see if the point of intersection
            //is on both segments. No need for ridiculous math here!
            ShapeLine one = ToLine();
            ShapeLine two = other.ToLine();
            LineCollisionResult res = one.CollideLines(two);
            if(res.Collides && PointLiesOnSegment(res.PointOfIntersection) && other.PointLiesOnSegment(res.PointOfIntersection))
            {
                return new SegmentCollisionResult {Collides = true, PointOfCollision = res.PointOfIntersection};
            }
            return new SegmentCollisionResult();
        }

        public ShapeLine ToLine()
        {
            return new ShapeLine{B = B, Slope = Slope};
        }

        public bool PointLiesOnSegment(Vector2 point)
        {
            double CurSlope = (Start.Y - point.Y) / (Start.X - point.X);
            if (!CurSlope.Equals(Slope)) return false;

            return (point.Y <= HighestEP.Y && point.Y >= LowestEP.Y);

        }
    }

    public struct SegmentCollisionResult
    {
        public bool Collides;
        public Vector2 PointOfCollision;
    }
}
