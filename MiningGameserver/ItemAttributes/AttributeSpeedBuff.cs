using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.ItemAttributes
{
    public class AttributeSpeedBuff : ItemAttribute
    {
        public AttributeSpeedBuff()
        {

        }

        public override int GetID()
        {
            return 1;
        }

        public override string GetName()
        {
            return "player_speed_buff";
        }

        public override AttributeAffinity GetAffinity()
        {
            return AttributeAffinity.Positive;
        }

        public override string GetDescription()
        {
            return "+" + AttributeModifier + "% increase to move speed while equipped";
        }

        public override PlayerStatBuff[] GetEquippedBuffs()
        {
            return new PlayerStatBuff[] { new PlayerStatBuff() { MoveSpeed = 3 * (AttributeModifier / 100) } };
        }

        public virtual PlayerStatBuff[] GetInventoryBuffs()
        {
            return new PlayerStatBuff[] { new PlayerStatBuff() { MoveSpeed =  AttributeModifier / 100 }};
        }
    }
}
