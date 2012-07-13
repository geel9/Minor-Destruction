using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.Shapes;

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
