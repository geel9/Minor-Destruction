namespace MiningGameServer.Packets
{
    public class Packet2SCCreateProjectile : Packet
    {
        public Packet2SCCreateProjectile(byte ProjectileID, byte projectileType, short X, short Y, short angle, byte strength, byte owner)
        {
            WriteByte(2);
            WriteByte(projectileType);
            WriteByte(ProjectileID);
            WriteShort(X);
            WriteShort(Y);
            WriteShort(angle);
            WriteByte(strength);
            WriteByte(owner);
        }
    }
}
