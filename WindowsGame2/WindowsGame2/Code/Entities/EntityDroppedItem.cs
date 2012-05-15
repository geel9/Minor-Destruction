using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGame.Code.Managers;

namespace MiningGame.Code.Entities
{
    public class EntityDroppedItem : EntityMoveable
    {

        private byte itemID;

        private int itemWidth = 10, itemHeight = 10;

        public override Microsoft.Xna.Framework.Rectangle BoundBox
        {
            get
            {
                return new Rectangle((int)entityPosition.X - itemWidth / 2, (int)entityPosition.Y - itemHeight / 2, itemWidth, itemHeight);
            }
        }

        private int timeAlive = 0;

        public override void Update(GameTime time)
        {
            entityMovement();
            float dist = 0;
            if (timeAlive++ < 15) return;
            if (timeAlive >= 3600) removeFromList();
            dist = Vector2.DistanceSquared(entityPosition, GameWorld.thePlayer.playerEntity.entityPosition);
            if (dist <= 500)
            {
                MoveTowardsPoint(GameWorld.thePlayer.playerEntity.entityPosition, 2);
            }
            if (this.hitTest(GameWorld.thePlayer.playerEntity))
            {
                GameWorld.thePlayer.PickupItem(new ItemStack(1, itemID));
                this.removeFromList();
                Main.SoundManager.PlaySound("collectitem");
            }
            base.Update(time);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Item i = Item.getItem(itemID);

            Vector2 scale = new Vector2((float)itemWidth / (float)SpriteTexture.Width, (float)itemHeight / (float)SpriteTexture.Height);

            Vector2 minus = new Vector2(SpriteTexture.Width / 2, SpriteTexture.Height / 2) * scale;

            float s = (float)Math.Sin(timeAlive / 4) * 3;
            minus.Y += s;

            sb.Draw(AssetManager.GetTexture(i.getAsset()), entityPosition - minus - CameraManager.cameraPosition, null, Color.White, 0f, Vector2.Zero, scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            base.Draw(sb);
        }

        public EntityDroppedItem(int x, int y, byte itemid)
        {
           Item i = Item.getItem(itemid);
            base.entityPosition = new Vector2(x, y);
            base.SpriteTexture = AssetManager.GetTexture(i.getAsset());
            entityVelocity.Y = -5 - Main.r.Next(0, 3);
            entityVelocity.X = Main.r.Next(1, 7);
            entityVelocity.X *= (Main.r.Next(0, 2) == 1) ? 1 : -1;
            this.itemID = itemid;
        }

    }
}
