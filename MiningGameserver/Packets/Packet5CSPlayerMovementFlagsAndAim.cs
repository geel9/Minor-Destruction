namespace MiningGameServer.Packets
{
    public class Packet5CSPlayerMovementFlagsAndAim : Packet
    {
        public Packet5CSPlayerMovementFlagsAndAim(byte flag, short aim)
        {
            WriteByte(5);
            WriteByte(flag);
            WriteShort(aim);
        }
    }
}
