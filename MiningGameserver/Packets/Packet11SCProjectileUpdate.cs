using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.Packets
{
    //You may be wondering why this is necessary; because surely projectiles can be deterministic
    //and only need to tell the client where to create/destroy them.
    //Shut up.
    public class Packet11SCProjectileUpdate : Packet
    {
        public Packet11SCProjectileUpdate(byte ProjectileID, short X, short Y, short XVel, short YVel)
        {
            WriteByte(11);
            WriteByte(ProjectileID);
            WriteShort(X);
            WriteShort(Y);
            WriteShort(XVel);
            WriteShort(YVel);
        }
    }
}
