using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer;

namespace MiningGameserver.Blocks
{
    public class BlockPistonBase : Block
    {
        public BlockPistonBase()
            : base()
        {
            SetBlockID(20).SetBlockName("Piston Base").SetBlockPistonable(false);
        }

        public override void OnBlockPlaced(int x, int y, bool notify, NetworkPlayer placer = null)
        {
            if (placer == null)
                return;

            int realX = x * 16;
            int realY = y * 16;
            int diffX = (int)(placer.EntityPosition.X - realX);
            int diffY = (int)(placer.EntityPosition.Y - realY);

            if (Math.Abs(diffX) > Math.Abs(diffY))
            {
                if (diffX < 0)
                    GameServer.SetBlockMetaData(x, y, (byte)PistonFlags.Left);
                else
                    GameServer.SetBlockMetaData(x, y, (byte)PistonFlags.Right);
            }
            else
            {
                if (diffY < 0)
                    GameServer.SetBlockMetaData(x, y, (byte)PistonFlags.Up);
                else
                    GameServer.SetBlockMetaData(x, y, (byte)PistonFlags.Down);
            }


            base.OnBlockPlaced(x, y, notify);
        }

        public override void OnBlockUsed(int x, int y)
        {
            byte flags = GameServer.GetBlockAt(x, y).MetaData;

            PistonFlags dir = PistonFlags.Left;
            if ((flags & (int)PistonFlags.Right) != 0) dir = PistonFlags.Right;
            if ((flags & (int)PistonFlags.Up) != 0) dir = PistonFlags.Up;
            if ((flags & (int)PistonFlags.Down) != 0) dir = PistonFlags.Down;

            if ((flags & (int)PistonFlags.Open) != 0)
            {
                flags ^= (int)PistonFlags.Open;
                ClosePiston(x, y, dir);
            }
            else
            {
                if (OpenPiston(x, y, dir))
                    flags |= (int)PistonFlags.Open;
            }
            GameServer.SetBlockMetaData(x, y, flags);
        }

        public bool OpenPiston(int x, int y, PistonFlags direction)
        {
            int xMove = 0;
            int yMove = 0;
            switch (direction)
            {
                case PistonFlags.Left:
                    xMove = -1;
                    break;
                case PistonFlags.Right:
                    xMove = 1;
                    break;
                case PistonFlags.Down:
                    yMove = 1;
                    break;
                case PistonFlags.Up:
                    yMove = -1;
                    break;
            }
            int closestEmpty = -1;
            bool isBlocked = false;
            for (int i = 1; i <= 6; i++)
            {
                BlockData b = GameServer.GetBlockAt(x + (xMove * i), y + (yMove * i));
                if (b.ID == 0)
                {
                    closestEmpty = i;
                    break;
                }
                if (!b.Block.GetBlockPistonable())
                {
                    isBlocked = true;
                    break;
                }
            }
            if (!isBlocked && closestEmpty != -1)
            {
                for (int i = closestEmpty; i > 1; i--)
                {
                    int curX = x + xMove * i;
                    int curY = y + yMove * i;
                    int nextX = curX - xMove;
                    int nextY = curY - yMove;
                    BlockData next = GameServer.GetBlockAt(nextX, nextY);
                    GameServer.SetBlock(curX, curY, next.ID, true, next.MetaData);
                    GameServer.SetBlock(nextX, nextY, 0, false, 0);
                }
            }
            if (closestEmpty != -1)
            {
                int xM = x + xMove;
                int yM = y + yMove;
                GameServer.SetBlock(xM, yM, 21, true, (byte)direction);
                return true;
            }
            return false;
        }

        public void ClosePiston(int x, int y, PistonFlags direction)
        {
            int xMove = 0;
            int yMove = 0;
            switch (direction)
            {
                case PistonFlags.Left:
                    xMove = -1;
                    break;
                case PistonFlags.Right:
                    xMove = 1;
                    break;
                case PistonFlags.Down:
                    yMove = 1;
                    break;
                case PistonFlags.Up:
                    yMove = -1;
                    break;
            }
            BlockData block = GameServer.GetBlockAt(x + xMove, y + yMove);
            if (block.ID == 21)
            {
                GameServer.SetBlock(x + xMove, y + yMove, 0);
            }
        }

        public override void OnBlockRemoved(int x, int y)
        {
            byte flags = GameServer.GetBlockAt(x, y).MetaData;

            PistonFlags dir = PistonFlags.Left;
            if ((flags & (int)PistonFlags.Right) != 0) dir = PistonFlags.Right;
            if ((flags & (int)PistonFlags.Up) != 0) dir = PistonFlags.Up;
            if ((flags & (int)PistonFlags.Down) != 0) dir = PistonFlags.Down;
        }
    }
    public enum PistonFlags
    {
        Open = 1,
        Left = 2,
        Right = 4,
        Up = 8,
        Down = 16
    }
}
