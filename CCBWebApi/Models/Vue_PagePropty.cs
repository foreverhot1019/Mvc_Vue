using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CCBWebApi.Models
{
    /// <summary>
    /// vue前端页面 生成Field属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Vue_PageProptyAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Sortable">可排序table-column</param>
        /// <param name="Editable">可编辑form</param>
        public Vue_PageProptyAttribute()
        {
            Sortable = true;
            Editable = true;
            SearchShow = false;
            FormShow = true;
            ListShow = true;
            IsForeignKey = false;
            ForeignKeyGetListUrl = "";
        }

        /// <summary>
        /// 列表-列宽度
        /// 默认 100
        /// <=0 默认*，>0 此宽度为准
        /// </summary>
        public int? Width_List { get; set; }

        /// <summary>
        /// Form-input宽度
        /// 默认 168
        /// <=0 默认*，>0 此宽度为准
        /// </summary>
        public int? Width_input { get; set; }

        /// <summary>
        /// 可排序（form）
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// 可编辑（table-column）
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// 搜索中展示
        /// </summary>
        public bool SearchShow { get; set; }

        /// <summary>
        /// Form中展示
        /// </summary>
        public bool FormShow { get; set; }

        /// <summary>
        /// 列表展示
        /// </summary>
        public bool ListShow { get; set; }

        /// <summary>
        /// 列表排序
        /// </summary>
        public int ListOrder { get; set; }

        /// <summary>
        /// Search排序
        /// </summary>
        public int SearchOrder { get; set; }

        /// <summary>
        /// Form排序
        /// </summary>
        public int FormOrder { get; set; }

        /// <summary>
        /// 外键关联
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// 外键关联 获取数据Url
        /// </summary>
        public string ForeignKeyGetListUrl { get; set; }

    }
}