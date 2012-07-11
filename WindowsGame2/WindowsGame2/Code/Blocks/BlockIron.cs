using MiningGame.Code.Managers;

namespace MiningGame.Code.Blocks
{
    public class BlockIron : Block
    {
        public BlockIron() : base()
        {
            this.SetBlockRenderSpecial(true).SetBlockID(3).SetBlockName("Iron");
        }

        public override BlockRenderer RenderBlock(int x, int y, Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            return new BlockRenderer(AssetManager.GetTexture("iron"));
        }
    }
}
