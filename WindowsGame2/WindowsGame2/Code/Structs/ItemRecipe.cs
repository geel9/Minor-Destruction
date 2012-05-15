using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Items;

namespace MiningGame.Code.Structs
{
    public class ItemRecipe
    {
        public List<ItemStack> requiredItems = new List<ItemStack>();
        public ItemStack returnItems = new ItemStack();
        public string recipeName = "abcdefg";
    }
}
