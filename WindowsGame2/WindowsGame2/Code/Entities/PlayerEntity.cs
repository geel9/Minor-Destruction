using System;
using GeeUI.Managers;
using Microsoft.Xna.Framework;
using MiningGame.Code.Blocks;
using MiningGame.Code.Items;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.ExtensionMethods;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;
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

        public bool FacingLeft;
        public byte OtherPlayerNetworkFlags;

        public string PlayerName = "player";

        public Animateable TorsoAnimateable;
        public Animateable LegsAnimateable;

        public Item EquippedItem;

        private Vector2 _oldPlayerPos = new Vector2();

        public override ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB((int)EntityPosition.X - PlayerWidth / 2, (int)EntityPosition.Y - PlayerHeight / 2, PlayerWidth, PlayerHeight);
            }
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
            sb.Draw(torso, drawPos, null, Color.White, 0f, new Vector2(PlayerWidth / 2, PlayerHeight / 2), 1f, !FacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            sb.Draw(legs, drawPos, null, Color.White, 0f, new Vector2(PlayerWidth / 2, PlayerHeight / 2), 1f, !FacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            sb.Draw(hat, (drawPos - new Vector2(PlayerWidth / 2, PlayerHeight / 2)) + new Vector2(torso.Width / 2, torso.Height / 2) + hatPos, null, Color.White, 0f, new Vector2(hat.Width / 2, hat.Height / 2), 1f, !FacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            TorsoAnimateable.AnimationUpdate();
            LegsAnimateable.AnimationUpdate();

            Vector2 aimingAt = GetBlockAimingAt();

            short BlockID = GameWorld.GetBlockAt(aimingAt.X, aimingAt.Y).ID;
            Rectangle drawBB = new Rectangle((int)aimingAt.X * GameWorld.BlockWidth, (int)aimingAt.Y * GameWorld.BlockHeight, GameWorld.BlockWidth, GameWorld.BlockHeight);
            Item inHand = GameWorld.ThePlayer.GetPlayerItemInHand();
            short itemBlockID = 0;
            if(inHand != null)
                itemBlockID = inHand.GetBlockID();
            Vector2 blockDrawPos = aimingAt*GameWorld.BlockHeight;
            if (BlockID == 0)
            {
                if (itemBlockID != 0)
                {
                    Block b = Block.GetBlock(itemBlockID);
                    BlockRenderer tex = b.RenderBlock((int)aimingAt.X, (int)aimingAt.Y, sb);
                    if (tex.Texture != null)
                    {
                        sb.Draw(tex.Texture, blockDrawPos - CameraManager.cameraPosition, null, new Color(1f, 1f, 1f, .75f), 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                    }
                }
            }
            else
            {
                DrawManager.DrawOutline(drawBB.Center.ToVector2() - CameraManager.cameraPosition, drawBB.Width,
                                        drawBB.Height, Color.White, sb);
            }

            ConsoleManager.SetVariableValue("window_title", (int)aimingAt.X + ", " + (int)aimingAt.Y);

            int leftX = FacingLeft ? BoundBox.Left - 15 : BoundBox.Right;
            ShapeAABB bound = new ShapeAABB(new Rectangle(leftX, (int)BoundBox.Top + 3, 15, PlayerHeight - 6));

            Vector2 measure = AssetManager.GetFont("Console").MeasureString(PlayerName);
            sb.DrawString(AssetManager.GetFont("Console"), PlayerName, EntityPosition - new Vector2(measure.X / 2, 30) - CameraManager.cameraPosition, Color.White);
            base.Draw(sb);
        }

        public void OnAttack()
        {
            if (EquippedItem == null)
                return;
            if (EquippedItem is ItemSword || EquippedItem is ItemBow)
            {
                TorsoAnimateable.GotoAndStart(TorsoAnimateable.IndexOfFrame("swing_start"));
                TorsoAnimateable.StartLooping("swing_start", "swing_end");
            }
        }

        public short GetAimingAngle()
        {
            Vector2 aim = InputManager.GetMousePosV() + CameraManager.cameraPosition;
            aim -= EntityPosition;
            return (short)Math.Atan2(aim.Y, aim.X).RToD();
        }

        public Vector2 GetBlockAimingAt()
        {
            //if (InputManager.GetMousePosV().Y <= 56) return Vector2.Zero;
            Vector2 tile = GameWorld.AbsoluteToTile(InputManager.GetMousePosV() + CameraManager.cameraPosition);
            if (tile.X >= GameWorld.WorldSizeX || tile.X < 0 || tile.Y >= GameWorld.WorldSizeY || tile.Y < 0) return Vector2.Zero;
            return tile;
        }

        public override void Update(GameTime time)
        {
            _oldPlayerPos = EntityPosition;
            base.Update(time);
        }

    }
}
