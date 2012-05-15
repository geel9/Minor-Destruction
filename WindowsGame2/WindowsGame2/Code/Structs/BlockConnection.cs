using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGame.Code.Structs
{
    public class BlockConnection
    {

        public ConnectedBlock blockConnection1;
        public ConnectedBlock blockConnection2;
        public BlockConnection(Vector2 pos1, Vector2 pos2)
        {
            blockConnection1 = new ConnectedBlock(pos1, 1);
            blockConnection2 = new ConnectedBlock(pos2, 2);
        }
    }

    public class ConnectedBlock
    {
        public int connectedNum = 0;
        public Vector2 blockPosition = Vector2.Zero;

        public ConnectedBlock(Vector2 pos, int connectedNum)
        {
            this.connectedNum = connectedNum;
            this.blockPosition = pos;
        }

    }
}
