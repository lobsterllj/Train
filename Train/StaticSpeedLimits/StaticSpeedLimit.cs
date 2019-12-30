using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.StaticSpeedLimits
{
    public abstract class StaticSpeedLimit
    {
        protected static double maxSpeedLimit = 600;
        public static double GetMaxSpeedLimit()
        {
            return maxSpeedLimit / 3.6;
        }
    }
}
