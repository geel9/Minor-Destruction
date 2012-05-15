using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace MiningGame.Code.CInterfaces
{
    public interface UpdatableAndDrawable : Updatable, Drawable
    {
        void addToList();
        void removeFromList();
    }
}
