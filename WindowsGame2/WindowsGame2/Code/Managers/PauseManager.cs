using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YogUILibrary.UIComponents;
using YogUILibrary.Managers;
namespace MiningGame.Code.Managers
{
    public class PauseManager : Manager
    {

        public bool Paused;

        public static TextDrawer PauseText;

        public PauseManager()
        {
            PauseText = new TextDrawer(AssetManager.GetFont("Console"), "- PAUSED -", Vector2.Zero, Color.Black, TextAlign.Center);
            InputManager.BindKey(
                () =>
                {
                    //TogglePaused();
                }, Microsoft.Xna.Framework.Input.Keys.P);
            AddToList();
        }

        public void TogglePaused()
        {
            Paused = !Paused;
        }

        public override void Update(GameTime time)
        {
            PauseText.text = "";
            if (Paused)
                PauseText.text = "- PAUSED -";
            SpriteFont s = AssetManager.GetFont("Console");
            Vector2 center = new Vector2(Main.graphics.PreferredBackBufferWidth / 2, Main.graphics.PreferredBackBufferHeight / 2);
            PauseText.Position = center;
            PauseText.font = s;
            base.Update(time);
        }


    }
}
