using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Packets;
using MiningGame.Code.Structs;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework;
namespace MiningGame.Code.Packets
{
    public class Packet255SCConnectionFirmed : Packet
    {
        public Packet255SCConnectionFirmed(int playerID)
        {
            writeByte((byte)255);
            writeInt(playerID);
        }
    }
}