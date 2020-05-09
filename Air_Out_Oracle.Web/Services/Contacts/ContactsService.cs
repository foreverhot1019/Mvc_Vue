using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;

using AirOut.Web.Models;
using AirOut.Web.Repositories;

using System.Data;
using System.Reflection;

using Newtonsoft.Json;
using AirOut.Web.Extensions;
using System.IO;

namespace AirOut.Web.Services
{
    public class ContactsService : Service<Contacts>, IContactsService
    {
        private readonly IRepositoryAsync<Contacts> _repository;
        private readonly IDataTableImportMappingService _mappingservice;
        public ContactsService(IRepositoryAsync<Contacts> repository, IDataTableImportMappingService mappingservice)
            : base(repository)
        {
            _repository = repository;
            _mappingservice = mappingservice;
        }

        public void ImportDataTable(System.Data.DataTable datatable)
        {
            var mapping = _mappingservice.Queryable().Where(x => x.EntitySetName == "Contacts").ToList();
            foreach (DataRow row in datatable.Rows)
            {
                Contacts item = new Contacts();
                foreach (var field in mapping)
                {
                    var defval = field.DefaultValue;
                    var contation = datatable.Columns.Contains((field.SourceFieldName == null ? "" : field.SourceFieldName));
                    if (contation && row[field.SourceFieldName] != DBNull.Value)
                    {
                        Type contactstype = item.GetType();
                        PropertyInfo propertyInfo = contactstype.GetProperty(field.FieldName);
                        propertyInfo.SetValue(item, Convert.ChangeType(row[field.SourceFieldName], propertyInfo.PropertyType), null);
                    }
                    else if (!string.IsNullOrEmpty(defval))
                    {
                        Type contactstype = item.GetType();
                        PropertyInfo propertyInfo = contactstype.GetProperty(field.FieldName);
                        if (defval.ToLower() == "now" && propertyInfo.PropertyType == typeof(DateTime))
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(DateTime.Now, propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(defval, propertyInfo.PropertyType), null);
                        }
                    }
                }

                this.Insert(item);
            }
        }

        public Stream ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            var contacts = this.Query(new ContactsQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
            var datarows = contacts.Select(n => new
            {
                Id = n.Id,
                CusBusInfoId = n.CusBusInfoId,
                CompanyName = n.CompanyName,
                CompanyCode = n.CompanyCode,
                CoAddress = n.CoAddress,
                CoArea = n.CoArea,
                CoCountry = n.CoCountry,
                Contact = n.Contact,
                ContactInfo = n.ContactInfo,
                Status = n.Status,
                Remark = n.Remark,
                OperatingPoint = n.OperatingPoint
            }).ToList();

            return ExcelHelper.ExportExcel(typeof(Contacts), datarows);
        }

