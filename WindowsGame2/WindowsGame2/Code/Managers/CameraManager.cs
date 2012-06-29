using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MiningGame.Code.Structs;
using MiningGameServer.Shapes;
using MiningGameServer.Structs;

namespace MiningGame.Code.Managers
{
    public static class CameraManager
    {

        public static Vector2 cameraPosition = new Vector2(0, 0);
        public static Vector2 cameraDimensions{
            get{
                return new Vector2(Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight);
            }
        }

        public static Rectangle cameraBoundBox
        {
            get
            {
                return new Rectangle((int)cameraPosition.X,
                                    (int)cameraPosition.Y,
                                    (int)cameraDimensions.X,
                                    (int)cameraDimensions.Y);
            }
        }

        private static float _cameraZoom = 1f;

        public static float cameraZoom
        {
            get
            {
                return _cameraZoom;
            }
            set
            {
                _cameraZoom = MathHelper.Clamp(value, 0.1f, value);
            }
        }

        public static void moveCamera(float x, float y)
        {
            cameraPosition.X += x;
            cameraPosition.Y += y;
        }

        public static void setCameraPosition(Vector2 position)
        {
            cameraPosition = position;
        }

        public static void setCameraPositionCenter(Vector2 position){
            cameraPosition = position - Main.Center;
        }

        public static void setCameraPositionCenterMin(Vector2 position, Vector2 min)
        {
            cameraPosition = position - Main.Center;
            if (cameraPosition.X < min.X) cameraPosition.X = min.X;
            if (cameraPosition.Y < min.Y) cameraPosition.Y = min.Y;
        }

        public static bool inCamera(Vector2 position, ShapeAABB dimensions)
        {
            Rectangle r = new Rectangle((int)position.X, (int)position.Y, dimensions.Width, dimensions.Height);
            return (r.Intersects(cameraBoundBox) || cameraBoundBox.Contains(r) || r.Contains(cameraBoundBox));
        }
    }
}
