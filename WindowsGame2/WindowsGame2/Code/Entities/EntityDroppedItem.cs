using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGame.Code.Items;
using MiningGame.Code.Managers;
using MiningGame.Code.Structs;

namespace MiningGame.Code.Entities
{
    public class EntityDroppedItem : EntityMoveable
    {

        private byte _itemID;

        private const int ItemWidth = 10;
        private const int ItemHeight = 10;

        public override AABB BoundBox
        {
            get
            {
                return new AABB((int)EntityPosition.X - ItemWidth / 2, (int)EntityPosition.Y - ItemHeight / 2, ItemWidth, ItemHeight);
            }
        }

        private int _timeAlive = 0;

        public override void Update(GameTime time)
        {
            EntityMovement();
            float dist = 0;
            if (_timeAlive++ < 15) return;
            if (_timeAlive >= 3600) removeFromList();
            dist = Vector2.DistanceSquared(EntityPosition, GameWorld.ThePlayer.PlayerEntity.EntityPosition);
            if (dist <= 500)
            {
                MoveTowardsPoint(GameWorld.ThePlayer.PlayerEntity.EntityPosition, 2);
            }
            if (HitTest(GameWorld.ThePlayer.PlayerEntity))
            {
                GameWorld.ThePlayer.PickupItem(new ItemStack(1, _itemID));
                removeFromList();
                Main.SoundManager.PlaySound("collectitem");
            }
            base.Update(time);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Item i = Item.GetItem(_itemID);

            Vector2 drawScale = new Vector2(ItemWidth / (float)SpriteTexture.Width, ItemHeight / (float)SpriteTexture.Height);

            Vector2 minus = new Vector2(SpriteTexture.Width / 2, SpriteTexture.Height / 2) * drawScale;

            float s = (float)Math.Sin(_timeAlive / 4) * 3;
            minus.Y += s;

            sb.Draw(AssetManager.GetTexture(i.GetAsset()), EntityPosition - minus - CameraManager.cameraPosition, null, Color.White, 0f, Vector2.Zero, drawScale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            base.Draw(sb);
        }

        public EntityDroppedItem(int x, int y, byte itemid)
        {
            Item i = Item.GetItem(itemid);
            EntityPosition = new Vector2(x, y);
            SpriteTexture = AssetManager.GetTexture(i.GetAsset());
            EntityVelocity.Y = -5 - Main.r.Next(0, 3);
            EntityVelocity.X = Main.r.Next(1, 7);
            EntityVelocity.X *= (Main.r.Next(0, 2) == 1) ? 1 : -1;
            _itemID = itemid;
        }

    }
}
