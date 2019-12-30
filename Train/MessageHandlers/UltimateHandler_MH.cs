using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Train.Messages;

namespace Train.MessageHandlers
{
    /// <summary>
    /// 责任链的末端，Message应在此类之前被处理
    /// </summary>
    class UltimateHandler_MH : AbstractMessageHandler
    {
        public UltimateHandler_MH(MessageHandler mh) : base(mh) { }
        /// <summary>
        /// 总是返回true
        /// </summary>
        /// <param name="am"></param>
        /// <returns></returns>
        public override bool Solve(AbstractRecvMessage am)
        {
            //应该记录这种异常情况
            return true;
        }
    
    }
}
