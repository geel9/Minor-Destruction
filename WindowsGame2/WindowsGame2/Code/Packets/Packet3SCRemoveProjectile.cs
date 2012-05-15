using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;

namespace MiningGame.Code.Packets
{
    public class Packet3SCRemoveProjectile : Packet
    {
        public Packet3SCRemoveProjectile(byte projectileID)
        {
            writeByte(3);
            writeByte(projectileID);
        }
    }
}
