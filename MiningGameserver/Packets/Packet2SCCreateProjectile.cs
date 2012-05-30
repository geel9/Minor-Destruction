namespace MiningGameServer.Packets
{
    public class Packet2SCCreateProjectile : Packet
    {
        public Packet2SCCreateProjectile(byte ProjectileID, byte projectileType, short X, short Y, short angle, byte strength, byte owner)
        {
            writeByte(2);
            writeByte(projectileType);
            writeByte(ProjectileID);
            writeShort(X);
            writeShort(Y);
            writeShort(angle);
            writeByte(strength);
            writeByte(owner);
        }
    }
}
