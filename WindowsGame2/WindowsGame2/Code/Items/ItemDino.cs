namespace MiningGame.Code.Items
{
    public class ItemDino : Item
    {
        public ItemDino()
            : base()
        {
            SetName("Dinosaur").SetDescription("Open the door!").SetID(7).SetValue(1).SetAsset("dinosaur").SetBlockID(201);
        }
        public override void OnItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
