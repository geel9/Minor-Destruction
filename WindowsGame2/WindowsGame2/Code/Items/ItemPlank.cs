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
            setName("Plank").setDescription("Planks!").setID(6).setValue(1).setAsset("planks").setBlockID(200);
        }
        public override void onItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
