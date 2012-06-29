using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGameServer.Shapes
{
    public class ShapeLine : Shape
    {
        public double Slope;
        public double B;
        public override AABBCollisionResult CollideAABB(ShapeAABB collidingWith)
        {
            throw new NotImplementedException();
        }

        public override RayCollisionResult CollideRay(ShapeRay collidingWith)
        {
            throw new NotImplementedException();
        }

        public override SegmentCollisionResult CollideLineSegment(ShapeLineSegment collidingWith)
        {
            throw new NotImplementedException();
        }

        public LineCollisionResult CollideLines(ShapeLine other)
        {
            double[] X = new double[] { 1, 2, 1, 2 };
            double[] Y = new double[] { Slope + B, 2*Slope + B, other.Slope + other.B, 2*other.Slope + other.B};

            double denominator = ((X[0] - X[1])*(Y[2] - Y[3])) - ((Y[0] - Y[1])*(X[2] - X[3]));
            if(denominator == 0)
                return new LineCollisionResult(Vector2.Zero, false);
                
            //Oh god
            double PX = ((X[0] * Y[1] - Y[0] * X[1]) * (X[2] - X[3])) - ((X[0] - X[1]) * (X[2] * Y[3] - Y[2] * X[3]));
            PX /= denominator;

            double PY = ((X[0] * Y[1] - Y[0] * X[1]) * (Y[2] - Y[3])) - ((Y[0] - Y[1]) * (X[2] * Y[3] - Y[2] * X[3]));
            PY /= denominator;

            return new LineCollisionResult(new Vector2((float) PX, (float) PY), true);
        }
    }

    public struct LineCollisionResult
    {
        public bool Collides;
        public Vector2 PointOfIntersection;

        public LineCollisionResult(Vector2 point, bool collides)
        {
            this.PointOfIntersection = point;
            this.Collides = collides;
        }
    }
}
