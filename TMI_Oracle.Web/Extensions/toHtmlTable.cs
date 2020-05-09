using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace TMI.Web
{
    public static class ListExtensions
    {
        public static string ToHtmlTable<T>(this List<T> listOfClassObjects)
        {
            var ret = string.Empty;

            return listOfClassObjects == null || !listOfClassObjects.Any()
                ? ret
                : "<table>" +
                  listOfClassObjects.First().GetType().GetProperties().Select(p => p.Name).ToList().ToColumnHeaders() +
                  listOfClassObjects.Aggregate(ret, (current, t) => current + t.ToHtmlTableRow()) +
                  "</table>";
        }

        public static string ToColumnHeaders<T>(this List<T> listOfProperties)
        {
            var ret = string.Empty;

            return listOfProperties == null || !listOfProperties.Any()
                ? ret
                : "<tr>" +
                  listOfProperties.Aggregate(ret,
                      (current, propValue) =>
                          current +
                          ("<th style='font-size: 11pt; font-weight: bold; border: 1pt solid black'>" +
                           (Convert.ToString(propValue).Length <= 100
                               ? Convert.ToString(propValue)
                               : Convert.ToString(propValue).Substring(0, 100)) + "..." + "</th>")) +
                  "</tr>";
        }

        public static string ToHtmlTableRow<T>(this T classObject)
        {
            var ret = string.Empty;

            return classObject == null
                ? ret
                : "<tr>" +
                  classObject.GetType()
                      .GetProperties()
                      .Aggregate(ret,
                          (current, prop) =>
                              current + ("<td style='font-size: 11pt; font-weight: normal; border: 1pt solid black'>" +
                                         (Convert.ToString(prop.GetValue(classObject, null)).Length <= 100
                                             ? Convert.ToString(prop.GetValue(classObject, null))
                                             : Convert.ToString(prop.GetValue(classObject, null)).Substring(0, 100) +
                                               "...") +
                                         "</td>")) + "</tr>";
        }

        public static string ToHtmlTable(this DataTable dt, Func<DataRow, bool> rowHiglithRule)
        {

            if (dt == null) throw new ArgumentNullException("dt");

            string tab = "\t";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + tab + "<table>");

            // headers.
            sb.Append(tab + tab + tab + "<thead><tr>");

            foreach (DataColumn dc in dt.Columns)
            {
                sb.AppendFormat("<td>{0}</td>", dc.ColumnName);
            }

            sb.AppendLine("</thead></tr>");

            // data rows
            foreach (DataRow dr in dt.Rows)
            {
                if (rowHiglithRule != null)
                {

                    if (rowHiglithRule(dr))
                    {
                        sb.Append(tab + tab + tab + "<tr class=\"highlightedRow\">");
                    }
                    else
                    {
                        sb.Append(tab + tab + tab + "<tr>");
                    }
                }
                else
                {
                    //Non ho alcuna regola, quindi caso normale.
                    sb.Append(tab + tab + tab + "<tr>");
                }

                foreach (DataColumn dc in dt.Columns)
                {
                    string cellValue = dr[dc] != null ? dr[dc].ToString() : "";
                    sb.AppendFormat("<td>{0}</td>", cellValue);
                }

                sb.AppendLine("</tr>");
            }

            sb.AppendLine(tab + tab + "</table>");


            return sb.ToString();
        }
    }
}