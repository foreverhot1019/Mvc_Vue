﻿



using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Service.Pattern;

using AirOut.Web.Models;
using AirOut.Web.Repositories;
using System.Reflection;
using Repository.Pattern.Ef6;

namespace AirOut.Web.Services
{
    public class DataTableImportMappingService : Service<DataTableImportMapping>, IDataTableImportMappingService
    {
        private readonly IRepositoryAsync<DataTableImportMapping> _repository;
        public DataTableImportMappingService(IRepositoryAsync<DataTableImportMapping> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<EntityInfo> GetAssemblyEntities()
        {
            List<EntityInfo> list = new List<EntityInfo>();

            Assembly asm = Assembly.GetExecutingAssembly();

            list = asm.GetTypes().
                Where(type => typeof(Entity).IsAssignableFrom(type)).
                SelectMany(type => type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)).
                Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).
                Select(x => new EntityInfo { EntitySetName = x.DeclaringType.Name, FieldName = x.Name, FieldTypeName = x.PropertyType.Name, IsRequired = x.GetCustomAttributes().Where(f => f.TypeId.ToString().IndexOf("Required") >= 0).Any() }).
                OrderBy(x => x.EntitySetName).ThenBy(x => x.FieldName).ToList();

            return list;
        }

        public void GenerateByEnityName(string entityName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var list = asm.GetTypes().Where(type => typeof(Entity).IsAssignableFrom(type) && type.Name == entityName)
                   .SelectMany(type => type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                   .Select(x => new EntityInfo
                   {
                       EntitySetName = x.DeclaringType.Name,
                       FieldName = x.Name,
                       FieldTypeName = x.PropertyType.Name,
                       IsRequired = AttributeHelper.GetRequired(x.DeclaringType, x.Name),
                       FieldDesc = AttributeHelper.GetDisplayName(x.DeclaringType, x.Name),
                       FieldMetaDesc = AirOut.Web.Extensions.Common.GetOnlyMetaDataDisplayName(x.DeclaringType, x.Name)
                   }).
                   OrderBy(x => x.EntitySetName).ThenBy(x => x.FieldName).
                   Where(x => x.FieldTypeName != "ICollection`1").ToList();
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

        public DataTableImportMapping FindMapping(string entitySetName, string sourceFieldName)
        {
            return this.Queryable().Where(x => x.EntitySetName == entitySetName && x.SourceFieldName == sourceFieldName).FirstOrDefault();
        }

        public string GetDisplayName(string entitySetName, string fieldName)
        {
            var displayName = this.Queryable().Where(x => x.EntitySetName == entitySetName && x.FieldName == fieldName).Select(x => x.FieldDesc).FirstOrDefault();
            return (string.IsNullOrEmpty(displayName) ? fieldName : displayName);

        }
    }
}



