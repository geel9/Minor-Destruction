﻿using System;
using Microsoft.Xna.Framework;
using MiningGame.Code.Managers;
using MiningGame.Code.Server;
using Microsoft.Xna.Framework.Graphics;
using YogUILibrary.Managers;
using MiningGame.Code.Structs;

namespace MiningGame.Code.Entities
{
    public class PlayerEntity : EntityMoveable
    {

        public byte PlayerID = 0;

        public static int PlayerWidth
        {
            get
            {
                return 11;
            }
        }

        public static int PlayerHeight
        {
            get
            {
                return 23;
            }
        }

        public bool FacingLeft = false;

        public string PlayerName = "player";

        public Animateable TorsoAnimateable;
        public Animateable LegsAnimateable;

        private Vector2 _oldPlayerPos = new Vector2();

        public override AABB BoundBox
        {
            get
            {
                return new AABB((int)EntityPosition.X - PlayerWidth / 2, (int)EntityPosition.Y - PlayerHeight / 2, PlayerWidth, PlayerHeight);
            }
        }

        public PlayerEntity(Vector2 position, byte playerID, string name = "Player")
        {
            EntityPosition = position;
            PlayerID = playerID;
            PlayerName = name;
            TorsoAnimateable = new Animateable();
            LegsAnimateable = new Animateable();
            TorsoAnimateable.setAnimation(AssetManager.GetAnimation("playertorso"));
            LegsAnimateable.setAnimation(AssetManager.GetAnimation("playerlegs"));
            TorsoAnimateable.startLooping("player_idle", "player_idle");
            LegsAnimateable.startLooping("player_idle", "player_idle");
        }

        public override void Draw(SpriteBatch sb)
        {
            AnimationControlPoint hatCP = TorsoAnimateable.getControlPoint("player_hat");
            Vector2 hatPos = new Vector2(hatCP.x, hatCP.y);
            if (FacingLeft) hatPos.X = -hatPos.X;
            Texture2D hat = AssetManager.GetTexture("hat");
            Texture2D torso = TorsoAnimateable.getCurrentFrame();
            Texture2D legs = LegsAnimateable.getCurrentFrame();
            Vector2 drawPos = (EntityPosition - new Vector2(BoundBox.Width / 2, 2)) - CameraManager.cameraPosition;
            sb.Draw(torso, drawPos , null, Color.White, 0f, new Vector2(PlayerWidth / 2, PlayerHeight / 2), 1f, !FacingLeft ? SpriteEffects.None :SpriteEffects.FlipHorizontally, 0f);
            sb.Draw(legs, drawPos, null, Color.White, 0f, new Vector2(PlayerWidth / 2, PlayerHeight / 2), 1f, !FacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            sb.Draw(hat, (drawPos - new Vector2(PlayerWidth / 2, PlayerHeight/2)) + new Vector2(torso.Width/2, torso.Height/2) + hatPos, null, Color.White, 0f, new Vector2(hat.Width/2, hat.Height/2), 1f, !FacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            
            TorsoAnimateable.AnimationUpdate();
            LegsAnimateable.AnimationUpdate();

            Vector2 aimingAt = GetBlockAimingAt();

            byte blockID = GameWorld.GetBlockIDAt(aimingAt.X, aimingAt.Y);

            Rectangle drawBB = new Rectangle((int)aimingAt.X * GameWorld.BlockWidth, (int)aimingAt.Y * GameWorld.BlockHeight, GameWorld.BlockWidth, GameWorld.BlockHeight);
            DrawManager.Draw_Outline(ConversionManager.PToV(drawBB.Center) - CameraManager.cameraPosition, drawBB.Width, drawBB.Height, Color.White, sb);

            sb.DrawString(AssetManager.GetFont("Console"), PlayerName, EntityPosition - new Vector2(30, 30) - CameraManager.cameraPosition, Color.White);
            base.Draw(sb);
        }

        public float GetAimingAngle()
        {
            Vector2 aim = InputManager.GetMousePosV() + CameraManager.cameraPosition;
            aim -= EntityPosition;
            return (float) Math.Atan2(aim.Y, aim.X);
        }

        public Vector2 GetBlockAimingAt()
        {
            //if (InputManager.GetMousePosV().Y <= 56) return Vector2.Zero;
            Vector2 tile = GameWorld.AbsoluteToTile(InputManager.GetMousePosV() + CameraManager.cameraPosition);
            if (tile.X >= GameWorld.WorldSizeX || tile.X < 0 || tile.Y >= GameServer.WorldSizeY || tile.Y < 0) return Vector2.Zero;
            return tile;
        }

        public void Update(GameTime time, bool server = false)
        {
            if (server)
                EntityMovement();
            _oldPlayerPos = EntityPosition;
            base.Update(time);
        }

    }
}
