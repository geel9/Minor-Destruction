using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGameServer.Packets
{
    public class Packet8SCItemDropped : Packet
    {
        public Packet8SCItemDropped(Vector2 position, Vector2 velocity, short droppedID, byte itemID)
        {
            WriteByte(8);
            WriteVectorS(position);
            WriteVectorS(velocity);
            WriteShort(droppedID);
            WriteByte(itemID);
        }
    }
}
