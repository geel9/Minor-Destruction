using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public struct BoundPacket
    {
        public Action<Packet> ToFire;
        public byte IDToFireOn;

        public BoundPacket(Action<Packet> a, byte b)
        {
            this.ToFire = a;
            this.IDToFireOn = b;
        }
    }
}
