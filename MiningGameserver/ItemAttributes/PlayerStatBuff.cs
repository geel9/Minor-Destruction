using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGameServer.ItemAttributes
{
    public class PlayerStatBuff
    {
        public double MoveSpeed;
        public double AttackSpeed;
        public double MaxHealth;
        public double AttackStrength;

        public bool EqualsBuff(PlayerStatBuff other)
        {
            return other.MoveSpeed == MoveSpeed && other.AttackSpeed == AttackSpeed && other.MaxHealth == MaxHealth &&
                   other.AttackStrength == AttackStrength;
        }
    }
}
