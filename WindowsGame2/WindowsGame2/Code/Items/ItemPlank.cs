using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Entities;
using MiningGame.Code.Server;
using Microsoft.Xna.Framework.Graphics;
namespace MiningGame.Code.Items
{
    public class ItemPlank : Item
    {
        public ItemPlank()
            : base()
        {
            SetName("Plank").SetDescription("Planks!").SetID(6).SetValue(1).SetAsset("planks").SetBlockID(200);
        }
        public override void OnItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
