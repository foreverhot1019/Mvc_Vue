using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TMI.Web.Extensions
{
    public static class IQueryableExtension
    {
        /// <summary>
        /// For an Entity Framework IQueryable, returns the SQL with inlined Parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string ToTraceQuery<T>(this IQueryable<T> query,bool IsOracle=true,bool NString = true)
        {
            ObjectQuery<T> objectQuery = GetQueryFromQueryable(query);

            var result = objectQuery.ToTraceString();
            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(@"(:p__linq__[\d]{0,})");
            var Num = 0;
            foreach(var matche in rgx.Matches(result)){
                var OMatche = ((System.Text.RegularExpressions.Match)matche);
                var val = OMatche.Value;
                result = result.Substring(0, OMatche.Index + Num) + val + " " + result.Substring(OMatche.Index + val.Length + Num);
                Num++;
            }
            var dtType = typeof(DateTime);
            var itType = typeof(int);
            var dbType = typeof(double);
            var ftType = typeof(float);
            var dcType = typeof(decimal);
            var blType = typeof(bool);

            foreach (var parameter in objectQuery.Parameters)
            {
                var name = (IsOracle?":":"@") + parameter.Name+" ";
                var paramVal = parameter.Value.ToString();
                string value = ""; 
                if (parameter.Value != null)
                {
                    var type = parameter.ParameterType;
                    if (parameter.ParameterType.IsGenericType)
                    {
                        var ArrGenericType = parameter.ParameterType.GetGenericArguments();
                        if (ArrGenericType != null && ArrGenericType.Any())
                        {
                            type = ArrGenericType[0];
                        }
                    }
                    if (type == dtType)
                    {
                        paramVal = ((DateTime)parameter.Value).ToString("yyyy/MM/dd HH:mm:ss");
                        if(!IsOracle)
                            value = "cast('" + paramVal + "' as datetime)";
                        else
                            value = "to_date('" + paramVal + "','yyyy/mm/dd hh24:mi:ss')";
                    }
                    else if (type == itType)
                    {
                        value = paramVal;
                    }
                    else if (type == dbType)
                    {
                        paramVal = ((double)parameter.Value).ToString("#0.#########");
                        value = paramVal;
                    }
                    else if (type == ftType)
                    {
                        paramVal = ((float)parameter.Value).ToString("#0.#########");
                        value = paramVal;
                    }
                    else if (type == dcType)
                    {
                        paramVal = ((decimal)parameter.Value).ToString("#0.#########");
                        value = paramVal;
                    }
                    else if (type == blType)
                    {
                        paramVal = (bool)parameter.Value ? "1" : "0";
                        value = paramVal;
                    }
                    else
                    {
                        value = (NString ? "N" : "") + "'" + paramVal + "'";
                    }
                }
                result = result.Replace(name, value);
            }

            return result;
        }

        /// <summary>
        /// For an Entity Framework IQueryable, returns the SQL and Parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Tuple<string, List<ObjectParameter>> ToTraceString<T>(this IQueryable<T> query)
        {
            ObjectQuery<T> objectQuery = GetQueryFromQueryable(query);
            var ArrIQueryParam = objectQuery.Parameters.ToList();

            var traceString = new StringBuilder();

            traceString.AppendLine(objectQuery.ToTraceString());
            //traceString.AppendLine();
            //foreach (var parameter in objectQuery.Parameters)
            //{
            //    traceString.AppendLine(parameter.Name + " [" + parameter.ParameterType.FullName + "] = " + parameter.Value);
            //}

            //return traceString.ToString();
            return new Tuple<string, List<ObjectParameter>>(traceString.ToString(), ArrIQueryParam);
        }

        /// <summary>
        /// 反射转换ObjectQuery<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        private static System.Data.Entity.Core.Objects.ObjectQuery<T> GetQueryFromQueryable<T>(IQueryable<T> query)
        {
            var internalQueryField = query.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(f => f.Name.Equals("_internalQuery")).FirstOrDefault();
            var internalQuery = internalQueryField.GetValue(query);
            var objectQueryField = internalQuery.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(f => f.Name.Equals("_objectQuery")).FirstOrDefault();
            return objectQueryField.GetValue(internalQuery) as System.Data.Entity.Core.Objects.ObjectQuery<T>;
        }
    }
}