using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace MiningGame.Code.CInterfaces
{
    public interface Updatable
    {
        void Update(GameTime time);
        void addToUpdateList();
        void removeFromUpdateList();
    }
}
