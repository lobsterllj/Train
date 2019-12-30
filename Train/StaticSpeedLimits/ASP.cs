using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.StaticSpeedLimits
{
    /// <summary>
    /// 轴重速度曲线
    /// </summary>
    public class ASP : StaticSpeedLimit
    {
        private Packets.Packet051 p51;
    }
}
