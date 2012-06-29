using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.ItemAttributes
{
    public class AttributeNoCrits : ItemAttribute
    {

        public AttributeNoCrits()
        {

        }

        public override int GetID()
        {
            return 2;
        }

        public override string GetName()
        {
            return "player_crit_chance_none";
        }

        public override AttributeAffinity GetAffinity()
        {
            return AttributeAffinity.Negative;
        }

        public override string GetDescription()
        {
            return "No random critical hits";
        }
    }
}
