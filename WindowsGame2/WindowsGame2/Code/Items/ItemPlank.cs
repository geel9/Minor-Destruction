﻿namespace MiningGame.Code.Items
{
    public class ItemPlank : Item
    {
        public ItemPlank()
            : base()
        {
            SetName("Plank").SetDescription("Planks!").SetID(6).SetValue(1).SetAsset("planks").SetBlockID(0);
        }
        public override void OnItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
