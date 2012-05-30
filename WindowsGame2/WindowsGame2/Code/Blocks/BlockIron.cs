using MiningGame.Code.Managers;

namespace MiningGame.Code.Blocks
{
    public class BlockIron : Block
    {
        public BlockIron() : base()
        {
            this.SetBlockRenderSpecial(true).SetBlockID(5).SetBlockName("Iron");
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D RenderBlock(int x, int y, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            return AssetManager.GetTexture("iron");
        }
    }
}
