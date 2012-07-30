using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeeUI.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Entities;
using MiningGame.Code.Managers;

namespace MiningGame.Code.GameModes
{
    public class CTFGameMode : ClientGameMode
    {
        public PlayerEntity RedFlagCarrier = null;
        public PlayerEntity BlueFlagCarrier = null;

        public int BlueScore = 0;
        public int RedScore = 0;

        public override string GetName()
        {
            return "MDCore";
        }

        public override void OnGameModeEvent(string eventName, MiningGameServer.Packets.Packet data)
        {
            byte whichFlag;
            byte playerID;
            PlayerEntity player;
            switch (eventName)
            {
                case "drop": //Remove the flag from the player's back, basically
                    playerID = data.ReadByte();
                    player = GameWorld.GetPlayer(playerID);
                    if (player == RedFlagCarrier) RedFlagCarrier = null;
                    if (player == BlueFlagCarrier) BlueFlagCarrier = null;
                    break;

                case "pickup":
                    whichFlag = data.ReadByte();
                    playerID = data.ReadByte();
                    player = GameWorld.GetPlayer(playerID);
                    if (whichFlag == 0) BlueFlagCarrier = player;
                    if (whichFlag == 1) RedFlagCarrier = player;
                    break;
            }
            base.OnGameModeEvent(eventName, data);
        }

        public override void OnPlayerPostDraw(PlayerEntity player, SpriteBatch sb)
        {
            if(player == RedFlagCarrier)
            {
                DrawManager.DrawBox(player.EntityPosition - CameraManager.cameraPosition - new Vector2(0, 70), 20, 20, Color.Red, sb);
            }
            if (player == BlueFlagCarrier)
            {
                DrawManager.DrawBox(player.EntityPosition - CameraManager.cameraPosition - new Vector2(0, 70), 20, 20, Color.Blue, sb);
            }
            base.OnPlayerPostDraw(player, sb);
        }
    }
}
