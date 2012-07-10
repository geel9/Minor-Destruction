using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public struct ConCommand
    {
        public Action<string[]> Lambda;
        public string Description;
        public string Name;
        public long Flags;

        public int[] PossibleArgNums;
        public bool HasArgs
        {
            get { return PossibleArgNums != null && PossibleArgNums.Length > 0; }
        }

        public bool IsInArgNums(int num)
        {
            return PossibleArgNums.Contains(num);
        }

        public void SetArgNumOne(int num)
        {
            PossibleArgNums = new int[1] { num };
        }

        public void SetArgNumMinMax(int min, int max)
        {
            int num = (max - min) + 1;
            PossibleArgNums = new int[num];
            for(int i = 0; i < num; i++)
            {
                PossibleArgNums[i] = min + i;
            }
        }

        public void SetArgNumMultiple(int[] num)
        {
            PossibleArgNums = num;
        }

        public ConCommand(Action<string[]> lambda, string description, string name, long flags = 0)
        {
            this.Lambda = lambda;
            this.Description = description;
            this.Name = name;
            this.Flags = flags;
            PossibleArgNums = new int[0];
        }
    }
}
