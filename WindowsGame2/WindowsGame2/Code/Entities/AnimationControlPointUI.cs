using GeeUI.Managers;
using GeeUI.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Managers;

namespace MiningGame.Code.Entities
{
    public class AnimationControlPointUI
    {
        public Vector2 position;
        public Vector2 absPos
        {
            get
            {
                return (position * scale) + Main.Center;
            }
        }
        public float scale = 1f;
        private Color curColor = Color.Red;
        private const int radius = 5;
        public bool selected;
        private bool dragging;
        public TextFieldView tf;
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
            curColor = new Color(Main.R.Next(0, 256), Main.R.Next(0, 255), Main.R.Next(0, 254));
            selected = false;
            dragging = false;
            this.scale = scale;
            this.position = position / scale;
            Color transparent = new Color(0, 0, 0, 0);
            tf = new TextFieldView(GeeUI.GeeUI.RootView, position - new Vector2(50, radius + 12),
                                   AssetManager.GetFont("TinyText"))
                     {
                         Width = 100,
                         Height = 15
                     };
            tf.Text = name;
            //tf.placeHolderText = "Control point name";
            InputManager.BindMouse(() => { if (mouseInsideCircle) { selected = true; dragging = true; } else { selected = false; } }, MouseButton.Left);
            InputManager.BindMouse(() => { if (dragging) {
               // this.position = (InputManager.GetMousePosV() - Main.center) / scale;
            }
            }, MouseButton.Movement, true, true);
            InputManager.BindMouse(() => { dragging = false; }, MouseButton.Left, false);
        }

        public void Update(GameTime theTime)
        {
            //tf.Update(theTime);
        }

        public void Draw(SpriteBatch sb)
        {
            curColor.B++;
            curColor.R++;
            curColor.G++;
            tf.Position = absPos - new Vector2(50, radius + 15);
            //tf.Draw(sb);
            DrawManager.DrawCircle(absPos, radius, curColor, Color.Black, sb);
        }
    }
}
