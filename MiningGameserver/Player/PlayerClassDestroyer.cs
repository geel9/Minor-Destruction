using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;

namespace MiningGameServer.Player
{
    public class PlayerClassDestroyer : PlayerClass
    {
        public override ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB(0, 0, 24, 48);
            }
        }

        public override void OnSpawn()
        {
            NetworkPlayer.Inventory.PickupItem(new ItemStack(1, 201));
            NetworkPlayer.Inventory.PickupItem(new ItemStack(1, 200));
            base.OnSpawn();
        }

        public override void OnDeath()
        {
            NetworkPlayer.Inventory.EmptyBag();
            base.OnDeath();
        }

        public override int GetPlayerInventorySize()
        {
            return 20;
        }

        public PlayerClassDestroyer(NetworkPlayer player)
        {
            NetworkPlayer = player;
        }

        public override float GetPlayerWalkVelocity()
        {
            return 2;
        }
    }
}
