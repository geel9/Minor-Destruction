using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
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
        public byte OtherPlayerNetworkFlags = 0;

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

        private void PlayerCollisions()
        {
            foreach(NetworkPlayer p in GameServer.NetworkPlayers)
            {
                if (p.PlayerEntity == this) continue;

                AABBResult collide = p.PlayerEntity.BoundBox.AxisCollide(BoundBox);
                if (!collide.IsIntersecting) continue;
                
                if(collide.XSmaller)
                {
                    EntityVelocity.X = 0;
                    EntityPosition.X -= collide.X;
                }
                else
                {
                    EntityVelocity.Y = 0;
                    EntityPosition.Y -= collide.Y;
                }
            }
        }

        private void BlockCollisions()
        {
            List<Vector2> newTiles = RectangleHitsTiles(BoundBox);

            foreach (Vector2 newTile in newTiles)
            {
                byte blockID = GameWorld.GetBlockIDAt(newTile.X, newTile.Y);

                if (blockID == 0) continue;

                Block block = Block.GetBlock(blockID);
                bool walkThrough = block.GetBlockWalkThrough();

                //A wall
                Rectangle blockBB = block.GetBlockBoundBox((int)newTile.X, (int)newTile.Y);

                AABB thisAABB = BoundBox;
                AABB blockAABB = new AABB(blockBB);
                AABBResult collide = thisAABB.AxisCollide(blockAABB);

                if (!collide.IsIntersecting) continue;

                if (collide.XSmaller)
                {
                    bool right = (collide.X < 0);
                    if (!walkThrough)
                    {
                        EntityVelocity.X = 0;
                        EntityPosition.X += collide.X;
                    }
                    block.OnBlockTouched((int)newTile.X, (int)newTile.Y, right ? 3 : 1, this);
                }
                else
                {
                    bool up = (collide.Y > 0);
                    //if (up && EntityVelocity.Y < 0) continue;
                    if (!walkThrough)
                    {
                        EntityVelocity.Y = 0;
                        EntityPosition.Y += collide.Y;
                        if (!up)
                        {
                            TimeFalling = 0;
                            Falling = false;
                        }
                    }
                    block.OnBlockTouched((int)newTile.X, (int)newTile.Y, up ? 2 : 0, this);
                }
            }
        }

        public override void EntityMovement()
        {
            if (BoundBox.Left < 0) EntityPosition.X = BoundBox.Width / 2 + 1;
            if (BoundBox.Top < 0) EntityPosition.Y = BoundBox.Height / 2 + 1;
            if (BoundBox.Right > GameWorld.BlockWidth * GameWorld.WorldSizeX) EntityPosition.X = GameWorld.BlockWidth * GameWorld.WorldSizeX - (BoundBox.Width / 2);
            if (BoundBox.Bottom > GameWorld.BlockHeight * GameWorld.WorldSizeY) EntityPosition.Y = GameWorld.BlockHeight * GameWorld.WorldSizeY - (BoundBox.Height / 2);

            //Didn't want to make a new BoundBox so this'll do. Gets the tiles the player will be in with his velocity.
            EntityPosition += EntityVelocity;
            if(EntityVelocity != Vector2.Zero)
                PlayerCollisions();

            BlockCollisions();

            //Ropes
            byte blockID2 = GameWorld.GetBlockIDAt(GetEntityTile().X, GetEntityTile().Y);

            if (EntityVelocity.Y < 6 && blockID2 != 6) EntityVelocity.Y += 1; // Gravity! This is a really nice side effect: The code for not allowing the player to go through a block downwards already exists, so I just need to add this one line to add gravity!

            if (blockID2 == 6)
            {
                if (EntityVelocity.Y > 0) EntityVelocity.Y--;
                if (EntityVelocity.Y < 0) EntityVelocity.Y++;
            }

            if (EntityVelocity.X < 0) EntityVelocity.X += 1;
            if (EntityVelocity.X > 0) EntityVelocity.X -= 1;

            if (EntityVelocity.Y != 1) Falling = true;
        }

        public PlayerEntity(Vector2 position, byte playerID, string name = "Player")
        {
            EntityPosition = position;
            PlayerID = playerID;
            PlayerName = name;
            TorsoAnimateable = new Animateable();
            LegsAnimateable = new Animateable();
            TorsoAnimateable.SetAnimation(AssetManager.GetAnimation("playertorso"));
            LegsAnimateable.SetAnimation(AssetManager.GetAnimation("playerlegs"));
            TorsoAnimateable.StartLooping("player_idle", "player_idle");
            LegsAnimateable.StartLooping("player_idle", "player_idle");
        }

        public override void Draw(SpriteBatch sb)
        {
            AnimationControlPoint hatCP = TorsoAnimateable.GetControlPoint("player_hat");
            Vector2 hatPos = new Vector2(hatCP.x, hatCP.y);
            if (FacingLeft) hatPos.X = -hatPos.X;
            Texture2D hat = AssetManager.GetTexture("hat");
            Texture2D torso = TorsoAnimateable.GetCurrentFrame();
            Texture2D legs = LegsAnimateable.GetCurrentFrame();
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

            ConsoleManager.setVariableValue("window_title", (int)aimingAt.X + ", " + (int)aimingAt.Y);

            sb.DrawString(AssetManager.GetFont("Console"), PlayerName, EntityPosition - new Vector2(30, 30) - CameraManager.cameraPosition, Color.White);
            base.Draw(sb);
        }

        public short GetAimingAngle()
        {
            Vector2 aim = InputManager.GetMousePosV() + CameraManager.cameraPosition;
            aim -= EntityPosition;
            return (short) ConversionManager.RadianToDegrees(Math.Atan2(aim.Y, aim.X));
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
