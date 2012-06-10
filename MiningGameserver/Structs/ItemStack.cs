using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.Packets;
using MiningGameServer.Interfaces;

namespace MiningGameServer.Structs
{
    public struct ItemStack : INetTransferable<ItemStack>
    {
        public int NumberItems;
        public byte ItemID;

        public ItemStack(int number = 0, byte id = 0)
        {
            NumberItems = number;
            ItemID = id;
        }

        public void Write(Packet p)
        {
            p.WriteByte(ItemID);
            p.WriteInt(NumberItems);
        }

        public ItemStack Read(Packet p)
        {
            byte ID = p.ReadByte();
            int num = p.ReadInt();
            return new ItemStack(num, ID);
        }
    }
}
