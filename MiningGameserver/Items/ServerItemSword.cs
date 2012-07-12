using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Structs;

namespace MiningGameServer.Items
{
    public class ServerItemSword : ServerItem
    {
        public ServerItemSword()
            :base()
        {
            SetName("Sword").SetDescription("Shawing!").SetID(201).SetValue(1);
        }

        public override void OnItemUsed(int x, int y, NetworkPlayer user)
        {
            return;
            Vector2 pos = new Vector2(x, y)*GameServer.BlockSize;
            GameServer.DropItem(new ItemStack(1, 1), pos);
            base.OnItemUsed(x, y, user);
        }
    }
}
