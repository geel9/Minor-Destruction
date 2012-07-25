using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGame.Code.Items;
using MiningGame.Code.Managers;
using MiningGame.Code.Structs;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;
using ItemStack = MiningGame.Code.Items.ItemStack;

namespace MiningGame.Code.Entities
{
    public class EntityDroppedItem : EntityMoveable
    {
        private byte _itemID;

        public PlayerEntity MovingTowards = null;

        public short DroppedItemID;

        public bool ShouldDestroy;
        public bool Incomplete = false;

        private int timeFlying = 0;

        private const int ItemWidth = 10;
        private const int ItemHeight = 10;

        public override ShapeAABB BoundBox
        {
            get
            {
                return new ShapeAABB((int)EntityPosition.X - ItemWidth / 2, (int)EntityPosition.Y - ItemHeight / 2, ItemWidth, ItemHeight);
            }
        }

        private int _timeAlive = 0;

        public override void Update(GameTime time)
        {
            if (MovingTowards == null)
                EntityMovement();

            if (_timeAlive++ < 15)
                return;

            if (MovingTowards == null)
                return;

            MoveTowardsPoint(MovingTowards.EntityPosition, 4);

            if (HitTest(MovingTowards) || ++timeFlying >= 120)
            {
                if (MovingTowards == GameWorld.ThePlayer.PlayerEntity)
                    Main.SoundManager.PlaySound("collectitem");

                ShouldDestroy = !Incomplete;
                MovingTowards = null;
                timeFlying = 0;
            }
            base.Update(time);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Item i = Item.GetItem(_itemID);

            Vector2 drawScale = new Vector2(ItemWidth / (float)SpriteTexture.Width, ItemHeight / (float)SpriteTexture.Height);

            Vector2 minus = new Vector2(SpriteTexture.Width / 2, SpriteTexture.Height / 2) * drawScale;

            sb.Draw(AssetManager.GetTexture(i.GetAsset()), EntityPosition - minus - CameraManager.cameraPosition, null, Color.White, 0f, Vector2.Zero, drawScale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }

        public EntityDroppedItem(Vector2 position, Vector2 velocity, byte itemid, short ID)
        {
            Item i = Item.GetItem(itemid);
            string asset = i != null ? i.GetAsset() : "error";
            EntityPosition = position;
            SpriteTexture = AssetManager.GetTexture(asset);
            _itemID = itemid;
            DroppedItemID = ID;
            EntityVelocity = velocity;
        }
    }
}
