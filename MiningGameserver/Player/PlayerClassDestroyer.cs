using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Blocks;
using MiningGameServer.Packets;
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

        public bool PickingUpBlock
        {
            get { return BlockPickupLocation != Vector2.Zero; }
        }

        public int BlockPickupTimer = 0;
        public Vector2 BlockPickupLocation;

        public short BlockInHand;

        public byte DestroyerUpdateMask;

        public PlayerClassDestroyer(NetworkPlayer player)
        {
            NetworkPlayer = player;
        }

        public override void OnSpawn()
        {
            NetworkPlayer.Inventory.PickupItem(new ItemStack(1, 201));
            NetworkPlayer.Inventory.PickupItem(new ItemStack(1, 200));
            base.OnSpawn();
        }

        public void PickupBlock(Vector2 location)
        {
            BlockData block = GameServer.GetBlockAt(location);
            if (block.ID == 0) return;

            BlockPickupLocation = location;
            BlockPickupTimer = 30;
            MarkClassFlagsUpdate();
            DestroyerUpdateMask |= (byte)DestroyerUpdateFlags.Picking_Up_Block;
        }

        public void PlaceBlock()
        {
            if (BlockInHand == 0) return;

            Vector2 tileLoc = NetworkPlayer.GetEntityTile();
            tileLoc.X += NetworkPlayer.FacingLeft ? -1 : 1;

            for (int i = -1; i < 3; i++)
            {
                int y = (int)tileLoc.Y - i;
                BlockData d = GameServer.GetBlockAt(tileLoc.X, y);
                BlockData dLower = GameServer.GetBlockAt(tileLoc.X, y + 1);
                if (d.ID == 0 && dLower.ID != 0)
                {
                    GameServer.SetBlock((int)tileLoc.X, y, BlockInHand);
                    BlockInHand = 0;
                    MarkClassFlagsUpdate();
                    DestroyerUpdateMask |= (byte)DestroyerUpdateFlags.Block_In_Hand;
                    break;
                }
            }
        }

        public override void Update_PostPhys(GameTime time, bool movedSince = false)
        {
            if (movedSince && BlockPickupTimer > 0)
            {
                //Moving cancels block pickup
                BlockPickupTimer = 0;
                BlockPickupLocation = Vector2.Zero;
                MarkClassFlagsUpdate();
                DestroyerUpdateMask |= (byte)DestroyerUpdateFlags.Picking_Up_Block;
            }

            if (PickingUpBlock)
            {
                BlockData block = GameServer.GetBlockAt(BlockPickupLocation);
                if (block.ID == 0)
                {
                    BlockPickupTimer = 0;
                    BlockPickupLocation = Vector2.Zero;
                    MarkClassFlagsUpdate();
                    DestroyerUpdateMask |= (byte)DestroyerUpdateFlags.Picking_Up_Block;
                }
                if (BlockPickupTimer-- <= 0 && BlockPickupLocation != Vector2.Zero)
                {
                    BlockInHand = block.ID;
                    MarkClassFlagsUpdate();
                    DestroyerUpdateMask |= (byte)DestroyerUpdateFlags.Block_In_Hand;
                    DestroyerUpdateMask |= (byte)DestroyerUpdateFlags.Picking_Up_Block;
                    GameServer.SetBlock((int)BlockPickupLocation.X, (int)BlockPickupLocation.Y, 0);
                    BlockPickupLocation = Vector2.Zero;
                    BlockPickupTimer = 0;
                }
            }

            base.Update_PostPhys(time, movedSince);
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

        public override float GetPlayerWalkVelocity()
        {
            return BlockInHand == 0 ? 2.5f : 1.5f;
        }

        public override void WriteState(Packet p)
        {
            p.WriteByte(DestroyerUpdateMask);
            if ((DestroyerUpdateMask & (byte)DestroyerUpdateFlags.Block_In_Hand) != 0)
            {
                p.WriteShort(BlockInHand);
            }
            if ((DestroyerUpdateMask & (byte)DestroyerUpdateFlags.Picking_Up_Block) != 0)
            {
                p.WriteBool(PickingUpBlock);
            }
        }

        public override void ClearUpdateMask()
        {
            DestroyerUpdateMask = 0;
        }
    }

    public enum DestroyerUpdateFlags
    {
        Picking_Up_Block = 1,
        Block_In_Hand = 2
    }
}
