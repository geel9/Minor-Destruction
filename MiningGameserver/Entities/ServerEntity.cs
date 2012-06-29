using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;

namespace MiningGameServer.Entities
{
    public class ServerEntity
    {
        public Vector2 EntityPosition;

        public float Rotation;

        public virtual ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB(EntityPosition.X, EntityPosition.Y, 0, 0);
            }
        }

        public virtual void Update()
        {
            
        }
    }
}
