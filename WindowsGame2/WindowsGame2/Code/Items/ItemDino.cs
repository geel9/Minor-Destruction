namespace MiningGame.Code.Items
{
    public class ItemDino : Item
    {
        public ItemDino()
            : base()
        {
            SetName("Piston").SetDescription("Very pistony").SetID(7).SetValue(1).SetAsset("pistonbase").SetBlockID(5);
        }
        public override void OnItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
