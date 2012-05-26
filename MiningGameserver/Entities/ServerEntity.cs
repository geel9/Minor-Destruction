using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Structs;

namespace MiningGameserver.Entities
{
    public class ServerEntity
    {
        public Vector2 EntityPosition;

        public float Rotation;

        public virtual AABB BoundBox
        {
            get
            {
                return new AABB(EntityPosition.X, EntityPosition.Y, 0, 0);
            }
        }

        public virtual void Update()
        {
            
        }
    }
}
