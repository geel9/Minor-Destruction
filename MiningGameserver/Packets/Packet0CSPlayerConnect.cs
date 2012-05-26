namespace MiningGameServer.Packets
{
    public class Packet0CSPlayerConnect : Packet
    {
        public Packet0CSPlayerConnect(string name)
        {
            writeByte((byte)0);
            writeString(name);
        }
    }
}