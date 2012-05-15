using Microsoft.Xna.Framework;

namespace MiningGame.Code.Managers
{
    public class Manager
    {
        public string Name;

        public void AddToList()
        {
            Main.managers.Add(this);
        }

        public void RemoveFromList()
        {
            Main.managers.Remove(this);
        }

        public virtual void Update(GameTime time)
        {

        }
    }
}
