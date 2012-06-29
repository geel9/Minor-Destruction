using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Shapes
{
    public abstract class Shape
    {
        public abstract AABBCollisionResult CollideAABB(ShapeAABB collidingWith);

        public abstract RayCollisionResult CollideRay(ShapeRay collidingWith);

        public abstract SegmentCollisionResult CollideLineSegment(ShapeLineSegment collidingWith);

    }
}
