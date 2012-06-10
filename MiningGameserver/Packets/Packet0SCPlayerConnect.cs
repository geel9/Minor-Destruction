
using Microsoft.Xna.Framework;
namespace MiningGameServer.Packets
{
    public class Packet0SCPlayerConnect : Packet
    {
        public Packet0SCPlayerConnect(string name, int playerID, Vector2 playerPos)
        {
            WriteByte((byte)0);
            WriteString(name);
            WriteInt(playerID);
            WriteInt((int)playerPos.X);
            WriteInt((int)playerPos.Y);
        }
    }
}