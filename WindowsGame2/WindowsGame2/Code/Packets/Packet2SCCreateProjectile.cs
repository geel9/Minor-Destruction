using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;

namespace MiningGame.Code.Packets
{
    public class Packet2SCCreateProjectile : Packet
    {
        public Packet2SCCreateProjectile(byte ProjectileID, byte projectileType, short X, short Y, float angle, byte strength, byte owner)
        {
            writeByte(2);
            writeByte(projectileType);
            writeByte(ProjectileID);
            writeShort(X);
            writeShort(Y);
            writeFloat(angle);
            writeByte(strength);
            writeByte(owner);
        }
    }
}
