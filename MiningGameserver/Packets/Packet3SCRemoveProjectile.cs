namespace MiningGameServer.Packets
{
    public class Packet3SCRemoveProjectile : Packet
    {
        public Packet3SCRemoveProjectile(byte projectileID)
        {
            writeByte(3);
            writeByte(projectileID);
        }
    }
}
