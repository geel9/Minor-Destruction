using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Structs;

namespace MiningGame.Code.Packets
{
    class Packet200SCPlayerUpdate : Packet
    {
        public Packet200SCPlayerUpdate()
        {
            writeByte(200);
        }
    }

    public enum PlayerUpdateFlags
    {
        Player_Update = 1,
        Player_Position_X = 2,
        Player_Position_Y = 4,
        Player_Animation = 8,
        Player_Facing_Left = 16,
    }
}
