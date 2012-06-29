using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.ItemAttributes
{
    public class AttributeHealthBuff : ItemAttribute
    {

        public AttributeHealthBuff()
        {

        }

        public override int GetID()
        {
            return 3;
        }

        public override string GetName()
        {
            return "player_health_buff";
        }

        public override AttributeAffinity GetAffinity()
        {
            return AttributeAffinity.Positive;
        }

        public override string GetDescription()
        {
            return "+" + AttributeModifier + "% increase to max health.";
        }
    }
}
