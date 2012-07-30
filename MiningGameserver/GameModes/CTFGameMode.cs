using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGameServer.Blocks;
using MiningGameServer.Packets;

namespace MiningGameServer.GameModes
{
    //NOTE: This class will not be in the final game.
    //When possible, move this to an external mod.
    public class CTFGameMode : ServerGameMode
    {
        public NetworkPlayer RedFlagCarrier = null;
        public NetworkPlayer BlueFlagCarrier = null;

        public Vector2 RedFlagLocation = Vector2.Zero;
        public Vector2 BlueFlagLocation = Vector2.Zero;

        public int BlueScore = 0;
        public int RedScore = 0;

        public override string GetName()
        {
            return "MDCore";
        }

        public void PickupFlag(NetworkPlayer pickerupper, BlockData block)
        {
            if (block.ID != 9001 || pickerupper.PlayerTeam == block.MetaData || block.MetaData > 1) return;
            //1 = red
            //0 = blue
            byte metaData = block.MetaData;
            if (metaData == 0 )
            {
                BlueFlagCarrier = pickerupper;
                GameServer.SetBlock((int)BlueFlagLocation.X, (int)BlueFlagLocation.Y, 0);
                GameServer.SendMessageToAll(pickerupper.PlayerName + " has picked up the blue flag.");
            }
            if (metaData == 1)
            {
                RedFlagCarrier = pickerupper;
                GameServer.SetBlock((int)RedFlagLocation.X, (int)RedFlagLocation.Y, 0);
                GameServer.SendMessageToAll(pickerupper.PlayerName + " has picked up the red flag.");
            }

            Packet p = new Packet((byte)metaData, pickerupper.PlayerID);
            SendGameModeEvent("pickup", p);
        }

        public void PlayerScore(NetworkPlayer scorer, BlockData touched)
        {
            if (touched.ID != 9001) return;
            if (scorer != RedFlagCarrier && scorer != BlueFlagCarrier) return;
            if(scorer == RedFlagCarrier)
            {
                RedFlagCarrier = null;
                BlueScore += 1;
                GameServer.SetBlock((int)RedFlagLocation.X, (int)RedFlagLocation.Y, 9001, true, 1);
                GameServer.SendMessageToAll(scorer.PlayerName + " has scored a point for blue. Blue's score: " + BlueScore);
                
                Packet p = new Packet(scorer.PlayerID);
                SendGameModeEvent("drop", p);
            }
            if (scorer == BlueFlagCarrier)
            {
                BlueFlagCarrier = null;
                RedScore += 1;
                GameServer.SetBlock((int)BlueFlagLocation.X, (int)BlueFlagLocation.Y, 9001);
                GameServer.SendMessageToAll(scorer.PlayerName + " has scored a point for red. Red's score: " + RedScore);
                
                Packet p = new Packet(scorer.PlayerID);
                SendGameModeEvent("drop", p);
            }
        }

        public void PlayerDropFlag(NetworkPlayer dropper)
        {
            if (dropper == RedFlagCarrier)
            {
                RedFlagCarrier = null;
                GameServer.SetBlock((int)RedFlagLocation.X, (int)RedFlagLocation.Y, 9001, true, 1);
                GameServer.SendMessageToAll(dropper.PlayerName + " has dropped the red flag.");
                Packet p = new Packet(dropper.PlayerID);
                SendGameModeEvent("drop", p);
            }
            else if (dropper == BlueFlagCarrier)
            {
                BlueFlagCarrier = null;
                GameServer.SetBlock((int)BlueFlagLocation.X, (int)BlueFlagLocation.Y, 9001);
                GameServer.SendMessageToAll(dropper.PlayerName + " has dropped the blue flag.");
                Packet p = new Packet(dropper.PlayerID);
                SendGameModeEvent("drop", p);
            }
        }

        public override void OnGameModeChosen()
        {
            RedFlagLocation = new Vector2(100, 19);
            BlueFlagLocation = new Vector2(5, 19);

            GameServer.SetBlock((int)RedFlagLocation.X, (int)RedFlagLocation.Y, 9001, true, 1);
            GameServer.SetBlock((int)BlueFlagLocation.X, (int)BlueFlagLocation.Y, 9001);

            RedFlagCarrier = null;
            BlueFlagCarrier = null;
            base.OnGameModeChosen();
        }

        public override void OnPlayerDeath(NetworkPlayer player)
        {
            PlayerDropFlag(player);
            base.OnPlayerDeath(player);
        }

        public override void OnPlayerChooseClass(NetworkPlayer player, int classChosen)
        {
            PlayerDropFlag(player);
            base.OnPlayerChooseClass(player, classChosen);
        }

        public override void OnPlayerChooseTeam(NetworkPlayer player, int team)
        {
            PlayerDropFlag(player);
            base.OnPlayerChooseTeam(player, team);
        }

        public override void OnPlayerDisconnect(NetworkPlayer player)
        {
            PlayerDropFlag(player);
            base.OnPlayerDisconnect(player);
        }

        public override void OnPlayerSpawn(NetworkPlayer player)
        {
            if(player.PlayerTeam == 1)
            {
                player.EntityPosition.X = 2400;
            }
            base.OnPlayerSpawn(player);
        }
    }
}
