using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMI.Web.Models
{
    /// <summary>
    /// 暂存 线程
    /// </summary>
    public class ThreadObj
    {
        public ThreadObj(string _ThreadName, System.Threading.Thread __Thread)
        {
            ThreadName = _ThreadName;
            _Thread = __Thread;
        }

        public string ThreadName { get; set; }

        public System.Threading.Thread _Thread { get; set; }

    }
}