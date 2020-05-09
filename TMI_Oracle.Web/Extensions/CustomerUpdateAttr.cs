using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMI.Web
{
    /// <summary>
    /// 自定义特性 属性或者类可用  支持继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class CustomerUpdateAttr : Attribute
    {
        /// <summary>
        /// 是否自动更新
        /// </summary>
        public bool AutoUpdate
        {
            get;
            set;
        }
    }
}