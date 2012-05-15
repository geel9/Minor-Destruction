/*using System;
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
using MiningGame.Code.Structs;
namespace MiningGame.Code.Managers
{
    public class InputManager : Manager
    {
        public InputManager(bool immediate = true)
        {
            base.name = "InputManager";
            if (immediate)
                base.addToList();
        }

        private static KeyboardState KeyboardState;
        private static KeyboardState oldKeyboardState;
        private static MouseState MouseState;
        private static MouseState oldMouseState;
        private int scrollValue = 0;

        private static List<CodeBoundMouse> boundMouse = new List<CodeBoundMouse>();
        private static List<CodeBoundKey> boundKey = new List<CodeBoundKey>();

        public static void BindKey(Action lambda, Keys key, bool constant = false, bool press = true, string identifier = "bind")
        {
            boundKey.Add(new CodeBoundKey(lambda, key, constant, press, identifier));
        }

        public static void BindMouse(Action a, MouseButton button, bool press = true, bool constant = false, string identifier = "bind")
        {
            boundMouse.Add(new CodeBoundMouse(a, button, press, constant, identifier));
        }

        public static Point GetMousePos()
        {
            return new Point(MouseState.X, MouseState.Y);
        }

        public static Vector2 GetMousePosV(bool offset = false)
        {
            return new Vector2(GetMousePos().X, GetMousePos().Y) + (offset ? CameraManager.cameraPosition : Vector2.Zero);
        }

        public override void Update(GameTime time)
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            int scroll = MouseState.ScrollWheelValue - scrollValue;
            scrollValue = MouseState.ScrollWheelValue;
            if (!Main.isActive) return;

            try
            {
                foreach (CodeBoundMouse b in boundMouse)
                {
                    switch (b.boundMouseButton)
                    {
                        case MouseButton.Left:
                            if (MouseState.LeftButton != oldMouseState.LeftButton || b.constant)
                            {
                                if (b.press && MouseState.LeftButton == ButtonState.Pressed)
                                    b.lambda();
                                else if (!b.press && MouseState.LeftButton == ButtonState.Released)
                                    b.lambda();
                            }
                            break;
                        case MouseButton.Middle:
                            if (MouseState.MiddleButton != oldMouseState.MiddleButton || b.constant)
                            {
                                if (b.press && MouseState.MiddleButton == ButtonState.Pressed)
                                    b.lambda();
                                else if (!b.press && MouseState.MiddleButton == ButtonState.Released)
                                    b.lambda();
                            }
                            break;

                        case MouseButton.Right:
                            if (MouseState.RightButton != oldMouseState.RightButton || b.constant)
                            {
                                if (b.press && MouseState.RightButton == ButtonState.Pressed)
                                    b.lambda();
                                else if (!b.press && MouseState.RightButton == ButtonState.Released)
                                    b.lambda();
                            }
                            break;

                        case MouseButton.Scroll:
                            if (scroll != 0)
                                b.lambda();
                            break;

                        case MouseButton.Scrolldown:
                            if (scroll < 0)
                                b.lambda();
                            break;
                        case MouseButton.Scrollup:
                            if (scroll > 0)
                                b.lambda();
                            break;

                        case MouseButton.Movement:
                            if (MouseState.Y != oldMouseState.Y || MouseState.X != oldMouseState.X)
                            {
                                b.lambda();
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }

            foreach (CodeBoundKey b in boundKey)
            {
                    Keys k = b.boundKey;
                    bool newP = KeyboardState.IsKeyDown(k);
                    bool oldP = oldKeyboardState.IsKeyDown(k);
                    bool constant = b.constant;
                    bool press = b.press;
                    
                    if ((newP != oldP || constant) && Main.isActive)
                    {
                        if (press && newP)
                            b.lambda();
                        else if (!press && !newP)
                            b.lambda();
                    }
            }

            oldMouseState = MouseState;
            oldKeyboardState = KeyboardState;
        }

        public static bool isKeyPressed(Keys k)
        {
            if (KeyboardState.IsKeyDown(k))
                return true;
            return false;
        }
    }
}
*/