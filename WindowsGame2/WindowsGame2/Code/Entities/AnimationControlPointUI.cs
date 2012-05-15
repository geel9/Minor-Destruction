using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MiningGame.Code.Managers;
using MiningGame.Code.CInterfaces;
using MiningGame.Code.Structs;
using YogUILibrary.Managers;
using YogUILibrary.UIComponents;

namespace MiningGame.Code.Entities
{
    public class AnimationControlPointUI
    {
        public Vector2 position;
        public Vector2 absPos
        {
            get
            {
                return (position * scale) + Main.center;
            }
        }
        public float scale = 1f;
        private Color curColor = Color.Red;
        private const int radius = 5;
        public bool selected;
        private bool dragging;
        public TextField tf;
        private bool mouseInsideCircle
        {
            get
            {
                float dist = Vector2.DistanceSquared(absPos, InputManager.GetMousePosV());
                return (dist <= radius * radius);
            }
        }

        public AnimationControlPointUI(Vector2 position, string name, float scale)
        {
            curColor = new Color(Main.r.Next(0, 256), Main.r.Next(0, 255), Main.r.Next(0, 254));
            selected = false;
            dragging = false;
            this.scale = scale;
            this.position = position / scale;
            Color transparent = new Color(0, 0, 0, 0);
            tf = new TextField(position - new Vector2(50, radius + 12), 100, 15, Color.Black, AssetManager.GetFont("TinyText"), null);
            tf.SetText(name);
            tf.placeHolderText = "Control point name";
            InputManager.BindMouse(() => { if (mouseInsideCircle) { selected = true; dragging = true; } else { selected = false; } }, MouseButton.Left);
            InputManager.BindMouse(() => { if (dragging) {
               // this.position = (InputManager.GetMousePosV() - Main.center) / scale;
            }
            }, MouseButton.Movement, true, true);
            InputManager.BindMouse(() => { dragging = false; }, MouseButton.Left, false);
        }

        public void Update(GameTime theTime)
        {
            tf.Update(theTime);
        }

        public void Draw(SpriteBatch sb)
        {
            curColor.B++;
            curColor.R++;
            curColor.G++;
            tf.Position = absPos - new Vector2(50, radius + 15);
            tf.Draw(sb);
            DrawManager.Draw_Circle(absPos, radius, curColor, Color.Black, sb);
        }
    }
}
