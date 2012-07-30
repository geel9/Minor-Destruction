using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGameServer.GameModes;

namespace MiningGameServer.Blocks
{
    public class BlockFlag : Block
    {
        public BlockFlag()
        {
            this.SetBlockID(9001).SetBlockInvincible().SetBlockPistonable(false).SetBlockName("Flag");
        }

        public override void OnBlockTouched(int X, int Y, int side, Entities.ServerEntityMoveable toucher)
        {
            if (!(GameServer.GameMode is CTFGameMode)) return;
            if (!(toucher is NetworkPlayer)) return;

            NetworkPlayer player = (NetworkPlayer)toucher;
            CTFGameMode game = (CTFGameMode)GameServer.GameMode;
            BlockData data = GameServer.GetBlockAt(X, Y);

            if (data.MetaData == player.PlayerTeam)
            {
                game.PlayerScore(player, data);
            }
            else
            {
                game.PickupFlag(player, data);
            }


            base.OnBlockTouched(X, Y, side, toucher);
        }
    }
}
