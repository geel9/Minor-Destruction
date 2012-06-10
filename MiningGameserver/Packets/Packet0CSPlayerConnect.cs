namespace MiningGameServer.Packets
{
    public class Packet0CSPlayerConnect : Packet
    {
        public Packet0CSPlayerConnect(string name)
        {
            WriteByte((byte)0);
            WriteString(name);
        }
    }
}