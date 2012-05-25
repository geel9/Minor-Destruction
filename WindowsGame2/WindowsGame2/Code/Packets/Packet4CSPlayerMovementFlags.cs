using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;

namespace MiningGame.Code.Packets
{
    class Packet4CSPlayerMovementFlags : Packet
    {
        public Packet4CSPlayerMovementFlags(byte flag)
        {
            writeByte(4);
            writeByte(flag);
        }
    }

    public enum PlayerMovementFlag
    {
        Left_Pressed = 1,
        Right_Pressed = 2,
        Jump_Pressed = 4,
        Attack_Pressed = 8
    }
}
