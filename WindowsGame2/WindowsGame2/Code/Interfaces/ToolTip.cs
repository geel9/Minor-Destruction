using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.ExtensionMethods;
namespace MiningGame.Code.Interfaces
{
    public class ToolTip : Interface
    {
        /*
        private const int widthAdd = 10;
        private const int heightAdd = 10;
        private string text = "";
        private TextDrawer tipText;
        private Rectangle displayBounds;
        private Rectangle BoundBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, tipText.BoundBox.Width, tipText.BoundBox.Height);
            }
        }

        public ToolTip(string text, Rectangle bounds)
        {
            this.text = text;
            this.displayBounds = bounds;
            Vector2 mousePos = InputManager.GetMousePos().ToVector2();
            tipText = new TextDrawer(AssetManager.GetFont("Console"), text, mousePos, Color.White, TextAlign.Center);
        }

        public override void Update(GameTime time)
        {
            tipText.text = text;
            if (!displayBounds.Contains(InputManager.GetMousePos()))
            {
                destroy();
            }
            base.Update(time);
        }

        public override void Draw(SpriteBatch sb)
        {
            Vector2 center = InputManager.GetMousePos().ToVector2();
            center.Y -= BoundBox.Height / 2;
            DrawManager.Draw_Box(center, BoundBox.Width + (widthAdd / 2), BoundBox.Height, Color.Black, sb, 0f, 200);
            DrawManager.Draw_Outline(center, BoundBox.Width + (widthAdd / 2), BoundBox.Height, Color.White, sb, 255);
            tipText.Position = center;
            tipText.Draw(sb);
            base.Draw(sb);
        }


        public void SetText(string text)
        {
            this.text = text;
        }

        public string GetText()
        {
            return text;
        }*/
    }
}