        /// <summary>
        /// 新增 客户联系人
        /// </summary>
        /// <param name="ContactInfo">联系信息</param>
        /// <param name="ContactType">类型</param>
        /// <param name="CusBusInfoId">客户ID</param>
        public void InsertContact(string ContactInfo, string ContactType, string CusBusInfoId)
        {
            if (!(string.IsNullOrWhiteSpace(ContactInfo) || string.IsNullOrWhiteSpace(CusBusInfoId)))
            {
                var rgx = new System.Text.RegularExpressions.Regex("\\r|\\n| ");
                var NewShipper_M = rgx.Replace(ContactInfo, "");
                if (NewShipper_M.Length > 0)
                {
                    if (string.IsNullOrWhiteSpace(ContactType))
                        ContactType = "-";
                    #region 新增数据

                    //主单/分单 发货人 收货人 通知人
                    var cantact_m = _repository.Queryable().AsNoTracking().Where(x => x.ContactInfo == ContactInfo && x.ContactType == ContactType && x.CusBusInfoId == CusBusInfoId).FirstOrDefault();
                    if (cantact_m == null || cantact_m.Id <= 0)
                    {
                        var newcantact = new Contacts();
                        newcantact.CusBusInfoId = CusBusInfoId;
                        newcantact.ContactInfo = ContactInfo;
                        newcantact.ContactType = ContactType;
                        //var Arr_H = item.Shipper_H.Split("\\r\\n");\
                        // 截取联系 电话-地址 等信息
                        SetMsgByContactInfo(ref newcantact);
                        if (newcantact.ContactInfo.Length > 2000)
                            newcantact.ContactInfo = newcantact.ContactInfo.Substring(0, 2000);
                        this.Insert(newcantact);
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// 截取联系 电话-地址 等信息
        /// </summary>
        /// <param name="?"></param>
        private void SetMsgByContactInfo(ref Contacts OContacts)
        {
            try
            {
                var rgx = new System.Text.RegularExpressions.Regex("(.*)\\n");//换行截取
                var rgxRplc = new System.Text.RegularExpressions.Regex("\\r");//替换 \r 符号
                var rgxNumber = new System.Text.RegularExpressions.Regex(@"[+]?\d+(\([0-9]+\))?([-])?\d+([-]{1}\d+)?");//获取 电话号码
                var ArrMatch = rgx.Matches(OContacts.ContactInfo);
                List<string> ArrMsgInfo = new List<string>();
                int lastMatchIndex = 0;//最后一个匹配，最后一行 没有 换行符
                foreach (System.Text.RegularExpressions.Match match in ArrMatch)
                {
                    var ArrMatchGp = match.Groups;
                    if (ArrMatchGp.Count > 0)
                    {
                        var gpVal = match.Groups[ArrMatchGp.Count - 1].Value;
                        //最后一个匹配，最后一行 没有 换行符
                        lastMatchIndex = match.Index + match.Value.Length;
                        ArrMsgInfo.Add(rgxRplc.Replace(gpVal, ""));
                    }
                }
                if (lastMatchIndex < OContacts.ContactInfo.Length)
                {
                    ArrMsgInfo.Add(OContacts.ContactInfo.Substring(lastMatchIndex));
                }
                int Num = ArrMsgInfo.Count;
                if (Num >= 2)
                {
                    OContacts.CompanyName = ArrMsgInfo[0];
                    OContacts.CoAddress = ArrMsgInfo[1];
                    if (Num > 2)
                    {
                        var rgxContactMan = new System.Text.RegularExpressions.Regex("([a-zA-Z]+[ ]?[a-zA-Z]+)");//获取 联系人
                        for (var i = 2; i < Num; i++)
                        {
                            var TelStr = ArrMsgInfo[i];
                            if (i == 2)
                                OContacts.Contact = TelStr;//联系电话
                            var ArrMatchNum = rgxContactMan.Matches(TelStr);
                            foreach (System.Text.RegularExpressions.Match matchNum in ArrMatchNum)
                            {
                                var ArrMatchGp = matchNum.Groups;
                                var NumValStr = ArrMatchGp[0].Value;
                                if (matchNum.Index >= 0)
                                {
                                    if ((matchNum.Index + matchNum.Length + 1) < TelStr.Length)
                                    {
                                        var valStr = TelStr.Substring(matchNum.Index, matchNum.Length + 1);
                                        if (valStr.IndexOf(":") < 0)
                                            OContacts.ContactWHO += NumValStr;
                                    }
                                }
                            }
                            if (OContacts.Contact.Length < 100)
                                OContacts.Contact += " " + TelStr;
                        }
                        if (OContacts.ContactWHO.Length > 50)
                            OContacts.ContactWHO = OContacts.ContactWHO.Substring(0, 50);
                        if (OContacts.Contact.Length > 100)
                            OContacts.Contact = OContacts.Contact.Substring(0, 100);
                    }
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                Common.WriteLog_Local(ErrMsg, "ContactServ", true, true);
            }
        }
    }
}