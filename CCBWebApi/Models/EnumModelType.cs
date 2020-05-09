using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    public class EnumModelType
    {
        public EnumModelType()
        {

        }

        /// <summary>
        /// Session/Cache 枚举键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Session/Cache 枚举键值 如果有的话
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Session/Cache 枚举键显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Session/Cache 枚举键显示描述
        /// </summary>
        public string DisplayDescription { get; set; }

    }
}