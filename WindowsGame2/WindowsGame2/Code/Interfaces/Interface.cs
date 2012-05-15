using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MiningGame.Code.Managers;
namespace MiningGame.Code.Interfaces
{
    public class Interface
    {

        public int depth = 0;
        public bool active = true;
        public bool blocking = true;
        public Vector2 Position;

        public Interface()
        {
        }

        public void initialize(int depth = 0)
        {
            this.depth = depth;
            Main.interfaces.Add(this);
            InterfaceManager.shownInterfaces.Add(this);
        }

        public virtual void destroy()
        {
            active = false;
            Main.interfaces.Remove(this);
            InterfaceManager.shownInterfaces.Remove(this);
        }

        public virtual void Update(GameTime time)
        {

        }

        public virtual void Draw(SpriteBatch sb)
        {

        }



    }
}
