using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Utilities
{
    /// <summary>
    /// 变量取非法值时引发此异常
    /// </summary>
    public class InvalidValueException:Exception
    {
        public InvalidValueException(String msg):base(msg)
        {
        }
    }
}
