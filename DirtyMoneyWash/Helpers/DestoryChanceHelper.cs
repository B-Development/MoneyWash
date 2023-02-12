using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyMoneyWash.Helpers
{
    public class DestoryChanceHelper
    {
        private static Random rng = new Random();

        public static bool RollChance(int chance)
        {
            return chance <= rng.Next(1, 101);
        }
    }
}
