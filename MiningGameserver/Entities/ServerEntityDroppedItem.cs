﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Items;
using MiningGameServer.Packets;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;

namespace MiningGameServer.Entities
{
    public class ServerEntityDroppedItem : ServerEntityMoveable
    {
        public ItemStack Stack;

        public short DroppedItemID;

        private const int ItemWidth = 10;
        private const int ItemHeight = 10;

        public bool ShouldRemove;

        public override ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB((int)EntityPosition.X - ItemWidth / 2, (int)EntityPosition.Y - ItemHeight / 2, ItemWidth, ItemHeight);
            }
        }

        private int _timeAlive = 0;

        public NetworkPlayer Dropper = null;

        public void Delete()
        {
            ShouldRemove = true;
        }

        public void Pickup(NetworkPlayer picker)
        {
            int canPickup = picker.Inventory.CanPickup(Stack);
            if (canPickup == 0)
            {
                var packet = new Packet9SCItemPickedUp(DroppedItemID, picker.PlayerID);
                GameServer.ServerNetworkManager.SendPacket(packet);
                picker.Inventory.PickupItem(Stack);
                Delete();
            }
            else
            {
                if (canPickup != Stack.NumberItems)
                {
                    int pickingUp = Stack.NumberItems - canPickup;
                    Stack.NumberItems = canPickup;

                    var packet = new Packet12SCItemPickedUpIncomplete(DroppedItemID, picker.PlayerID);
                    GameServer.ServerNetworkManager.SendPacket(packet);
                    picker.Inventory.PickupItem(new ItemStack(pickingUp, Stack.ItemID));
                }
            }
        }

        public override void Update()
        {
            EntityMovement();
            if (_timeAlive++ < 15)
                return;
            if (_timeAlive >= 3600)
                Delete();

            NetworkPlayer closest = null;
            double closestDist = -1;
            foreach (NetworkPlayer player in GameServer.NetworkPlayers)
            {
                int canPickup = player.Inventory.CanPickup(Stack);
                if (canPickup == Stack.NumberItems) continue;
                if (!Stack.Item.CanPickup(player.PClass)) continue;
                //Dropping an item takes longer to pick up.
                if (Dropper != null && player == Dropper && _timeAlive < 60)
                    continue;

                float dX = player.EntityPosition.X - EntityPosition.X;
                float dY = player.EntityPosition.Y - EntityPosition.Y;
                double dist = Math.Sqrt((dX * dX) + (dY * dY));
                if (dist < closestDist || closestDist == -1)
                {
                    closestDist = dist;
                    closest = player;
                }
            }

            if (closestDist < 32 && closest != null)
            {
                Pickup(closest);
            }

            base.Update();
        }


        public ServerEntityDroppedItem(int x, int y, Vector2 velocity, ItemStack stack, short id)
        {
            ServerItem i = ServerItem.GetItem(stack.ItemID);
            EntityPosition = new Vector2(x, y);
            Stack = stack;
            DroppedItemID = id;
            EntityVelocity = velocity;
        }
    }
}
