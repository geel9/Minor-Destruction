using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeeUI.Managers;
using Microsoft.Xna.Framework;
using MiningGame.Code.Managers;
using MiningGameServer.Shapes;

namespace MiningGame.Code.GameModes
{
    public class CPGameMode : ClientGameMode
    {
        public ShapeAABB RedCapPoint;
        public ShapeAABB BluCapPoint;

        public int RedProgress;
        public int BlueProgress;

        public override string GetName()
        {
            return "Control Points";
        }

        public CPGameMode()
        {
            RedCapPoint = new ShapeAABB(2520, 384, 168, 96);
            BluCapPoint = new ShapeAABB(120, 384, 168, 96);
        }

        public override void Draw_PostWorld(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Vector2 PositionRed = RedCapPoint.Center - CameraManager.cameraPosition;
            Vector2 PositionBlue = BluCapPoint.Center - CameraManager.cameraPosition;

            DrawManager.DrawBox(PositionRed, RedCapPoint.Width, RedCapPoint.Height, Color.Red, sb, 0, 100);
            DrawManager.DrawBox(PositionBlue, BluCapPoint.Width, BluCapPoint.Height, Color.Blue, sb, 0, 100);

            Vector2 topLeftBlu = new Vector2(174, 300) - CameraManager.cameraPosition;
            Vector2 bottomRightBlu = new Vector2(214, 340) - CameraManager.cameraPosition;
            Vector2 topLeftRed = new Vector2(2574, 300) - CameraManager.cameraPosition;
            Vector2 bottomRightRed = new Vector2(2614, 340) - CameraManager.cameraPosition;

            if (BlueProgress > 0)
            {
                Vector2 topLeftBluProgress = new Vector2(174, 340 - (40*(BlueProgress / 100f))) - CameraManager.cameraPosition;
                DrawManager.DrawBox(topLeftBluProgress, bottomRightBlu, Color.Red, sb, 0, 150);
                bottomRightBlu.Y = topLeftBluProgress.Y;
            }
            if (RedProgress > 0)
            {
                Vector2 topLeftRedProgress = new Vector2(1574, 340 - (40 * (RedProgress / 100f))) - CameraManager.cameraPosition;
                DrawManager.DrawBox(topLeftRedProgress, bottomRightRed, Color.Blue, sb, 0, 150);
                bottomRightRed.Y = topLeftRedProgress.Y;
            }

            DrawManager.DrawBox(topLeftRed, bottomRightRed, Color.Red, sb, 0f, 150);
            DrawManager.DrawBox(topLeftBlu, bottomRightBlu, Color.Blue, sb, 0f, 150);

            
            base.Draw_PostWorld(sb);
        }

        public override void OnGameModeEvent(string eventName, MiningGameServer.Packets.Packet data)
        {
            switch(eventName)
            {
                case "w":
                    byte team = data.ReadByte();
                    RedProgress = 0;
                    BlueProgress = 0;
                    break;
                    
                case "t":
                    RedProgress = 0;
                    BlueProgress = 0;
                    break;
                    //Red CP progress
                case "pr":
                    RedProgress = data.ReadByte();
                    break;

                case "pb":
                    BlueProgress = data.ReadByte();
                    break;
            }
            base.OnGameModeEvent(eventName, data);
        }
    }
}
