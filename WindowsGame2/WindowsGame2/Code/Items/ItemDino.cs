using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Entities;
using MiningGame.Code.Server;
using Microsoft.Xna.Framework.Graphics;
namespace MiningGame.Code.Items
{
    public class ItemDino : Item
    {
        public ItemDino()
            : base()
        {
            setName("Dinosaur").setDescription("Open the door!").setID(7).setValue(1).setAsset("dinosaur").setBlockID(201);
        }
        public override void onItemUsed(int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
