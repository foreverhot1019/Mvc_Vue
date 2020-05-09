using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;

using TMI.Web.Models;
using TMI.Web.Repositories;
using System.Reflection;
using Repository.Pattern.Ef6;
using TMI.Web.Extensions;

namespace TMI.Web.Services
{
    public class DataTableImportMappingService : Service<DataTableImportMapping>, IDataTableImportMappingService
    {
        private readonly IRepositoryAsync<DataTableImportMapping> _repository;
        public DataTableImportMappingService(IRepositoryAsync<DataTableImportMapping> repository)
            : base(repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 获取所有用户的类
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EntityInfo> GetAssemblyEntities()
        {
            List<EntityInfo> list = new List<EntityInfo>();
            var ArrTypes = (List<Type>)CacheHelper.Get_SetCache(Common.CacheNameS.AllEntityAssembly);
            ArrTypes = ArrTypes.Where(type => typeof(Entity).IsAssignableFrom(type)).ToList();
            foreach (Type TypeModel in ArrTypes)
            {
                var TypeModelFullName = TypeModel.FullName;
                PropertyInfo[] ProptyModel;
                var CacheProptyModel = CacheHelper.GetCache(TypeModelFullName);
                if (CacheProptyModel == null)
                {
                    ProptyModel = TypeModel.GetProperties();
                    CacheHelper.SetCache(TypeModelFullName, ProptyModel);
                }
                else
                    ProptyModel = (PropertyInfo[])CacheProptyModel;
                list.AddRange(ProptyModel.Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).
                Select(x => new EntityInfo
                {
                    EntitySetName = x.DeclaringType.Name,
                    FieldName = x.Name,
                    FieldTypeName = x.PropertyType.Name,
                    IsRequired = AttributeHelper.GetRequired(x.DeclaringType, x.Name),
                    FieldDesc = AttributeHelper.GetDisplayName(x.DeclaringType, x.Name),
                    FieldMetaDesc = TMI.Web.Extensions.Common.GetOnlyMetaDataDisplayName(x.DeclaringType, x.Name)
                }));
            }

            list =list.OrderBy(x => x.EntitySetName).ThenBy(x => x.FieldName).ToList();

            return list;
        }

        /// <summary>
        /// 生成Entity Excel导入配置
        /// </summary>
        /// <param name="entityName"></param>
        public void GenerateByEnityName(string entityName)
        {
            List<EntityInfo> list = new List<EntityInfo>();
            var ArrTypes = (List<Type>)CacheHelper.Get_SetCache(Common.CacheNameS.AllEntityAssembly);
            var ArrTypeModel = ArrTypes.Where(type => typeof(Entity).IsAssignableFrom(type)).ToList().Where(x => x.Name == entityName);
            if (ArrTypeModel != null && ArrTypeModel.Any())
            {
                var TypeModel = ArrTypeModel.FirstOrDefault();
                var TypeModelFullName = TypeModel.FullName;
                PropertyInfo[] ProptyModel;
                var CacheProptyModel = CacheHelper.GetCache(TypeModelFullName);
                if (CacheProptyModel == null)
                {
                    ProptyModel = TypeModel.GetProperties();
                    CacheHelper.SetCache(TypeModelFullName, ProptyModel);
                }
                else
                    ProptyModel = (PropertyInfo[])CacheProptyModel;
                list.AddRange(ProptyModel.Where(m => !m.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), false).Any() && 
                    !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any() && 
                    m.PropertyType.Name != "ICollection`1").
                Select(x => new EntityInfo
                {
                    EntitySetName = x.DeclaringType.Name,
                    FieldName = x.Name,
                    FieldTypeName = x.PropertyType.Name,
                    IsRequired = AttributeHelper.GetRequired(x.DeclaringType, x.Name),
                    FieldDesc = AttributeHelper.GetDisplayName(x.DeclaringType, x.Name),
                    FieldMetaDesc = TMI.Web.Extensions.Common.GetOnlyMetaDataDisplayName(x.DeclaringType, x.Name)
                }));
            }
            list = list.OrderBy(x => x.EntitySetName).ThenBy(x => x.FieldName).ToList();
            foreach (var item in list)
            {
                var exist = this.Queryable().Where(x => x.EntitySetName == item.EntitySetName && x.FieldName == item.FieldName).Any();
                if (!exist)
                {
                    DataTableImportMapping row = new DataTableImportMapping();
                    row.EntitySetName = item.EntitySetName;
                    row.FieldName = item.FieldName;
                    row.IsRequired = item.IsRequired;
                    row.TypeName = item.FieldTypeName;
                    row.FieldDesc = item.FieldDesc;
                    row.SourceFieldName = item.FieldMetaDesc;
                    row.IsEnabled = true;
                    this.Insert(row);
                }
            }
        }

        /// <summary>
        /// 查找 配置
        /// </summary>
        /// <param name="entitySetName"></param>
        /// <param name="sourceFieldName"></param>
        /// <returns></returns>
        public DataTableImportMapping FindMapping(string entitySetName, string sourceFieldName)
        {
            return this.Queryable().Where(x => x.EntitySetName == entitySetName && x.SourceFieldName == sourceFieldName).FirstOrDefault();
        }

        /// <summary>
        /// 获取显示名
        /// </summary>
        /// <param name="entitySetName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetDisplayName(string entitySetName, string fieldName)
        {
            var displayName = this.Queryable().Where(x => x.EntitySetName == entitySetName && x.FieldName == fieldName).Select(x => x.FieldDesc).FirstOrDefault();
            return (string.IsNullOrEmpty(displayName) ? fieldName : displayName);
        }
    }
}



