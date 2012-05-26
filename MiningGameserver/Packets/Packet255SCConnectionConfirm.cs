
namespace MiningGameServer.Packets
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