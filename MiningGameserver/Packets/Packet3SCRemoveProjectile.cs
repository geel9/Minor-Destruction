namespace MiningGameServer.Packets
{
    public class Packet3SCRemoveProjectile : Packet
    {
        public Packet3SCRemoveProjectile(byte projectileID)
        {
            WriteByte(3);
            WriteByte(projectileID);
        }
    }
}
