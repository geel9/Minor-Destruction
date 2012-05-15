using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public struct BoundPacket
    {
        public Action<Packet> toFire;
        public byte idToFireOn;

        public BoundPacket(Action<Packet> a, byte b)
        {
            this.toFire = a;
            this.idToFireOn = b;
        }
    }
}
