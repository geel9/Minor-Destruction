using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGameServer.Shapes
{
    public class ShapeRay : Shape
    {
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
    }

    public struct RayCollisionResult
    {
        public bool Collides;
        public Vector2 PointOfCollision;
    }
}
