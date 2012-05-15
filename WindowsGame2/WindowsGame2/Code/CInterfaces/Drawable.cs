using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MiningGame.Code.CInterfaces
{
    public interface Drawable
    {
        void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb);
        void addToDrawList();
        void removeFromDrawList();
        bool inCamera();
    }
}
