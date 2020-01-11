using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train
{
    /// <summary>
    ///  记录故障注入的一些标志
    /// </summary>
    class FaultInjection
    {
        private static bool isVersionCompatible = true ;  // M32 version
        private static bool isNoMessageSent = false ;      // train send no message
        private static _Q_STATUS qStatus = _Q_STATUS.VALID;
        private static bool isNoLevelTr = false;  // do not execute level switch

        public static bool IsVersionCompatible
        {
            get
            {
                return isVersionCompatible;
            }

            set
            {
                isVersionCompatible = value;
            }
        }

        public static bool IsNoMessageSent
        {
            get
            {
                return isNoMessageSent;
            }

            set
            {
                isNoMessageSent = value;
            }
        }

        public static _Q_STATUS QStatus
        {
            get
            {
                return qStatus;
            }

            set
            {
                qStatus = value;
            }
        }

        public static bool IsNoLevelTr
        {
            get
            {
                return isNoLevelTr;
            }

            set
            {
                isNoLevelTr = value;
            }
        }

    }

}
