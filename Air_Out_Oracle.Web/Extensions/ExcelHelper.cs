using AirOut.Web.Extensions;
using Aspose.Cells;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AirOut.Web
{
    public class ExcelHelper
    {
        public ExcelHelper()
        {
            Aspose.Cells.License license = new Aspose.Cells.License();
            SetAsposeLicense(license);
        }

        public static void SetAsposeLicense(Aspose.Cells.License license)
        {
            //string strLic = @"<License>
            //                    <Data>
            //                      <SerialNumber>aed83727-21cc-4a91-bea4-2607bf991c21</SerialNumber>
            //                      <EditionType>Enterprise</EditionType>
            //                      <Products>
            //                        <Product>Aspose.Total</Product>
            //                      </Products>
            //                    </Data>
            //                    <Signature>CxoBmxzcdRLLiQi1kzt5oSbz9GhuyHHOBgjTf5w/wJ1V+lzjBYi8o7PvqRwkdQo4tT4dk3PIJPbH9w5Lszei1SV/smkK8SCjR8kIWgLbOUFBvhD1Fn9KgDAQ8B11psxIWvepKidw8ZmDmbk9kdJbVBOkuAESXDdtDEDZMB/zL7Y=</Signature>
            //                  </License>";

            //MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(strLic));
            //license.SetLicense(ms);

            string path = HttpContext.Current.Server.MapPath("/Aspose/License.lic");
            license.SetLicense(path);
        }

        public static List<T> getClassFromExcel<T>(string path) where T : class
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                List<T> retList = new List<T>();

                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                bool hasHeader = true; // adjust it accordingly( i've mentioned that this is a simple approach)
                var fielddic = new Dictionary<string, int>();
                int idx = 0;
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    string field = (hasHeader ? firstRowCell.Text : string.Format("Column{0}", firstRowCell.Start.Column));
                    fielddic.Add(field, idx++);
                }
                var startRow = hasHeader ? 2 : 1;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    T objT = Activator.CreateInstance<T>();
                    Type myType = typeof(T);
                    PropertyInfo[] myProp = myType.GetProperties();

                    for (int i = 0; i < myProp.Count(); i++)
                    {
                        int colidx = fielddic[myProp[i].Name];
                        myProp[i].SetValue(objT, wsRow[rowNum, colidx + 1].Text);
                    }
                    retList.Add(objT);
                }
                return retList;
            }
        }

        public static DataTable GetDataTableFromExcel(string path)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                bool hasHeader = true; // adjust it accordingly( i've mentioned that this is a simple approach)
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    tbl.Rows.Add(row);
                }
                return tbl;
            }
        }

        public static DataTable GetDataTableFromExcel(Stream filestream)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                pck.Load(filestream);
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                bool hasHeader = true; // adjust it accordingly( i've mentioned that this is a simple approach)
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    tbl.Rows.Add(row);
                }
                return tbl;
            }
        }

        public static string ExcelContentType { get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; } }

        public static DataTable ToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                //table.Columns.Add(prop.Name, prop.PropertyType);
                string displayName = AttributeHelper.GetDisplayName(data.First(), prop.Name);
                table.Columns.Add(displayName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType); // to avoid nullable types
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }

                table.Rows.Add(values);
            }
            return table;
        }

        public static byte[] ExportExcel(DataTable dt, string Heading = "", params string[] IgnoredColumns)
        {
            byte[] result = null;
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Exported Data");
                int StartFromRow = String.IsNullOrEmpty(Heading) ? 1 : 3;

                // add the content into the Excel file
                ws.Cells["A" + StartFromRow].LoadFromDataTable(dt, true);

                // autofit width of cells with small content
                int colindex = 1;
                foreach (DataColumn col in dt.Columns)
                {
                    ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, colindex, ws.Dimension.End.Row, colindex];
                    int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                    if (maxLength < 150)
                        ws.Column(colindex).AutoFit();

                    colindex++;
                }

                // format header - bold, yellow on black
                using (ExcelRange r = ws.Cells[StartFromRow, 1, StartFromRow, dt.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Yellow);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                }

                // format cells - add borders
                using (ExcelRange r = ws.Cells[StartFromRow + 1, 1, StartFromRow + dt.Rows.Count, dt.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // removed ignored columns
                for (int i = dt.Columns.Count - 1; i >= 0; i--)
                {
                    if (IgnoredColumns.Contains(dt.Columns[i].ColumnName))
                    {
                        ws.DeleteColumn(i + 1);
                    }
                }

                // add header and an additional column (left) and row (top)
                if (!String.IsNullOrEmpty(Heading))
                {
                    ws.Cells["A1"].Value = Heading;
                    ws.Cells["A1"].Style.Font.Size = 20;

                    ws.InsertColumn(1, 1);
                    ws.InsertRow(1, 1);
                    ws.Column(1).Width = 5;
                }

                result = pck.GetAsByteArray();
            }

            return result;
        }

        public static byte[] ExportExcel(DataSet ds, string Heading = "", params string[] IgnoredColumns)
        {
            byte[] result = null;
            using (ExcelPackage pck = new ExcelPackage())
            {
                foreach (DataTable dt in ds.Tables)
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Exported Data");
                    int StartFromRow = String.IsNullOrEmpty(Heading) ? 1 : 3;

                    // add the content into the Excel file
                    ws.Cells["A" + StartFromRow].LoadFromDataTable(dt, true);

                    // autofit width of cells with small content
                    int colindex = 1;
                    foreach (DataColumn col in dt.Columns)
                    {
                        ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, colindex, ws.Dimension.End.Row, colindex];
                        int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                        if (maxLength < 150)
                            ws.Column(colindex).AutoFit();

                        colindex++;
                    }

                    // format header - bold, yellow on black
                    using (ExcelRange r = ws.Cells[StartFromRow, 1, StartFromRow, dt.Columns.Count])
                    {
                        r.Style.Font.Color.SetColor(System.Drawing.Color.Yellow);
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                    }

                    // format cells - add borders
                    using (ExcelRange r = ws.Cells[StartFromRow + 1, 1, StartFromRow + dt.Rows.Count, dt.Columns.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    }

                    // removed ignored columns
                    for (int i = dt.Columns.Count - 1; i >= 0; i--)
                    {
                        if (IgnoredColumns.Contains(dt.Columns[i].ColumnName))
                        {
                            ws.DeleteColumn(i + 1);
                        }
                    }

                    // add header and an additional column (left) and row (top)
                    if (!String.IsNullOrEmpty(Heading))
                    {
                        ws.Cells["A1"].Value = Heading;
                        ws.Cells["A1"].Style.Font.Size = 20;

                        ws.InsertColumn(1, 1);
                        ws.InsertRow(1, 1);
                        ws.Column(1).Width = 5;
                    }
                }

                result = pck.GetAsByteArray();
            }

            return result;
        }

        public static byte[] ExportExcel<T>(List<T> data, string Heading = "", params string[] IgnoredColumns)
        {
            return ExportExcel(ToDataTable<T>(data), Heading, IgnoredColumns);
        }

        public static Stream ExportExcel(DataTable dt, params string[] IgnoredColumns)
        {
            string Heading = "";
            var stream = new MemoryStream();
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Exported Data");
                int StartFromRow = String.IsNullOrEmpty(Heading) ? 1 : 3;

                // add the content into the Excel file
                ws.Cells["A" + StartFromRow].LoadFromDataTable(dt, true);

                // autofit width of cells with small content
                int colindex = 1;
                foreach (DataColumn col in dt.Columns)
                {
                    ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, colindex, ws.Dimension.End.Row, colindex];
                    int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                    if (maxLength < 150)
                        ws.Column(colindex).AutoFit();

                    colindex++;
                }

                // format header - bold, yellow on black
                using (ExcelRange r = ws.Cells[StartFromRow, 1, StartFromRow, dt.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Yellow);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                }

                // format cells - add borders
                using (ExcelRange r = ws.Cells[StartFromRow + 1, 1, StartFromRow + dt.Rows.Count, dt.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // removed ignored columns
                for (int i = dt.Columns.Count - 1; i >= 0; i--)
                {
                    if (IgnoredColumns.Contains(dt.Columns[i].ColumnName))
                    {
                        ws.DeleteColumn(i + 1);
                    }
                }

                // add header and an additional column (left) and row (top)
                if (!String.IsNullOrEmpty(Heading))
                {
                    ws.Cells["A1"].Value = Heading;
                    ws.Cells["A1"].Style.Font.Size = 20;

                    ws.InsertColumn(1, 1);
                    ws.InsertRow(1, 1);
                    ws.Column(1).Width = 5;
                }

                pck.SaveAs(stream);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static Stream ExportExcel(DataSet ds, params string[] IgnoredColumns)
        {
            string Heading = "";
            var stream = new MemoryStream();
            using (ExcelPackage pck = new ExcelPackage())
            {
                foreach (DataTable dt in ds.Tables)
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Exported Data");
                    int StartFromRow = String.IsNullOrEmpty(Heading) ? 1 : 3;

                    // add the content into the Excel file
                    ws.Cells["A" + StartFromRow].LoadFromDataTable(dt, true);

                    // autofit width of cells with small content
                    int colindex = 1;
                    foreach (DataColumn col in dt.Columns)
                    {
                        ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, colindex, ws.Dimension.End.Row, colindex];
                        int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                        if (maxLength < 150)
                            ws.Column(colindex).AutoFit();

                        colindex++;
                    }

                    // format header - bold, yellow on black
                    using (ExcelRange r = ws.Cells[StartFromRow, 1, StartFromRow, dt.Columns.Count])
                    {
                        r.Style.Font.Color.SetColor(System.Drawing.Color.Yellow);
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                    }

                    // format cells - add borders
                    using (ExcelRange r = ws.Cells[StartFromRow + 1, 1, StartFromRow + dt.Rows.Count, dt.Columns.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    }

                    // removed ignored columns
                    for (int i = dt.Columns.Count - 1; i >= 0; i--)
                    {
                        if (IgnoredColumns.Contains(dt.Columns[i].ColumnName))
                        {
                            ws.DeleteColumn(i + 1);
                        }
                    }

                    // add header and an additional column (left) and row (top)
                    if (!String.IsNullOrEmpty(Heading))
                    {
                        ws.Cells["A1"].Value = Heading;
                        ws.Cells["A1"].Style.Font.Size = 20;

                        ws.InsertColumn(1, 1);
                        ws.InsertRow(1, 1);
                        ws.Column(1).Width = 5;
                    }
                }

                pck.SaveAs(stream);
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="name"></param>
        /// <param name="exporttype"></param>
        /// <returns></returns>
        private static string GetDisplayNameCol(Type modelType, string name, string exporttype)
        {
            string displayName = AttributeHelper.GetDisplayName(modelType, name);
            switch (exporttype)
            {
                #region ExportExcelOrder
                case "ExportExcelOrder":
                    if (name == "Flight_No")
                    {
                        displayName = "航班号";
                    }
                    else if (name == "Flight_Date_Want")
                    {
                        displayName = "航班日期";
                    }
                    else if (name == "Pieces_Fact")
                    {
                        displayName = "实际件数";
                    }
                    else if (name == "Charge_Weight_Fact")
                    {
                        displayName = "实际重量";
                    }
                    else if (name == "Volume_Fact")
                    {
                        displayName = "实际体积";
                    }
                    else if (name == "Pieces_SK")
                    {
                        displayName = "分单件数";
                    }
                    else if (name == "Weight_SK")
                    {
                        displayName = "分单毛重";
                    }
                    else if (name == "Volume_SK")
                    {
                        displayName = "分单体积";
                    }
                    else if (name == "ADDWHO")
                    {
                        displayName = "接单人员";
                    }
                    break;
                #endregion
                #region ExportWarehouseReceipt
                case "ExportWarehouseReceipt":
                    if (name == "Pieces_H")
                    {
                        displayName = "No.OF PKG";
                    }
                    else if (name == "HBL")
                    {
                        displayName = "HAWB NO.";
                    }
                    else if (name == "Weight_H")
                    {
                        displayName = "Gross Weight";
                    }
                    else if (name == "EN_Name_H")
                    {
                        displayName = "NATURE OF GOOD";
                    }
                    else if (name == "End_Port")
                    {
                        displayName = "FINAL DEST";
                    }
                    else if (name == "Shipper_H")
                    {
                        displayName = @"NAME & ADDRESS OF | SHIPPER";
                    }
                    else if (name == "Consignee_H")
                    {
                        displayName = @"NAME & ADDRESS OF CONSIGNEE";
                    }
                    else if (name == "Pay_Mode_H")
                    {
                        displayName = "RE";
                    }
                    break;
                #endregion
                #region ExportOperationList
                case "ExportOperationList":
                    if (name == "Consign_Code")
                    {
                        displayName = "客户";
                    }
                    else if (name == "Flight_Date_Want")
                    {
                        displayName = "航班号/仓位";
                    }
                    else if (name == "Position")
                    {
                        displayName = "仓位";
                    }
                    else if (name == "Pieces_Fact")
                    {
                        displayName = "实际件数";
                    }
                    else if (name == "Weight_Fact")
                    {
                        displayName = "实际重量";
                    }
                    else if (name == "Volume_Fact")
                    {
                        displayName = "实际体积";
                    }
                    else if (name == "Pieces_SK")
                    {
                        displayName = "分单件数";
                    }
                    else if (name == "Weight_SK")
                    {
                        displayName = "分单毛重";
                    }
                    else if (name == "Volume_SK")
                    {
                        displayName = "分单体积";
                    }
                    break;
                #endregion
                #region ExportSpellingList
                case "ExportSpellingList":
                    if (name == "RK")
                    {
                        displayName = "拼号";
                    }
                    else
                        if (name == "Incidental_Expenses_M")
                    {
                        displayName = "P/C";
                    }
                    else if (name == "HPieces")
                    {
                        displayName = "件数";
                    }
                    else if (name == "HWeight")
                    {
                        displayName = "重量";
                    }
                    else if (name == "HVolume")
                    {
                        displayName = "体积";
                    }
                    else if (name == "Flight_Date_Want")
                    {
                        displayName = "航班号/日期";
                    }
                    else if (name == "Bragainon_Article_H")
                    {
                        displayName = "条款";
                    }
                    else if (name == "Weight_BG")
                    {
                        displayName = "报关重量";
                    }
                    else if (name == "Delivery_Point_Book_Flat_Code")
                    {
                        displayName = "卖货/交货点";
                    }
                    break;
                #endregion
                #region ExportDocumentManagementList
                case "ExportDocumentManagementList":
                    if (name == "Id")
                    {
                        displayName = "序号";
                    }
                    break;
                #endregion
                #region ExportBookFlatExcel
                case "ExportBookFlatExcel":
                    if (name == "FltNoDate")
                    {
                        displayName = "Flt No./Date";
                    }
                    if (name == "AWBNO")
                    {
                        displayName = "AWB NO.";
                    }
                    if (name == "BUremark")
                    {
                        displayName = "B/U remark";
                    }
                    if (name == "ConxFlts")
                    {
                        displayName = "Conx Flts";
                    }
                    if (name == "SPLcode")
                    {
                        displayName = "SPL code";
                    }
                    break;
                #endregion
                #region ExportBatchExcel
                case "ExportBatchExcel":
                    if (name == "Size")
                    {
                        displayName = "尺寸【长*宽*高*件数】";
                    }
                    else if (name == "Packing")
                    {
                        displayName = "包装情况";
                    }
                    else if (name == "Damaged_CK")
                    {
                        displayName = "破损情况";
                    }
                    else if (name == "Document")
                    {
                        displayName = "随机单证";
                    }
                    break;
                #endregion
            }
            return displayName;
        }

        public static Stream ExportExcel<T>(Type modelType, List<T> list, string exporttype = "", params string[] IgnoredColumns)
        {
            string Heading = "";
            var stream = new MemoryStream();
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(modelType.Name);
                int StartFromRow = String.IsNullOrEmpty(Heading) ? 1 : 3;

                // add the content into the Excel file
                ws.Cells["A" + StartFromRow].LoadFromCollection<T>(list, true, OfficeOpenXml.Table.TableStyles.Light1);
                if (exporttype == "ExportOperationList")
                {//设置导出excel的列宽度
                    ws.Column(1).Width = 11;//业务编号
                    ws.Column(2).Width = 15;//客户
                    ws.Column(3).Width = 13;//总单号
                    ws.Column(4).Width = 15;//航班号/仓位
                    ws.Column(5).Width = 6;//目的港
                    ws.Column(6).Width = 8;//实际件数
                    ws.Column(7).Width = 8;//实际重量
                    ws.Column(8).Width = 8;//实际体积
                    ws.Column(9).Width = 8;//分单件数
                    ws.Column(10).Width = 8;//分单毛重
                    ws.Column(11).Width = 8;//分单体积
                    ws.Column(12).Width = 27;//备注           
                    ws.PrinterSettings.LeftMargin = 0.1M;
                    ws.PrinterSettings.RightMargin = 0.1M;
                    ws.PrinterSettings.Orientation = eOrientation.Landscape;
                }
                else if (exporttype == "ExportBatchExcel")
                {
                    ws.Column(1).Width = 12;
                    ws.Column(2).Width = 9;
                    ws.Column(3).Width = 9;
                    ws.Column(4).Width = 10;
                    ws.Column(5).Width = 16;
                    ws.Column(6).Width = 8;
                    ws.Column(7).Width = 18;

                }

                Type _ty = typeof(T);
                //if (list.Count > 0)
                //{
                //    _ty = list[0].GetType();
                //}

                Type t = _ty;
                System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                for (int i = 1; i <= PropertyInfos.Count(); i++)
                {
                    //string fieldName = ws.Cells[1, i].Value.ToString();//PropertyInfos.Where(x => x.Name == ws.Cells[0, i].Value.ToString()).Select(x=>x.GetCustomAttributes(typeof(DisplayAttribute), true).);

                    string displayName = GetDisplayNameCol(modelType, PropertyInfos[i - 1].Name, exporttype);
                        //AttributeHelper.GetDisplayName(modelType, PropertyInfos[i - 1].Name);
                    ws.Cells[1, i].Value = displayName;
                    ws.Cells[StartFromRow, i].AutoFilter = false;//去掉筛选功能
                    ws.Cells[StartFromRow, i].Style.Font.Size = 8;
                }

                // autofit width of cells with small content
                int colindex = 1;
                //
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
                foreach (var col in props)
                {

                    ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, colindex, ws.Dimension.End.Row, colindex];
                    int maxLength = columnCells.Max(cell => (cell.Value == null ? "" : cell.Value.ToString()).Count());
                    if (maxLength < 150)
                    {
                        if (exporttype != "ExportOperationList" && exporttype != "ExportBatchExcel")//非操作清单的，自动列宽
                            ws.Column(colindex).AutoFit();
                    }

                        if (exporttype == "ExportOperationList")
                        {//操作清单的导出，设置自动换行
                            foreach (var cell in columnCells)
                            {
                                cell.Style.WrapText = true;     //文本自动换行 
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;////垂直居中  
                                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//设置边框  

                            }
                        }
                    colindex++;
                }

                // format header - bold, yellow on black
                using (ExcelRange r = ws.Cells[StartFromRow, 1, StartFromRow, props.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    if (exporttype == "ExportBatchExcel")
                    {
                        r.Style.Font.Bold = false; 
                        r.Style.Font.Size = 7;
                    }
                    else
                    {
                        r.Style.Font.Bold = true;
                    }
                    if (exporttype == "ExportOperationList")
                    {
                        r.Style.Font.Size = 10;
                    }
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }
                if (list.Count > 0)
                {
                    // format cells - add borders
                    using (ExcelRange r = ws.Cells[StartFromRow + 1, 1, StartFromRow + list.Count, props.Count])
                    {

                        r.Style.Font.Size = 10;
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    }
                }

                // removed ignored columns
                for (int i = props.Count - 1; i >= 0; i--)
                {
                    if (IgnoredColumns.Contains(props[i].Name))
                    {
                        ws.DeleteColumn(i + 1);
                    }
                }

                // add header and an additional column (left) and row (top)
                if (!String.IsNullOrEmpty(Heading))
                {
                    ws.Cells["A1"].Value = Heading;
                    ws.Cells["A1"].Style.Font.Size = 20;

                    ws.InsertColumn(1, 1);
                    ws.InsertRow(1, 1);
                    ws.Column(1).Width = 5;
                }

                pck.SaveAs(stream);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;

        }

        public static Stream ExportSpellingExcel<T>(Type modelType, List<T> list, string exporttype = "", params string[] IgnoredColumns)
        {
            string Heading = "";
            var stream = new MemoryStream();
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(modelType.Name);
                int StartFromRow = 1;
                if (exporttype == "ExportSpellingList")
                {
                    StartFromRow = String.IsNullOrEmpty(Heading) ? 5 : 1;
                }

                // add the content into the Excel file
                ws.Cells["A" + StartFromRow].LoadFromCollection<T>(list, true, OfficeOpenXml.Table.TableStyles.Light1);

                Type _ty = typeof(T);

                Type t = _ty;
                System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (exporttype == "ExportSpellingList")
                {
                    using (ExcelRange r = ws.Cells[1, 1, 1, PropertyInfos.Count()])
                    {//首行合并单元格，设置单元格格式
                        r.Merge = true;
                        r.Value = "上海飞力达国际物流有限公司";
                        r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Font.Size = 20;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        r.Style.Font.Bold = true;
                    }
                    using (ExcelRange r = ws.Cells[2, 1, 2, PropertyInfos.Count()])
                    {
                        r.Merge = true;
                        r.Value = "SHANGHAI FEILIKS INTERNATIONAL LOGISTICS CO.,LTD.";
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }


                    using (ExcelRange r = ws.Cells[3, 1, 3, PropertyInfos.Count()])
                    {
                        r.Merge = true;
                        r.Value = "拼  货  编  排  表";
                        r.Style.Font.Size = 20;
                        r.Style.Font.Bold = true;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    using (ExcelRange r = ws.Cells[4, 1, 4, PropertyInfos.Count()])
                    {
                        r.Merge = true;
                        r.Value = DateTime.Now.Hour+":"+DateTime.Now.Minute;
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = true;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }


                    //using (ExcelRange r = ws.Cells[5, 1, 5, PropertyInfos.Count()])
                    //{
                    //    r.AutoFilter = false;
                    //}
                }
                

                for (int i = 1; i <= PropertyInfos.Count(); i++)
                {
                    //string fieldName = ws.Cells[1, i].Value.ToString();//PropertyInfos.Where(x => x.Name == ws.Cells[0, i].Value.ToString()).Select(x=>x.GetCustomAttributes(typeof(DisplayAttribute), true).);

                    string displayName = GetDisplayNameCol(modelType, PropertyInfos[i - 1].Name, exporttype);
                    //AttributeHelper.GetDisplayName(modelType, PropertyInfos[i - 1].Name);
                    ws.Cells[StartFromRow, i].Value = displayName;
                    //ws.Cells[StartFromRow, i].AutoFilter = false;
                    //ws.Cells[20, 20].AutoFilter = false;
                }              
                // autofit width of cells with small content
                int colindex = 1;
                //
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
                //foreach (var col in props)
                //{
                //    ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, colindex, ws.Dimension.End.Row, colindex];
                //    int maxLength = columnCells.Max(cell => (cell.Value == null ? "" : cell.Value.ToString()).Count());
                //    if (maxLength < 150)
                //        ws.Column(colindex).AutoFit();
                //    if (exporttype == "ExportSpellingList" && colindex == 3)
                //    {
                //        var cellValue = 1;
                //        for (var i = 6; i <= ws.Dimension.End.Row; i++)
                //        {
                //            var cellnext = ws.Cells[i, colindex + 1];
                //            var cellnext2 = ws.Cells[i + 1, colindex + 1];
                //            if (cellnext.Value == null || cellnext.Value.ToString() == "")
                //            {
                //                cellValue = 1;
                //                continue;
                //            }
                //            if (cellnext.Value.ToString() == cellnext2.Value.ToString())
                //            {
                //                ws.Cells[i, colindex].Value = cellValue;
                //                cellValue++;
                //            }
                //            else
                //            {
                //                ws.Cells[i, colindex].Value = cellValue;
                //                cellValue = 1;
                //            }

                //        }
                //    }
                //    colindex++;
                //}

                // format header - bold, yellow on black
                using (ExcelRange r = ws.Cells[StartFromRow, 1, StartFromRow, props.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                    r.Style.Font.Size = 9;

                    //r.AutoFilter = false;
                    //r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                }
                //ws.Cells["O5"].AutoFilter = true;
                //ws.Cells["B5:05"].AutoFilter = true;
                ws.Column(1).Width = 10;//业务编号
                ws.Column(2).Width = 14;//发货方
                ws.Column(3).Width = 4;//拼号
                ws.Column(4).Width = 11;//总单号
                ws.Column(5).Width = 9;//分单号
                ws.Column(6).Width = 9;//目的港
                ws.Column(7).Width = 4;//P/C
                ws.Column(8).Width = 8;//件数
                ws.Column(9).Width = 8;//重量
                ws.Column(10).Width = 8;//体积
                ws.Column(11).Width = 10;//航班号/日期
                ws.Column(12).Width = 7;//条款
                ws.Column(13).Width = 8;//报关重量
                ws.Column(14).Width = 10;//备注
                ws.Column(15).Width = 15;//卖货/交货点

                ws.PrinterSettings.LeftMargin = 0.1M;
                ws.PrinterSettings.RightMargin = 0.1M;
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                //ws.Cells[ws.Dimension.Start.Row, colindex, ws.Dimension.End.Row, colindex].AutoFilter = false;
                
                

                //ws.Cells[StartFromRow, 1, StartFromRow, props.Count].AutoFilter;
                //ws.Cells.AutoFilter = false;
                if (list.Count > 0)
                {
                    // format cells - add borders
                    using (ExcelRange r = ws.Cells[5, 1, StartFromRow + list.Count, props.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        r.Style.Font.Size = 9; 
                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        //r.AutoFilter = false;
                    }
                }

                // removed ignored columns
                for (int i = props.Count - 1; i >= 0; i--)
                {
                    if (IgnoredColumns.Contains(props[i].Name))
                    {
                        ws.DeleteColumn(i + 1);
                    }
                }

                // add header and an additional column (left) and row (top)
                if (!String.IsNullOrEmpty(Heading))
                {
                    ws.Cells["A1"].Value = Heading;
                    ws.Cells["A1"].Style.Font.Size = 20;

                    ws.InsertColumn(1, 1);
                    ws.InsertRow(1, 1);
                    ws.Column(1).Width = 5;
                }

                pck.SaveAs(stream);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;

        }      

        //导出订舱单
        public static Stream ExportBookFlatExcel<T, P>(Type modelType, List<T> list, List<P> headlist, string exporttype = "", params string[] IgnoredColumns)
        {
            string Heading = "";
            var stream = new MemoryStream();
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(modelType.Name);
                int StartFromRow = 1;
                if (exporttype == "ExportBookFlatExcel")
                {
                    StartFromRow = String.IsNullOrEmpty(Heading) ? 2 : 1;
                }

                // add the content into the Excel file
                ws.Cells["A" + StartFromRow].LoadFromCollection<T>(list, true, OfficeOpenXml.Table.TableStyles.Light1);

                Type _ty = typeof(T);

                Type t = _ty;
                System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (exporttype == "ExportBookFlatExcel")
                {
                    using (ExcelRange r = ws.Cells[1, 1, 1, 3])
                    {//首行合并单元格，设置单元格格式
                        r.Merge = true;
                        r.Value = "AGT:__KSF__/Handling____";
                        r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Font.Size = 14;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        r.Style.Font.Bold = true;
                    }           
                    using (ExcelRange r = ws.Cells[1, 4, 1, PropertyInfos.Count()])
                    {//首行合并单元格，设置单元格格式
                        r.Merge = true;
                        r.Value = "CX/KA Booking List     ";                        
                        r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Font.Size = 22;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        r.Style.Font.Bold = true;
                    }
                }
              
                ws.PrinterSettings.LeftMargin = 0;
                ws.PrinterSettings.RightMargin = 0;
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.Column(1).Width = 13;
                ws.Column(2).Width = 6;
                ws.Column(3).Width = 13;
                ws.Column(4).Width = 8;
                ws.Column(5).Width = 8;
                ws.Column(6).Width = 6;
                ws.Column(7).Width = 6;
                ws.Column(8).Width = 6;
                ws.Column(9).Width = 6;
                ws.Column(10).Width = 6;
                ws.Column(11).Width = 6;
                ws.Column(12).Width = 6;
                ws.Column(13).Width = 6;
                ws.Column(14).Width = 6;
                ws.Column(15).Width = 8;
                ws.Column(16).Width = 15;
                ws.Column(17).Width = 10;


                for (int i = 1; i <= PropertyInfos.Count(); i++)
                {
                    //string fieldName = ws.Cells[1, i].Value.ToString();//PropertyInfos.Where(x => x.Name == ws.Cells[0, i].Value.ToString()).Select(x=>x.GetCustomAttributes(typeof(DisplayAttribute), true).);

                    string displayName = GetDisplayNameCol(modelType, PropertyInfos[i - 1].Name, exporttype);
                    //AttributeHelper.GetDisplayName(modelType, PropertyInfos[i - 1].Name);
                    ws.Cells[StartFromRow, i].Value = displayName;
                }

                // autofit width of cells with small content
                int colindex = 1;
                //
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

                // format header - bold, yellow on black
                using (ExcelRange r = ws.Cells[StartFromRow, 1, StartFromRow, props.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                    //r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                }
                if (list.Count > 0)
                {
                    // format cells - add borders
                    using (ExcelRange r = ws.Cells[1, 1, StartFromRow + list.Count, props.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        
                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                        ws.Cells["C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells["C1"].Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                        ws.Cells["D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells["D1"].Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                    }
                }
       
                // removed ignored columns
                for (int i = props.Count - 1; i >= 0; i--)
                {
                    if (IgnoredColumns.Contains(props[i].Name))
                    {
                        ws.DeleteColumn(i + 1);
                    }
                }

                // add header and an additional column (left) and row (top)
                if (!String.IsNullOrEmpty(Heading))
                {
                    ws.Cells["A1"].Value = Heading;
                    ws.Cells["A1"].Style.Font.Size = 20;

                    ws.InsertColumn(1, 1);
                    ws.InsertRow(1, 1);
                    ws.Column(1).Width = 5;
                }

                pck.SaveAs(stream);

            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;

        }
        /// <summary>
        /// 导出仓单PDF
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="modelType"></param>
        /// <param name="list"></param>
        /// <param name="headlist"></param>
        /// <param name="exporttype"></param>
        /// <param name="IgnoredColumns"></param>
        /// <returns></returns>
        public static Stream ExportWarehouseReceiptExcel<T, P>(Type modelType, List<T> list, List<P> headlist, string exporttype = "", params string[] IgnoredColumns)
        {
            string Heading = "";
            var stream = new MemoryStream();
            using (ExcelPackage pck = new ExcelPackage())
            {
                var head = headlist.FirstOrDefault();
                PropertyInfo[] pi = head.GetType().GetProperties();
                var sheetname = modelType.Name;
                if (exporttype == "ExportWarehouseReceipt")
                    sheetname = "仓单";

                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetname);
                
                int StartFromRow = String.IsNullOrEmpty(Heading) ? 7 : 3;

                // add the content into the Excel file
                ws.Cells["A" + StartFromRow].LoadFromCollection<T>(list, true, OfficeOpenXml.Table.TableStyles.Light1);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;//设置横向打印
                ws.PrinterSettings.LeftMargin = 0;//设置左边距
                ws.PrinterSettings.RightMargin = 0;//设置右边距

                Type _ty = typeof(T);

                Type t = _ty;
                System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                
                #region 报表顶部版面设置

                if (exporttype == "ExportWarehouseReceipt")
                {
                    using (ExcelRange r = ws.Cells[1, 1, 1, PropertyInfos.Count()])
                    {//首行合并单元格，设置单元格格式
                        r.Merge = true;
                        r.Value = "SHANGHAI FEILIKS INTERNATIONAL LOGISTICS CO.,LTD.";
                        r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Font.Size = 18;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        r.Style.Font.Bold = true;
                        
                    }
                    using (ExcelRange r = ws.Cells[2, 1, 2, PropertyInfos.Count()])
                    {
                        r.Merge = true;
                        r.Value = "CARGO MANIFEST";
                        r.Style.Font.Size = 18;
                        r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        r.Style.Font.Bold = true;
                    }


                    using (ExcelRange r = ws.Cells[3, 1, 3, 1])
                    {
                        r.Merge = true;
                        r.Value = " Master Bill No.:";
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    using (ExcelRange r = ws.Cells[3, 4, 3, 4])
                    {
                        r.Merge = true;
                        r.Value = " Port of Loading:";
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    using (ExcelRange r = ws.Cells[3, 6, 3, 6])
                    {
                        r.Merge = true;
                        r.Value = " Destination:";
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    using (ExcelRange r = ws.Cells[5, 1, 5, 1])
                    {
                        r.Merge = true;
                        r.Value = " Flight /Date :";
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    using (ExcelRange r = ws.Cells[5, 4, 5, 4])
                    {
                        r.Merge = true;
                        r.Value = " Agent to:";
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    using (ExcelRange r = ws.Cells[8 + list.Count, 1, 8 + list.Count, 2])
                    {
                        r.Merge = true;
                        r.Value = " TOTAL NO. OF HAWB:";
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    using (ExcelRange r = ws.Cells[8 + list.Count, 3, 8 + list.Count, 3])
                    {
                        r.Merge = true;
                        r.Value = list.Count;
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    }

                    using (ExcelRange r = ws.Cells[8 + list.Count, 4, 8 + list.Count, 4])
                    {
                        r.Merge = true;
                        r.Value = " TOTAL PACKAGS:";
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    using (ExcelRange r = ws.Cells[8 + list.Count, 6, 8 + list.Count, 6])
                    {
                        r.Merge = true;
                        r.Value = " TOTAL GrossWeight：";
                        r.Style.Font.Size = 10;
                        r.Style.Font.Bold = false;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    foreach (var item in pi)
                    {
                        if (item.Name == "MBL")
                        {
                            using (ExcelRange r = ws.Cells[3, 2, 3, 3])
                            {
                                r.Style.Font.Size = 10;
                                r.Merge = true;
                                r.Value = item.GetValue(head, null);
                                r.Style.Font.Bold = false;
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            }
                        }
                        else if (item.Name == "Depart_Port")
                        {
                            using (ExcelRange r = ws.Cells[3, 5, 3, 5])
                            {
                                r.Style.Font.Size = 10;
                                r.Merge = true;
                                r.Value = item.GetValue(head, null);
                                r.Style.Font.Bold = false;
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            }
                        }
                        else if (item.Name == "End_Port")
                        {
                            using (ExcelRange r = ws.Cells[3, 7, 3, 8])
                            {
                                r.Style.Font.Size = 10;
                                r.Merge = true;
                                r.Value = item.GetValue(head, null);
                                r.Style.Font.Bold = false;
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            }
                        }
                        else if (item.Name == "Flight_No")
                        {
                            using (ExcelRange r = ws.Cells[5, 2, 5, 3])
                            {
                                r.Style.Font.Size = 10;
                                r.Merge = true;
                                r.Value = item.GetValue(head, null);
                                r.Style.Font.Bold = false;
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            }
                        }
                        else if (item.Name == "FWD_Code")
                        {
                            using (ExcelRange r = ws.Cells[5, 5, 5, 8])
                            {
                                r.Style.Font.Size = 10;
                                r.Merge = true;
                                r.Value = item.GetValue(head, null);
                                r.Style.Font.Bold = false;
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            }
                        }
                    }
                }
                #endregion 
                
                var row = ws.Row(1);
                row.Height = 30;
                var row2 = ws.Row(2);
                row2.Height = 25;
                for(int i = 1; i <= PropertyInfos.Count(); i++)
                {
                    //string fieldName = ws.Cells[1, i].Value.ToString();//PropertyInfos.Where(x => x.Name == ws.Cells[0, i].Value.ToString()).Select(x=>x.GetCustomAttributes(typeof(DisplayAttribute), true).);
                    if (i < PropertyInfos.Count() + 1)
                    {
                        string displayName = GetDisplayNameCol(modelType, PropertyInfos[i - 1].Name, exporttype);
                        ws.Cells[StartFromRow, i].Value = displayName;
                        switch (i)
                        {
                            case 1:
                                ws.Column(i).Width = 14;
                                break;
                            case 2:
                                ws.Column(i).Width = 9;
                                break;
                            case 3:
                                ws.Column(i).Width = 10;
                                break;
                            case 4:
                                ws.Column(i).Width = 21;
                                break;
                            case 5:
                                ws.Column(i).Width = 15;
                                break;
                            case 6:
                                ws.Column(i).Width = 26;
                                break;
                            case 7:
                                ws.Column(i).Width = 24;
                                break;
                            case 8:
                                ws.Column(i).Width = 11;
                                break;

                        }
                    }
                
                    ws.Cells[StartFromRow, i].AutoFilter = false;
                    //ws.Column(i).Style.WrapText = true;
                }

                // autofit width of cells with small content
                int colindex = 1;
                var numtotal = 0;//统计件数
                var weighttotal = 0;//统计毛重
                //
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
                foreach (var col in props)
                {
                    ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, colindex, ws.Dimension.End.Row, colindex];
                    int maxLength = columnCells.Max(cell => (cell.Value == null ? "" : cell.Value.ToString()).Count());
                    if (maxLength < 150)
                        //ws.Column(colindex).AutoFit();
                    foreach (var cell in columnCells)
                    {
                        cell.Style.WrapText = true;     //文本自动换行 
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;////垂直居中  
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;//设置边框  
                    }
                    if (exporttype == "ExportWarehouseReceipt" && colindex == 2)
                    {
                        for (var i = 8; i < ws.Dimension.End.Row; i++)
                        {
                            var cellnumnext = ws.Cells[i, colindex];
                            var cellweightnext = ws.Cells[i, colindex+1];
                            if (cellnumnext.Value != null)
                            {
                                var colnumvalue = cellnumnext.Value.ToString();
                                numtotal += Int32.Parse(colnumvalue);
                            }
                            if (cellweightnext.Value != null)
                            {
                                var weightvalue = cellweightnext.Value.ToString();
                                weighttotal += Int32.Parse(weightvalue);
                            }
                        }
                    }
                    colindex++;
                }
                
                using (ExcelRange r = ws.Cells[8 + list.Count, 5, 8 + list.Count, 5])
                {
                    r.Merge = true;
                    r.Value = numtotal;
                    r.Style.Font.Size = 10;
                    r.Style.Font.Bold = false;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                }

                using (ExcelRange r = ws.Cells[8 + list.Count, 7, 8 + list.Count, 7])
                {
                    r.Merge = true;
                    r.Value = weighttotal;
                    r.Style.Font.Size = 10;
                    r.Style.Font.Bold = false;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                }
                using (ExcelRange r = ws.Cells[8 + list.Count, 8, 8 + list.Count, 8])
                {
                    r.Merge = true;
                    r.Value = "KGS";
                    r.Style.Font.Size = 10;
                    r.Style.Font.Bold = false;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }


                // format header - bold, yellow on black
                using (ExcelRange r = ws.Cells[StartFromRow, 1, StartFromRow, props.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }
                if (list.Count > 0)
                {
                    // format cells - add borders
                    using (ExcelRange r = ws.Cells[StartFromRow + 1, 1, StartFromRow + list.Count, props.Count])
                    {
                        r.Style.Font.Size = 8;
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        r.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                        for (var i = StartFromRow + 1; i < StartFromRow + list.Count + 1; i++) 
                        {
                            ws.Row(i).Height = 77;
                        }
                    }
                }

                // removed ignored columns
                for (int i = props.Count - 1; i >= 0; i--)
                {
                    if (IgnoredColumns.Contains(props[i].Name))
                    {
                        ws.DeleteColumn(i + 1);
                    }
                }

                // add header and an additional column (left) and row (top)
                if (!String.IsNullOrEmpty(Heading))
                {
                    ws.Cells["A1"].Value = Heading;
                    ws.Cells["A1"].Style.Font.Size = 20;

                    ws.InsertColumn(1, 1);
                    ws.InsertRow(1, 1);
                    //ws.Column(1).Width = 5;
                }

                pck.SaveAs(stream);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;

        }

        public static DataSet GetDataTableFromExcelByAspose(string ExcelPath)
        {
            DataSet ds = new DataSet();
            try
            {
                Aspose.Cells.License li = new Aspose.Cells.License();
                string lipath = HttpContext.Current.Server.MapPath("/Aspose/License.lic");
                li.SetLicense(lipath);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(HttpContext.Current.Server.MapPath(ExcelPath));
                if (workbook.Worksheets.Count > 0)
                {
                    foreach (Worksheet ws in workbook.Worksheets)
                    {
                        DataTable dt = ws.Cells.ExportDataTable(0, 0, ws.Cells.MaxDataRow + 1, ws.Cells.MaxDataColumn + 1, true);
                        ds.Tables.Add(dt);
                    }
                }
            }
            catch
            {
                ds = new DataSet();
            }
            return ds;
        }

        public static DataSet GetDataTableFromExcelByAspose(Stream ExcelStream)
        {
            DataSet ds = new DataSet();
            try
            {
                Aspose.Cells.License li = new Aspose.Cells.License();
                string lipath = HttpContext.Current.Server.MapPath("/Aspose/License.lic");
                li.SetLicense(lipath);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ExcelStream);
                if (workbook.Worksheets.Count > 0)
                {
                    foreach (Worksheet ws in workbook.Worksheets)
                    {
                        if (ws.Cells.MaxDataRow > 0)
                        {
                            DataTable dt = ws.Cells.ExportDataTable(0, 0, ws.Cells.MaxDataRow + 1, ws.Cells.MaxDataColumn + 1, true);
                            ds.Tables.Add(dt);
                        }
                    }
                }
            }
            catch
            {
                ds = new DataSet();
            }
            return ds;
        }

        public static DataSet GetDataTableFromExcelByAspose(byte[] Excelbytes)
        {
            DataSet ds = new DataSet();
            try
            {
                MemoryStream ExcelStream = new MemoryStream(Excelbytes);
                Aspose.Cells.License li = new Aspose.Cells.License();
                string lipath = HttpContext.Current.Server.MapPath("/Aspose/License.lic");
                li.SetLicense(lipath);
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ExcelStream);
                if (workbook.Worksheets.Count > 0)
                {
                    foreach (Worksheet ws in workbook.Worksheets)
                    {
                        if (ws.Cells.MaxDataRow > 0)
                        {
                            DataTable dt = ws.Cells.ExportDataTable(0, 0, ws.Cells.MaxDataRow + 1, ws.Cells.MaxDataColumn + 1, true);
                            ds.Tables.Add(dt);
                        }
                    }
                }
            }
            catch
            {
                ds = new DataSet();
            }
            return ds;
        }

        /// <summary>
        /// 返回Model 数据集,Excel形式的二进制数据
        /// </summary>
        /// <param name="ArrObjT">IEnumerable 数据</param>
        /// <returns></returns>
        public static byte[] OutPutExcelByArrModel(Object ArrObjT)
        {
            //项目顶层命名空间
            string Top_NameSpace = System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"] ?? "AirOut";
            //网站顶层命名空间
            string WebTop_NameSpace = System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"] ?? "AirOut.Web";
            //项目类命名空间
            string _ModelsNameSpace = System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"] ?? "Models";
            //服务命名空间
            string ServiceNameSpace = System.Configuration.ConfigurationManager.AppSettings["ServiceNameSpace"] ?? "Services";
            //仓库命名空间
            string RepositoriesNameSpace = System.Configuration.ConfigurationManager.AppSettings["RepositoriesNameSpace"] ?? "Repositories";

            string ModelsNameSpace = (string.IsNullOrEmpty(WebTop_NameSpace) || string.IsNullOrEmpty(_ModelsNameSpace)) ? "AirOut.Web.Models" : WebTop_NameSpace + "." + _ModelsNameSpace;
            string ModelsTopNameSpace = Top_NameSpace + ".";

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            Aspose.Cells.License li = new Aspose.Cells.License();
            string path = HttpContext.Current.Server.MapPath("/Aspose/License.lic");
            li.SetLicense(path);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            int RowIndex = 0;
            int CellsIndex = 0;
            var _obj = ArrObjT as System.Collections.IList;
            Type _ty = null;
            if (_obj.Count > 0)
            {
                _ty = _obj[0].GetType();
            }

            Type t = _ty;
            System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            //string ss = Common.GetDisplayName(_Employee.GetType(), "Id");
            foreach (var item in _obj)
            {
                CellsIndex = 0;
                if (RowIndex == 0)
                {
                    #region 首行赋值 并 设置 列宽

                    foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                    {
                        DisplayAttribute disAttr = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                        string StrTxt = fi.Name;// +"(" + fi.PropertyType.Name + ")";
                        if (disAttr != null)
                            StrTxt = disAttr.Name;// +"(" + fi.PropertyType.Name + ")";
                        sheet.Cells[RowIndex, CellsIndex].Value = StrTxt;
                        Aspose.Cells.Style style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                        System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                        //12cm为12*96/2.54=454像素
                        sheet.Cells.Columns[CellsIndex].Width = g.MeasureString(StrTxt, font).Width / 7.384;
                        CellsIndex++;
                    }

                    #endregion

                    CellsIndex = 0;
                    RowIndex++;
                }
                foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                {
                    DisplayAttribute disAttr = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

                    object obj = fi.GetValue((object)item, null);
                    string str = "";
                    Type _type = null;
                    if (obj != null)
                    {
                        str = obj.ToString();
                        _type = obj.GetType();
                    }
                    if (str.IndexOf(ModelsTopNameSpace) >= 0 || str.IndexOf("System.") >= 0)
                    {
                        try
                        {
                            if (_type != null)
                            {
                                if (str.IndexOf(ModelsTopNameSpace) >= 0 && _type.Name.IndexOf("HashSet") < 0)
                                {
                                    string FieldName = "Name";
                                    System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                    var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                    if (_PropertyInfos.Any())
                                    {
                                        obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null);
                                    }
                                }
                                if (str.IndexOf("System.Collections.") >= 0)
                                {
                                    string FieldName = "Count";
                                    System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                    var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                    if (_PropertyInfos.Any())
                                    {
                                        string ModelName = obj.ToString();
                                        try
                                        {
                                            if (disAttr != null)
                                                ModelName = disAttr.Name;
                                            else
                                            {
                                                string Split = ModelsNameSpace;
                                                int Sindex = ModelName.IndexOf(Split);
                                                if (Sindex > 0)
                                                {
                                                    ModelName = ModelName.Substring(Sindex + Split.Length);
                                                    if (ModelName.IndexOf(']') >= 0)
                                                    {
                                                        ModelName = ModelName.Substring(0, ModelName.IndexOf(']'));
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                        obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null).ToString() + "-" + ModelName;
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    var style = sheet.Cells[RowIndex, CellsIndex].GetStyle();

                    var retVal = obj;
                    string DataType = fi.PropertyType.Name;
                    if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var Arguments = fi.PropertyType.GetGenericArguments();
                        DataType = Arguments[0].Name;
                    }
                    if (retVal != null)
                    {
                        switch (DataType.ToLower())
                        {
                            case "int":
                                int Dftint = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            case "int32":
                                int Dftint32 = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint32))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            case "int64":
                                int Dftint64 = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint64))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;

                            case "string":
                                style.Number = 9;
                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                break;
                            case "datetime":
                                int TDatetime = 0;
                                if (int.TryParse(retVal.ToString(), out TDatetime))
                                {
                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                else
                                {
                                    DateTime DftDateTime = new DateTime();
                                    if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                    {
                                        style.Custom = "yyyy-MM-dd HH:mm:ss";
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                }
                                break;
                            case "bool":
                                bool Dftbool = false;
                                if (bool.TryParse(retVal.ToString(), out Dftbool))
                                {
                                    style.Number = 9;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    if (Dftbool)
                                    {
                                        style.Font.Color = System.Drawing.Color.Green;
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        obj = "是";
                                    }
                                    else
                                    {
                                        style.Font.Color = System.Drawing.Color.Red;
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        obj = "否";
                                    }
                                }
                                break;
                            case "boolean":
                                bool Dftboolean = false;
                                if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                {
                                    style.Number = 9;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    if (Dftboolean)
                                    {
                                        style.Font.Color = System.Drawing.Color.Green;
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        obj = "是";
                                    }
                                    else
                                    {
                                        style.Font.Color = System.Drawing.Color.Red;
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        obj = "否";
                                    }
                                }
                                break;
                            case "decimal":
                                decimal Dftdecimal = 0;
                                if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            case "double":
                                double Dftdouble = 0;
                                if (double.TryParse(retVal.ToString(), out Dftdouble))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            case "float":
                                float Dftfloat = 0;
                                if (float.TryParse(retVal.ToString(), out Dftfloat))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            default:
                                style.Number = 9;
                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                break;
                        }
                    }
                    if (obj != null)
                    {
                        Aspose.Cells.Style _style = sheet.Cells.Columns[CellsIndex].Style;
                        System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                        double StrWidth = 0;
                        StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                        if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                            sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                    }
                    #region 设置背景色
                    //else
                    //{
                    //    var _style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                    //    //_style.BackgroundColor = System.Drawing.Color.LightSkyBlue;
                    //    _style.ForegroundColor = System.Drawing.Color.FromArgb(230, 211, 211, 211);
                    //    _style.Pattern = Aspose.Cells.BackgroundType.Solid;
                    //    sheet.Cells[RowIndex, CellsIndex].SetStyle(_style);
                    //}
                    #endregion

                    sheet.Cells[RowIndex, CellsIndex].Value = obj;
                    CellsIndex++;
                }
                RowIndex++;
            }

            string FileDownLoadPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "\\FileDownLoad\\";
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath(FileDownLoadPath));
            if (!dir.Exists)
                dir.Create();

            workbook.Save((dir.FullName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls"), Aspose.Cells.SaveFormat.Excel97To2003);
            return workbook.SaveToStream().ToArray();

        }

        /// <summary>
        /// 返回Model 数据集,Excel形式的二进制数据
        /// </summary>
        /// <param name="ListEmployee"></param>
        /// <returns></returns>
        public static byte[] OutPutExcelByListModel<T>(T ObjT)//List<obj>
        {
            //项目顶层命名空间
            string Top_NameSpace = System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"] ?? "AirOut";
            //网站顶层命名空间
            string WebTop_NameSpace = System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"] ?? "AirOut.Web";
            //项目类命名空间
            string _ModelsNameSpace = System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"] ?? "Models";
            //服务命名空间
            string ServiceNameSpace = System.Configuration.ConfigurationManager.AppSettings["ServiceNameSpace"] ?? "Services";
            //仓库命名空间
            string RepositoriesNameSpace = System.Configuration.ConfigurationManager.AppSettings["RepositoriesNameSpace"] ?? "Repositories";

            string ModelsNameSpace = (string.IsNullOrEmpty(WebTop_NameSpace) || string.IsNullOrEmpty(_ModelsNameSpace)) ? "AirOut.Web.Models" : WebTop_NameSpace + "." + _ModelsNameSpace;
            string ModelsTopNameSpace = Top_NameSpace + ".";

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            Aspose.Cells.License li = new Aspose.Cells.License();
            string path = HttpContext.Current.Server.MapPath("/Aspose/License.lic");
            li.SetLicense(path);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            int RowIndex = 0;
            int CellsIndex = 0;
            var _obj = ObjT as System.Collections.IList;
            Type _ty = null;
            if (_obj.Count > 0)
            {
                _ty = _obj[0].GetType();
            }

            Type t = _ty;
            System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            //string ss = Common.GetDisplayName(_Employee.GetType(), "Id");
            foreach (var item in _obj)
            {
                CellsIndex = 0;
                if (RowIndex == 0)
                {
                    #region 首行赋值 并 设置 列宽
                    foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                    {
                        DisplayAttribute disAttr = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                        string StrTxt = fi.Name;// +"(" + fi.PropertyType.Name + ")";
                        if (disAttr != null)
                            StrTxt = disAttr.Name;// +"(" + fi.PropertyType.Name + ")";
                        sheet.Cells[RowIndex, CellsIndex].Value = StrTxt;
                        Aspose.Cells.Style style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                        System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                        //12cm为12*96/2.54=454像素
                        sheet.Cells.Columns[CellsIndex].Width = g.MeasureString(StrTxt, font).Width / 7.384;
                        CellsIndex++;
                    }
                    #endregion
                    CellsIndex = 0;
                    RowIndex++;
                }
                foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                {
                    DisplayAttribute disAttr = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

                    object obj = fi.GetValue((object)item, null);
                    string str = "";
                    Type _type = null;
                    if (obj != null)
                    {
                        str = obj.ToString();
                        _type = obj.GetType();
                    }
                    if (str.IndexOf(ModelsTopNameSpace) >= 0 || str.IndexOf("System.") >= 0)
                    {
                        try
                        {
                            if (_type != null)
                            {
                                if (str.IndexOf(ModelsTopNameSpace) >= 0 && _type.Name.IndexOf("HashSet") < 0)
                                {
                                    string FieldName = "Name";
                                    System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                    var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                    if (_PropertyInfos.Any())
                                    {
                                        obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null);
                                    }
                                }
                                if (str.IndexOf("System.Collections.") >= 0)
                                {
                                    string FieldName = "Count";
                                    System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                    var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                    if (_PropertyInfos.Any())
                                    {
                                        string ModelName = obj.ToString();
                                        try
                                        {
                                            if (disAttr != null)
                                                ModelName = disAttr.Name;
                                            else
                                            {
                                                string Split = ModelsNameSpace;
                                                int Sindex = ModelName.IndexOf(Split);
                                                if (Sindex > 0)
                                                {
                                                    ModelName = ModelName.Substring(Sindex + Split.Length);
                                                    if (ModelName.IndexOf(']') >= 0)
                                                    {
                                                        ModelName = ModelName.Substring(0, ModelName.IndexOf(']'));
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                        obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null).ToString() + "-" + ModelName;
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    var style = sheet.Cells[RowIndex, CellsIndex].GetStyle();

                    var retVal = obj;
                    string DataType = fi.PropertyType.Name;
                    if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var Arguments = fi.PropertyType.GetGenericArguments();
                        DataType = Arguments[0].Name;
                    }
                    if (retVal != null)
                    {
                        switch (DataType.ToLower())
                        {
                            case "int":
                                int Dftint = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            case "int32":
                                int Dftint32 = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint32))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            case "int64":
                                int Dftint64 = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint64))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;

                            case "string":
                                style.Number = 9;
                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                break;
                            case "datetime":
                                int TDatetime = 0;
                                if (int.TryParse(retVal.ToString(), out TDatetime))
                                {
                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                else
                                {
                                    DateTime DftDateTime = new DateTime();
                                    if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                    {
                                        style.Custom = "yyyy-MM-dd HH:mm:ss";
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                }
                                break;
                            case "bool":
                                bool Dftbool = false;
                                if (bool.TryParse(retVal.ToString(), out Dftbool))
                                {
                                    style.Number = 9;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    if (Dftbool)
                                    {
                                        style.Font.Color = System.Drawing.Color.Green;
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        obj = "是";
                                    }
                                    else
                                    {
                                        style.Font.Color = System.Drawing.Color.Red;
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        obj = "否";
                                    }
                                }
                                break;
                            case "boolean":
                                bool Dftboolean = false;
                                if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                {
                                    style.Number = 9;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    if (Dftboolean)
                                    {
                                        style.Font.Color = System.Drawing.Color.Green;
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        obj = "是";
                                    }
                                    else
                                    {
                                        style.Font.Color = System.Drawing.Color.Red;
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        obj = "否";
                                    }
                                }
                                break;
                            case "decimal":
                                decimal Dftdecimal = 0;
                                if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            case "double":
                                double Dftdouble = 0;
                                if (double.TryParse(retVal.ToString(), out Dftdouble))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            case "float":
                                float Dftfloat = 0;
                                if (float.TryParse(retVal.ToString(), out Dftfloat))
                                {
                                    style.Number = 1;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                }
                                break;
                            default:
                                style.Number = 9;
                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                break;
                        }
                    }
                    if (obj != null)
                    {
                        Aspose.Cells.Style _style = sheet.Cells.Columns[CellsIndex].Style;
                        System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                        double StrWidth = 0;
                        StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                        if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                            sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                    }
                    #region 设置背景色
                    //else
                    //{
                    //    var _style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                    //    //_style.BackgroundColor = System.Drawing.Color.LightSkyBlue;
                    //    _style.ForegroundColor = System.Drawing.Color.FromArgb(230, 211, 211, 211);
                    //    _style.Pattern = Aspose.Cells.BackgroundType.Solid;
                    //    sheet.Cells[RowIndex, CellsIndex].SetStyle(_style);
                    //}
                    #endregion

                    sheet.Cells[RowIndex, CellsIndex].Value = obj;
                    CellsIndex++;
                }

                RowIndex++;
            }
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/DownLoad/"));
            if (!dir.Exists)
                dir.Create();

            workbook.Save((dir.FullName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls"), Aspose.Cells.SaveFormat.Excel97To2003);
            return workbook.SaveToStream().ToArray();

        }

        /// <summary>
        /// 返回Model 数据集,Excel形式的二进制数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="ObjT">List<obj></param>
        /// <param name="ColumnNames">要输出的列名</param>
        /// <returns></returns>
        public static byte[] OutPutExcelByListModel<T>(T ObjT, List<string> ColumnNames, string FileStartName)//List<obj>
        {
            //项目顶层命名空间
            string Top_NameSpace = System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"] ?? "AirOut";
            //网站顶层命名空间
            string WebTop_NameSpace = System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"] ?? "AirOut.Web";
            //项目类命名空间
            string _ModelsNameSpace = System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"] ?? "Models";
            //服务命名空间
            string ServiceNameSpace = System.Configuration.ConfigurationManager.AppSettings["ServiceNameSpace"] ?? "Services";
            //仓库命名空间
            string RepositoriesNameSpace = System.Configuration.ConfigurationManager.AppSettings["RepositoriesNameSpace"] ?? "Repositories";

            string ModelsNameSpace = (string.IsNullOrEmpty(WebTop_NameSpace) || string.IsNullOrEmpty(_ModelsNameSpace)) ? "AirOut.Web.Models" : WebTop_NameSpace + "." + _ModelsNameSpace;
            string ModelsTopNameSpace = Top_NameSpace + ".";

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            ////单元格 最大宽度
            //double MaxCellWidth = 80;

            Aspose.Cells.License li = new Aspose.Cells.License();
            string path = HttpContext.Current.Server.MapPath("/Aspose/License.lic");
            li.SetLicense(path);
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            int RowIndex = 0;
            int CellsIndex = 0;
            var _obj = ObjT as System.Collections.IList;
            Type _ty = null;
            if (_obj.Count > 0)
            {
                _ty = _obj[0].GetType();
            }

            Type t = _ty;
            System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            //string ss = Common.GetDisplayName(_Employee.GetType(), "Id");

            Dictionary<string, Tuple<bool, System.Reflection.PropertyInfo, DisplayAttribute>> DictArrHasColumn = new Dictionary<string, Tuple<bool, System.Reflection.PropertyInfo, DisplayAttribute>>();
            foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
            {
                DisplayAttribute disAttritem = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                string fiName = fiitem == null ? "" : fiitem.Name;
                string disfiName = disAttritem == null ? "" : disAttritem.Name;
                var WhereColumnNames = ColumnNames.Where(x => x == fiName || x == disfiName);
                if (WhereColumnNames.Any())
                {
                    var OColumnName = WhereColumnNames.FirstOrDefault();
                    if (!DictArrHasColumn.Any(x => x.Key == OColumnName))
                    {
                        Tuple<bool, System.Reflection.PropertyInfo, DisplayAttribute> OTuple = new Tuple<bool, PropertyInfo, DisplayAttribute>(true, fiitem, disAttritem);
                        DictArrHasColumn.Add(OColumnName, OTuple);
                    }
                }
            }

            foreach (var item in _obj)
            {
                CellsIndex = 0;
                if (RowIndex == 0)
                {
                    #region 首行赋值 并 设置 列宽

                    foreach (var itemColumn in ColumnNames)
                    {
                        sheet.Cells[RowIndex, CellsIndex].Value = itemColumn;
                        Aspose.Cells.Style style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                        System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                        //12cm为12*96/2.54=454像素
                        sheet.Cells.Columns[CellsIndex].Width = g.MeasureString(itemColumn, font).Width / 7.384;
                        CellsIndex++;
                    }

                    #endregion
                    CellsIndex = 0;
                    RowIndex++;
                }

                foreach (var itemColumn in ColumnNames)
                {
                    DisplayAttribute disAttr = null;
                    System.Reflection.PropertyInfo fi = null;

                    #region 是否 包含列

                    bool HasColumn = true;
                    var WhereDictArrHasColumn = DictArrHasColumn.Where(x => x.Key == itemColumn);
                    if (WhereDictArrHasColumn.Any())
                    {
                        var ODictArrHasColumn = WhereDictArrHasColumn.FirstOrDefault();
                        HasColumn = ODictArrHasColumn.Value.Item1;
                        fi = ODictArrHasColumn.Value.Item2;
                        disAttr = ODictArrHasColumn.Value.Item3;
                    }
                    //foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                    //{
                    //    HasColumn = false;
                    //    DisplayAttribute disAttritem = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                    //    if (itemColumn == fiitem.Name)
                    //        HasColumn = true;
                    //    if (disAttritem != null)
                    //    {
                    //        if (itemColumn == disAttritem.Name)
                    //            HasColumn = true;
                    //    }
                    //    if (HasColumn)
                    //    {
                    //        fi = fiitem;
                    //        disAttr = disAttritem;
                    //        break;
                    //    }
                    //}

                    #endregion

                    #region 添加列

                    if (fi != null)
                    {
                        object obj = fi.GetValue((object)item, null);
                        string str = "";
                        Type _type = null;
                        if (obj != null)
                        {
                            str = obj.ToString();
                            _type = obj.GetType();
                        }
                        if (str.IndexOf(ModelsTopNameSpace) >= 0 || str.IndexOf("System.") >= 0)
                        {
                            try
                            {
                                if (_type != null)
                                {
                                    if (str.IndexOf(ModelsTopNameSpace) >= 0 && _type.Name.IndexOf("HashSet") < 0)
                                    {
                                        string FieldName = "Name";
                                        System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                        var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                        if (_PropertyInfos.Any())
                                        {
                                            obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null);
                                        }
                                    }
                                    if (str.IndexOf("System.Collections.") >= 0)
                                    {
                                        string FieldName = "Count";
                                        System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                        var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                        if (_PropertyInfos.Any())
                                        {
                                            string ModelName = obj.ToString();
                                            try
                                            {
                                                if (disAttr != null)
                                                    ModelName = disAttr.Name;
                                                else
                                                {
                                                    string Split = ModelsNameSpace;
                                                    int Sindex = ModelName.IndexOf(Split);
                                                    if (Sindex > 0)
                                                    {
                                                        ModelName = ModelName.Substring(Sindex + Split.Length);
                                                        if (ModelName.IndexOf(']') >= 0)
                                                        {
                                                            ModelName = ModelName.Substring(0, ModelName.IndexOf(']'));
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                            obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null).ToString() + "-" + ModelName;
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }

                        var style = sheet.Cells[RowIndex, CellsIndex].GetStyle();

                        var retVal = obj;
                        string DataType = fi.PropertyType.Name;
                        if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var Arguments = fi.PropertyType.GetGenericArguments();
                            DataType = Arguments[0].Name;
                        }
                        if (retVal != null)
                        {
                            switch (DataType.ToLower())
                            {
                                case "int":
                                    int Dftint = 0;
                                    if (int.TryParse(retVal.ToString(), out Dftint))
                                    {
                                        style.Number = 1;
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                    break;
                                case "int32":
                                    int Dftint32 = 0;
                                    if (int.TryParse(retVal.ToString(), out Dftint32))
                                    {
                                        style.Number = 1;
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                    break;
                                case "int64":
                                    int Dftint64 = 0;
                                    if (int.TryParse(retVal.ToString(), out Dftint64))
                                    {
                                        style.Number = 1;
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                    break;

                                case "string":
                                    style.Number = 9;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    break;
                                case "datetime":
                                    int TDatetime = 0;
                                    if (int.TryParse(retVal.ToString(), out TDatetime))
                                    {
                                        style.Custom = "yyyy-MM-dd HH:mm:ss";
                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                    else
                                    {
                                        DateTime DftDateTime = new DateTime();
                                        if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                        {
                                            style.Custom = "yyyy-MM-dd HH:mm:ss";
                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        }
                                    }
                                    break;
                                case "bool":
                                    bool Dftbool = false;
                                    if (bool.TryParse(retVal.ToString(), out Dftbool))
                                    {
                                        style.Number = 9;
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        if (Dftbool)
                                        {
                                            style.Font.Color = System.Drawing.Color.Green;
                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            obj = "是";
                                        }
                                        else
                                        {
                                            style.Font.Color = System.Drawing.Color.Red;
                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            obj = "否";
                                        }
                                    }
                                    break;
                                case "boolean":
                                    bool Dftboolean = false;
                                    if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                    {
                                        style.Number = 9;
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                        if (Dftboolean)
                                        {
                                            style.Font.Color = System.Drawing.Color.Green;
                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            obj = "是";
                                        }
                                        else
                                        {
                                            style.Font.Color = System.Drawing.Color.Red;
                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            obj = "否";
                                        }
                                    }
                                    break;
                                case "decimal":
                                    decimal Dftdecimal = 0;
                                    if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                    {
                                        style.Number = 1;
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                    break;
                                case "double":
                                    double Dftdouble = 0;
                                    if (double.TryParse(retVal.ToString(), out Dftdouble))
                                    {
                                        style.Number = 1;
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                    break;
                                case "float":
                                    float Dftfloat = 0;
                                    if (float.TryParse(retVal.ToString(), out Dftfloat))
                                    {
                                        style.Number = 1;
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    }
                                    break;
                                default:
                                    style.Number = 9;
                                    //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                    break;
                            }
                        }
                        if (obj != null)
                        {
                            Aspose.Cells.Style _style = sheet.Cells.Columns[CellsIndex].Style;
                            System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                            double StrWidth = 0;
                            StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                            if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                        }
                        #region 设置背景色
                        //else
                        //{
                        //    var _style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                        //    //_style.BackgroundColor = System.Drawing.Color.LightSkyBlue;
                        //    _style.ForegroundColor = System.Drawing.Color.FromArgb(230, 211, 211, 211);
                        //    _style.Pattern = Aspose.Cells.BackgroundType.Solid;
                        //    sheet.Cells[RowIndex, CellsIndex].SetStyle(_style);
                        //}
                        #endregion

                        sheet.Cells[RowIndex, CellsIndex].Value = obj;
                        CellsIndex++;
                    }
                    else
                    {
                        sheet.Cells[RowIndex, CellsIndex].Value = "";
                        CellsIndex++;
                    }
                    #endregion
                }

                RowIndex++;
            }
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/DownLoad/"));
            if (!dir.Exists)
                dir.Create();

            workbook.Save((dir.FullName + FileStartName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls"), Aspose.Cells.SaveFormat.Excel97To2003);
            return workbook.SaveToStream().ToArray();

        }

        /// <summary>
        /// 根据 数组导出 指定模板Excel 数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="ObjT">List<obj></param>
        /// <param name="ExcelModelPath">模板Excel位置</param>
        /// <param name="HeadLineNum">模板列表头行开始号</param>
        /// <param name="HeadLineNum">模板列表头列开始号</param>
        /// <returns></returns>
        public static byte[] OutPutExcelByListModel<T>(T ObjT, string ExcelModelPath, int HeadLineRowIndex, int HeadLineCellsIndex, out string FilePath, string FileStartName)//List<obj>
        {
            FilePath = "";

            //项目顶层命名空间
            string Top_NameSpace = System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"] == null ? "AirOut" : System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"].ToString();
            //网站顶层命名空间
            string WebTop_NameSpace = System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"] == null ? "AirOut.Web" : System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"].ToString();
            //项目类命名空间
            string Models_NameSpace = System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"] == null ? "Models" : System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"].ToString();

            string ModelsNameSpace = WebTop_NameSpace + "." + Models_NameSpace;//"AirOut.Web.Models"
            string ModelsTopNameSpace = Top_NameSpace + ".";

            //单元格 最大宽度
            double MaxCellWidth = 80;

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //如果 是 int 或者 decimal 或者 double 的 合计
            List<AirOut.Web.Models.TotalColumn> ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();

            ////为Aspose添加License注册
            //Aspose.Cells.License license = new Aspose.Cells.License();
            //SetAsposeLicense(license);

            FileInfo ExcelFile = new FileInfo(HttpContext.Current.Server.MapPath(ExcelModelPath));
            if (ExcelFile.Exists)
            {
                try
                {
                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ExcelFile.FullName);

                    #region 单元格样式3

                    Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                    CellStyle.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                    CellStyle.Font.Name = "宋体";//文字字体 
                    CellStyle.Font.Size = 10;//文字大小 
                    CellStyle.IsTextWrapped = true;//自动换行
                    CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                    #endregion

                    Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                    int RowIndex = HeadLineRowIndex;
                    int CellsIndex = HeadLineCellsIndex;

                    #region 获取Excel表头列名
                    Dictionary<string, int> ColumnNames = new Dictionary<string, int>();
                    string ColumnName = sheet.Cells[RowIndex, CellsIndex].StringValue;
                    var MergeColumnName = "";
                    while (!string.IsNullOrEmpty(ColumnName))
                    {
                        MergeColumnName = "";
                        ColumnNames.Add(ColumnName.Replace("\r\n", ""), CellsIndex);

                        #region  自动列宽

                        if (!string.IsNullOrEmpty(ColumnName))
                        {
                            System.Drawing.Font font = new System.Drawing.Font(CellStyle.Font.Name, CellStyle.Font.Size);
                            double StrWidth = 0;
                            StrWidth = g.MeasureString(ColumnName, font).Width / 7.384;
                            if (StrWidth > MaxCellWidth)
                            {
                                StrWidth = MaxCellWidth;
                            }
                            if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                            else if (RowIndex == HeadLineRowIndex)
                            {
                                sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                            }
                        }

                        #endregion

                        while (sheet.Cells[RowIndex, CellsIndex].IsMerged)
                        {
                            CellsIndex++;
                            MergeColumnName = sheet.Cells[RowIndex, CellsIndex].StringValue;
                            if (string.IsNullOrEmpty(MergeColumnName))
                            {
                                CellsIndex++;
                                MergeColumnName = sheet.Cells[RowIndex, CellsIndex].StringValue;
                            }
                            else if (MergeColumnName != ColumnName)
                                break;
                        }
                        if (MergeColumnName != ColumnName && !string.IsNullOrEmpty(MergeColumnName))
                        {
                            CellsIndex--;
                        }
                        CellsIndex++;
                        ColumnName = sheet.Cells[RowIndex, CellsIndex].StringValue;
                    }
                    #endregion

                    var _obj = ObjT as System.Collections.IList;
                    Type _ty = null;
                    if (_obj.Count > 0)
                    {
                        _ty = _obj[0].GetType();
                    }

                    Type t = _ty;
                    System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    //string ss = Common.GetDisplayName(_Employee.GetType(), "Id");
                    RowIndex++;
                    foreach (var item in _obj)
                    {
                        var IsToTal = false;//是否是 汇总行

                        #region 判断 是否是 汇总 行 如果是汇总 不需要 再总汇总中 累加

                        foreach (var itemColumn in ColumnNames)
                        {
                            CellsIndex = itemColumn.Value;
                            DisplayAttribute disAttr = null;
                            System.Reflection.PropertyInfo fi = null;

                            #region 是否 包含列

                            bool HasColumn = true;
                            foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                            {
                                HasColumn = false;
                                DisplayAttribute disAttritem = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                                if (itemColumn.Key == fiitem.Name)
                                    HasColumn = true;
                                if (disAttritem != null)
                                {
                                    if (itemColumn.Key == disAttritem.Name)
                                        HasColumn = true;
                                }
                                if (HasColumn)
                                {
                                    fi = fiitem;
                                    disAttr = disAttritem;
                                    break;
                                }
                            }

                            #endregion

                            if (fi != null)
                            {
                                object obj = fi.GetValue((object)item, null);
                                string str = "";
                                Type _type = null;
                                if (obj != null)
                                {
                                    str = obj.ToString();
                                    _type = obj.GetType();
                                }
                                var retVal = obj;

                                //是否是 汇总行
                                if (retVal != null)
                                {
                                    if (retVal.ToString().IndexOf("汇总") >= 0 || retVal.ToString().IndexOf("合计") >= 0)
                                    {
                                        IsToTal = true;
                                        break;
                                    }
                                }
                            }
                        }

                        #endregion

                        foreach (var itemColumn in ColumnNames)
                        {
                            CellsIndex = itemColumn.Value;
                            DisplayAttribute disAttr = null;
                            System.Reflection.PropertyInfo fi = null;

                            #region 是否 包含列

                            bool HasColumn = true;
                            foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                            {
                                HasColumn = false;
                                DisplayAttribute disAttritem = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                                if (itemColumn.Key == fiitem.Name)
                                    HasColumn = true;
                                if (disAttritem != null)
                                {
                                    if (itemColumn.Key == disAttritem.Name)
                                        HasColumn = true;
                                }
                                if (HasColumn)
                                {
                                    fi = fiitem;
                                    disAttr = disAttritem;
                                    break;
                                }
                            }

                            #endregion

                            #region 添加列

                            if (fi != null)
                            {
                                object obj = fi.GetValue((object)item, null);
                                string str = "";
                                Type _type = null;
                                if (obj != null)
                                {
                                    str = obj.ToString();
                                    _type = obj.GetType();
                                }

                                #region 如果是 数组

                                if (str.IndexOf(ModelsTopNameSpace) >= 0 || str.IndexOf("System.") >= 0)
                                {
                                    try
                                    {
                                        if (_type != null)
                                        {
                                            if (str.IndexOf(ModelsTopNameSpace) >= 0 && _type.Name.IndexOf("HashSet") < 0)
                                            {
                                                string FieldName = "Name";
                                                System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                                var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                                if (_PropertyInfos.Any())
                                                {
                                                    obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null);
                                                }
                                            }
                                            if (str.IndexOf("System.Collections.") >= 0)
                                            {
                                                string FieldName = "Count";
                                                System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                                var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                                if (_PropertyInfos.Any())
                                                {
                                                    string ModelName = obj.ToString();
                                                    try
                                                    {
                                                        if (disAttr != null)
                                                            ModelName = disAttr.Name;
                                                        else
                                                        {
                                                            string Split = ModelsNameSpace + ".";
                                                            int Sindex = ModelName.IndexOf(Split);
                                                            if (Sindex > 0)
                                                            {
                                                                ModelName = ModelName.Substring(Sindex + Split.Length);
                                                                if (ModelName.IndexOf(']') >= 0)
                                                                {
                                                                    ModelName = ModelName.Substring(0, ModelName.IndexOf(']'));
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {

                                                    }
                                                    obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null).ToString() + "-" + ModelName;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                                #endregion

                                sheet.Cells[RowIndex, CellsIndex].SetStyle(CellStyle);
                                var style = CellStyle;
                                style.Font.Color = System.Drawing.Color.Black;

                                var retVal = obj;

                                string DataType = fi.PropertyType.Name;
                                if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = fi.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;
                                }

                                //int decimal double float 统计 零时保存数组
                                var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");

                                #region 根据 数据类型 显示值和设置列宽
                                if (retVal != null)
                                {
                                    switch (DataType.ToLower())
                                    {
                                        case "int":
                                            int Dftint = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint))
                                            {
                                                style.Number = 1;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp = wheredictTotal.FirstOrDefault();
                                                int _Total = 0;
                                                if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                {
                                                    Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                Temp.ColumnName = itemColumn.Key;
                                                Temp.ColumnIndex = itemColumn.Value;
                                                Temp.ColumnTotal = Dftint.ToString();
                                                ArrColumnTotal.Add(Temp);
                                            }

                                            #endregion
                                            break;
                                        case "int32":
                                            int Dftint32 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint32))
                                            {
                                                style.Number = 1;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp32 = wheredictTotal.FirstOrDefault();
                                                int _Total32 = 0;
                                                if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                {
                                                    Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                Temp32.ColumnName = itemColumn.Key;
                                                Temp32.ColumnIndex = itemColumn.Value;
                                                Temp32.ColumnTotal = Dftint32.ToString();
                                                ArrColumnTotal.Add(Temp32);
                                            }

                                            #endregion
                                            break;
                                        case "int64":
                                            int Dftint64 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint64))
                                            {
                                                style.Number = 1;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp64 = wheredictTotal.FirstOrDefault();
                                                int _Total64 = 0;
                                                if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                {
                                                    Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                Temp64.ColumnName = itemColumn.Key;
                                                Temp64.ColumnIndex = itemColumn.Value;
                                                Temp64.ColumnTotal = Dftint64.ToString();
                                                ArrColumnTotal.Add(Temp64);
                                            }

                                            #endregion
                                            break;
                                        case "string":
                                            style.Number = 9;
                                            //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            break;
                                        case "datetime":
                                            int TDatetime = 0;
                                            if (int.TryParse(retVal.ToString(), out TDatetime))
                                            {
                                                style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else
                                            {
                                                DateTime DftDateTime = new DateTime();
                                                if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                {
                                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                            }
                                            break;
                                        case "bool":
                                            bool Dftbool = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftbool))
                                            {
                                                style.Number = 9;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                if (Dftbool)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "boolean":
                                            bool Dftboolean = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                            {
                                                style.Number = 9;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                if (Dftboolean)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "decimal":
                                            decimal Dftdecimal = 0;
                                            if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                            {
                                                style.Number = 1;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (itemColumn.Key.IndexOf("单价") >= 0)
                                            {
                                                //obj = Dftdecimal.ToString("f4");
                                                style.Custom = "0.0000";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0)
                                            {
                                                //obj = Dftdecimal.ToString("f2");
                                                style.Custom = "0.00";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                decimal _Totaldecimal = 0;
                                                if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                {
                                                    Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                Tempdecima.ColumnName = itemColumn.Key;
                                                Tempdecima.ColumnIndex = itemColumn.Value;
                                                Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                ArrColumnTotal.Add(Tempdecima);
                                            }

                                            #endregion
                                            break;
                                        case "double":
                                            double Dftdouble = 0;
                                            if (double.TryParse(retVal.ToString(), out Dftdouble))
                                            {
                                                style.Number = 1;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (itemColumn.Key.IndexOf("单价") >= 0)
                                            {
                                                //obj = Dftdecimal.ToString("f4");
                                                style.Custom = "0.0000";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0)
                                            {
                                                //obj = Dftdecimal.ToString("f2");
                                                style.Custom = "0.00";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdouble = wheredictTotal.FirstOrDefault();
                                                double _Totaldouble = 0;
                                                if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                {
                                                    Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                Tempdouble.ColumnName = itemColumn.Key;
                                                Tempdouble.ColumnIndex = itemColumn.Value;
                                                Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                ArrColumnTotal.Add(Tempdouble);
                                            }

                                            #endregion
                                            break;
                                        case "float":
                                            float Dftfloat = 0;
                                            if (float.TryParse(retVal.ToString(), out Dftfloat))
                                            {
                                                style.Number = 1;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (itemColumn.Key.IndexOf("单价") >= 0)
                                            {
                                                //obj = Dftdecimal.ToString("f4");
                                                style.Custom = "0.0000";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0)
                                            {
                                                //obj = Dftdecimal.ToString("f2");
                                                style.Custom = "0.00";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempfloat = wheredictTotal.FirstOrDefault();
                                                float _Totalfloat = 0;
                                                if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                {
                                                    Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                Tempfloat.ColumnName = itemColumn.Key;
                                                Tempfloat.ColumnIndex = itemColumn.Value;
                                                Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                ArrColumnTotal.Add(Tempfloat);
                                            }

                                            #endregion
                                            break;
                                        default:
                                            style.Number = 9;
                                            //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            break;
                                    }
                                }

                                #endregion

                                #region  自动列宽

                                if (obj != null)
                                {
                                    System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                                    double StrWidth = 0;
                                    StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                                    if (StrWidth > MaxCellWidth)
                                    {
                                        StrWidth = MaxCellWidth;
                                    }
                                    if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                        sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                }

                                #endregion

                                #region 设置背景色
                                //else
                                //{
                                //    var _style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                                //    //_style.BackgroundColor = System.Drawing.Color.LightSkyBlue;
                                //    _style.ForegroundColor = System.Drawing.Color.FromArgb(230, 211, 211, 211);
                                //    _style.Pattern = Aspose.Cells.BackgroundType.Solid;
                                //    sheet.Cells[RowIndex, CellsIndex].SetStyle(_style);
                                //}
                                #endregion

                                if (itemColumn.Key == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineCellsIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = obj;
                                //CellsIndex++;
                            }
                            else
                            {
                                if (itemColumn.Key == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineCellsIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = "";
                                //CellsIndex++;
                            }
                            #endregion
                        }

                        RowIndex++;
                    }
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/DownLoad/"));
                    if (!dir.Exists)
                        dir.Create();

                    //统计 赋值
                    if (ArrColumnTotal.Any())
                    {
                        int NumIndex = 0;
                        foreach (var itemTotal in ArrColumnTotal.OrderBy(x => x.ColumnIndex))
                        {
                            //var style = sheet.Cells[RowIndex, itemTotal.ColumnIndex].GetStyle();
                            var style = CellStyle;
                            if (NumIndex == 0)
                            {
                                if (itemTotal.ColumnIndex > 0)
                                {
                                    sheet.Cells[RowIndex, itemTotal.ColumnIndex - 1].Value = "合计：";
                                }
                                else
                                {
                                    int ColIndex = itemTotal.ColumnIndex;
                                    while (ArrColumnTotal.Any(x => x.ColumnIndex == ColIndex))
                                    {
                                        ColIndex++;
                                    }

                                    sheet.Cells[RowIndex, ColIndex].Value = "合计：";
                                }
                            }
                            object objVal = itemTotal.ColumnTotal;

                            #region 截取小数位
                            if (itemTotal.ColumnName == "序号")
                            {
                                objVal = "";
                            }
                            else if (itemTotal.ColumnName.IndexOf("单价") >= 0)
                            {
                                //var indexpoint = objVal.IndexOf(".") + 1;
                                //var objlen = objVal.Length;
                                //if (indexpoint > 0)
                                //{
                                //    if (objlen > (indexpoint + 4))
                                //        objVal = objVal.Substring(0, indexpoint + 4);
                                //    else
                                //        objVal = objVal.Substring(0, objlen);
                                //}
                                decimal dcparse = 0;
                                if (decimal.TryParse(objVal.ToString(), out dcparse))
                                {
                                    objVal = (object)dcparse;
                                }
                                style.Custom = "0.0000";
                                sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                            }
                            else if (itemTotal.ColumnName.IndexOf("毛重") >= 0 || itemTotal.ColumnName.IndexOf("净重") >= 0 || itemTotal.ColumnName.IndexOf("金额") >= 0)
                            {
                                //var indexpoint = objVal.IndexOf(".") + 1;
                                //var objlen = objVal.Length;
                                //if (indexpoint > 0)
                                //{
                                //    if (objlen > (indexpoint + LevePointNum))
                                //        objVal = objVal.Substring(0, indexpoint + LevePointNum);
                                //    else
                                //        objVal = objVal.Substring(0, objlen);
                                //}
                                decimal dcparse = 0;
                                if (decimal.TryParse(objVal.ToString(), out dcparse))
                                {
                                    objVal = (object)dcparse;
                                }
                                style.Custom = "0.00";
                            }
                            else if (itemTotal.ColumnName.IndexOf("件数") >= 0)
                            {
                                style.Custom = "0";
                                int dcparse = 0;
                                if (int.TryParse(objVal.ToString(), out dcparse))
                                {
                                    objVal = (object)dcparse;
                                }
                            }
                            #endregion

                            if (objVal.ToString() == "0.0000" || objVal.ToString() == "0.000" || objVal.ToString() == "0.00" || objVal.ToString() == "0.0" || objVal.ToString() == "0")
                                objVal = "";

                            style.Font.Color = System.Drawing.Color.Red;
                            sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                            sheet.Cells[RowIndex, itemTotal.ColumnIndex].Value = objVal;
                            NumIndex++;
                        }
                    }
                    FilePath = (dir.FullName + FileStartName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff_") + new Random().Next(1, 10).ToString("00") + ".xls");
                    workbook.Save(FilePath, Aspose.Cells.SaveFormat.Excel97To2003);
                    return workbook.SaveToStream().ToArray();
                }
                catch (Exception)
                {
                    return OutPutExcelByListModel(ObjT);
                }
            }
            else
            {
                return OutPutExcelByListModel(ObjT);
            }
        }

        /// <summary>
        /// 根据 数组导出 指定模板Excel 数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="ObjT">List<obj></param>
        /// <param name="ExcelModelPath">模板Excel位置</param>
        /// <param name="HeadLineNum">模板列表头行开始号</param>
        /// <param name="HeadLineNum">模板列表头列开始号</param>
        /// <returns></returns>
        public static byte[] OutPutExcelByListModel<T>(T ObjT, bool MoreSheet, string ExcelModelPath, int HeadLineRowIndex, int HeadLineCellsIndex, out string FilePath, string FileStartName)//List<obj>
        {
            FilePath = "";

            //项目顶层命名空间
            string Top_NameSpace = System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"] == null ? "AirOut" : System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"].ToString();
            //网站顶层命名空间
            string WebTop_NameSpace = System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"] == null ? "AirOut.Web" : System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"].ToString();
            //项目类命名空间
            string Models_NameSpace = System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"] == null ? "Models" : System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"].ToString();

            string ModelsNameSpace = WebTop_NameSpace + "." + Models_NameSpace;//"AirOut.Web.Models";
            string ModelsTopNameSpace = Top_NameSpace + ".";// "AirOut.";
            //单元格 最大宽度
            double MaxCellWidth = 80;

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //如果 是 int 或者 decimal 或者 double 的 合计
            List<AirOut.Web.Models.TotalColumn> ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();

            Aspose.Cells.License license = new Aspose.Cells.License();
            SetAsposeLicense(license);
            FileInfo ExcelFile = new FileInfo(HttpContext.Current.Server.MapPath(ExcelModelPath));
            if (ExcelFile.Exists)
            {
                try
                {
                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ExcelFile.FullName);
                    //Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];

                    #region 单元格样式3

                    Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                    CellStyle.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                    CellStyle.Font.Name = "宋体";//文字字体 
                    CellStyle.Font.Size = 10;//文字大小 
                    CellStyle.IsTextWrapped = true;//自动换行
                    CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                    #endregion

                    var _objArrSheet = ObjT as System.Collections.IList;
                    if (MoreSheet)
                    {
                        if (_objArrSheet.Count > 1)
                            CopySheet(workbook, _objArrSheet.Count - 1);
                    }
                    List<Aspose.Cells.Worksheet> Sheet_s = new List<Aspose.Cells.Worksheet>();
                    for (int i = 0; i < workbook.Worksheets.Count; i++)
                    {
                        Sheet_s.Add(workbook.Worksheets[i]);
                    }
                    //最大列序号
                    int MaxCells = 0;

                    #region 获取Excel表头列名

                    Dictionary<string, int> ColumnNames = new Dictionary<string, int>();
                    if (Sheet_s.Any())
                    {
                        var sheet = Sheet_s.FirstOrDefault();
                        int RowIndex = HeadLineRowIndex;
                        int CellsIndex = HeadLineCellsIndex;
                        string ColumnName = sheet.Cells[RowIndex, CellsIndex].StringValue;
                        var MergeColumnName = "";
                        var HeadStyle = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                        while (!string.IsNullOrEmpty(ColumnName))
                        {
                            MergeColumnName = "";
                            ColumnNames.Add(ColumnName.Replace("\r\n", ""), CellsIndex);

                            #region  自动列宽

                            if (!string.IsNullOrEmpty(ColumnName))
                            {
                                System.Drawing.Font font = new System.Drawing.Font(HeadStyle.Font.Name, HeadStyle.Font.Size);
                                double StrWidth = 0;
                                StrWidth = g.MeasureString(ColumnName, font).Width / 7.384;
                                if (StrWidth > MaxCellWidth)
                                {
                                    StrWidth = MaxCellWidth;
                                }
                                if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                    sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                else if (RowIndex == HeadLineRowIndex)
                                {
                                    sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                }
                            }

                            #endregion

                            while (sheet.Cells[RowIndex, CellsIndex].IsMerged)
                            {
                                CellsIndex++;
                                MergeColumnName = sheet.Cells[RowIndex, CellsIndex].StringValue;

                                if (string.IsNullOrEmpty(MergeColumnName))
                                {
                                    CellsIndex++;
                                    MergeColumnName = sheet.Cells[RowIndex, CellsIndex].StringValue;
                                }
                                else if (MergeColumnName != ColumnName)
                                    break;
                            }
                            if (MergeColumnName != ColumnName && !string.IsNullOrEmpty(MergeColumnName))
                            {
                                CellsIndex--;
                            }
                            CellsIndex++;
                            if (CellsIndex > MaxCells)
                                MaxCells = CellsIndex;
                            ColumnName = sheet.Cells[RowIndex, CellsIndex].StringValue;
                            HeadStyle = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                        }
                    }

                    #endregion

                    int index = 0;
                    int StartNum = 0;
                    int EndNum = 0;
                    foreach (var itemObj in _objArrSheet)
                    {
                        ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();
                        Aspose.Cells.Worksheet sheet = Sheet_s[index];

                        object objList = null;

                        #region 获取List 数据

                        System.Reflection.PropertyInfo[] itemPropertyInfos = itemObj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        foreach (System.Reflection.PropertyInfo fiitem in itemPropertyInfos)
                        {
                            object obj = fiitem.GetValue((object)itemObj, null);
                            string str = "";
                            Type _type = null;
                            if (obj != null)
                            {
                                str = obj.ToString();
                                _type = obj.GetType();
                            }
                            if (str.IndexOf("TMSApp.") >= 0 || str.IndexOf("System.") >= 0)
                            {
                                if (_type != null)
                                {
                                    if (str.IndexOf("System.Collections.") >= 0)
                                    {
                                        objList = obj;
                                    }
                                }
                            }
                        }

                        #endregion

                        #region  每个Sheet 添加值

                        int RowIndex = HeadLineRowIndex;
                        int CellsIndex = HeadLineCellsIndex;

                        if (objList != null)
                        {
                            var _obj = objList as System.Collections.IList;
                            if (index == 0)
                            {
                                if (_obj.Count > 0)
                                    StartNum = 1;
                                else
                                    StartNum = 0;
                            }
                            else
                                StartNum = EndNum;
                            EndNum += _obj.Count;
                            Type _ty = null;
                            if (_obj.Count > 0)
                            {
                                if (objList != null)
                                    sheet.Name += "(" + StartNum.ToString() + "-" + EndNum.ToString() + ")";
                                _ty = _obj[0].GetType();
                            }

                            Type t = _ty;
                            System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                            //string ss = Common.GetDisplayName(_Employee.GetType(), "Id");
                            RowIndex++;
                            foreach (var item in _obj)
                            {
                                var IsToTal = false;//是否是 汇总行

                                #region 设置列格式

                                for (int i = 0; i < MaxCells; i++)
                                {
                                    CellStyle.Font.Color = System.Drawing.Color.Black;
                                    CellStyle.Number = 0;//设置默认单元格格式为 常规
                                    sheet.Cells[RowIndex, i].SetStyle(CellStyle);
                                }

                                #endregion

                                #region 判断 是否是 汇总 行 如果是汇总 不需要 再总汇总中 累加

                                foreach (var itemColumn in ColumnNames)
                                {
                                    CellsIndex = itemColumn.Value;
                                    DisplayAttribute disAttr = null;
                                    System.Reflection.PropertyInfo fi = null;

                                    #region 是否 包含列

                                    bool HasColumn = true;
                                    foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                                    {
                                        HasColumn = false;
                                        DisplayAttribute disAttritem = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                                        if (itemColumn.Key == fiitem.Name)
                                            HasColumn = true;
                                        if (disAttritem != null)
                                        {
                                            if (itemColumn.Key == disAttritem.Name)
                                                HasColumn = true;
                                        }
                                        if (HasColumn)
                                        {
                                            fi = fiitem;
                                            disAttr = disAttritem;
                                            break;
                                        }
                                    }

                                    #endregion

                                    if (fi != null)
                                    {
                                        object obj = fi.GetValue((object)item, null);
                                        string str = "";
                                        Type _type = null;
                                        if (obj != null)
                                        {
                                            str = obj.ToString();
                                            _type = obj.GetType();
                                        }
                                        var retVal = obj;

                                        //是否是 汇总行
                                        if (retVal != null)
                                        {
                                            if (retVal.ToString().IndexOf("汇总") >= 0 || retVal.ToString().IndexOf("合计") >= 0)
                                            {
                                                IsToTal = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                #endregion

                                foreach (var itemColumn in ColumnNames)
                                {
                                    CellsIndex = itemColumn.Value;
                                    DisplayAttribute disAttr = null;
                                    System.Reflection.PropertyInfo fi = null;

                                    #region 是否 包含列

                                    bool HasColumn = true;
                                    foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                                    {
                                        HasColumn = false;
                                        DisplayAttribute disAttritem = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                                        if (itemColumn.Key == fiitem.Name)
                                            HasColumn = true;
                                        if (disAttritem != null)
                                        {
                                            if (itemColumn.Key == disAttritem.Name)
                                                HasColumn = true;
                                        }
                                        if (HasColumn)
                                        {
                                            fi = fiitem;
                                            disAttr = disAttritem;
                                            break;
                                        }
                                    }

                                    #endregion

                                    #region 添加列

                                    if (fi != null)
                                    {
                                        object obj = fi.GetValue((object)item, null);
                                        string str = "";
                                        Type _type = null;
                                        if (obj != null)
                                        {
                                            str = obj.ToString();
                                            _type = obj.GetType();
                                        }

                                        #region 如果是 数组

                                        if (str.IndexOf(ModelsTopNameSpace) >= 0 || str.IndexOf("System.") >= 0)
                                        {
                                            try
                                            {
                                                if (_type != null)
                                                {
                                                    if (str.IndexOf(ModelsTopNameSpace) >= 0 && _type.Name.IndexOf("HashSet") < 0)
                                                    {
                                                        string FieldName = "Name";
                                                        System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                                        var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                                        if (_PropertyInfos.Any())
                                                        {
                                                            obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null);
                                                        }
                                                    }
                                                    if (str.IndexOf("System.Collections.") >= 0)
                                                    {
                                                        string FieldName = "Count";
                                                        System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                                        var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                                        if (_PropertyInfos.Any())
                                                        {
                                                            string ModelName = obj.ToString();
                                                            try
                                                            {
                                                                if (disAttr != null)
                                                                    ModelName = disAttr.Name;
                                                                else
                                                                {
                                                                    string Split = ModelsNameSpace + ".";
                                                                    int Sindex = ModelName.IndexOf(Split);
                                                                    if (Sindex > 0)
                                                                    {
                                                                        ModelName = ModelName.Substring(Sindex + Split.Length);
                                                                        if (ModelName.IndexOf(']') >= 0)
                                                                        {
                                                                            ModelName = ModelName.Substring(0, ModelName.IndexOf(']'));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            catch
                                                            {

                                                            }
                                                            obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null).ToString() + "-" + ModelName;
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }

                                        #endregion

                                        //CellStyle.Font.Color = System.Drawing.Color.Black;
                                        //CellStyle.Number = 0;//设置默认单元格格式为 常规
                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(CellStyle);
                                        //var style = CellStyle;
                                        var style = sheet.Cells[RowIndex, CellsIndex].GetStyle();

                                        #region 根据单位 设置 数量格式  成交数量 法定数量 第二数量
                                        object objss = null;
                                        switch (itemColumn.Key)
                                        {
                                            case "申报数量":
                                                #region  申报数量

                                                objss = GetProtityValue(item, "申报单位");
                                                if (objss != null)
                                                {
                                                    if (objss.ToString().ToUpper().Trim() == "千个")
                                                    {
                                                        style.Custom = "0.000";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString().ToUpper().Trim() == "千克" || objss.ToString().ToUpper().Trim() == "KG")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString().ToUpper().Trim() == "克" || objss.ToString().ToUpper().Trim() == "G")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (",个,块,台,张,".IndexOf("," + objss.ToString().ToUpper().Trim().Replace(" ", "").Replace(",", "，") + ",") >= 0)
                                                    {
                                                        style.Custom = "0";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                }

                                                #endregion
                                                break;
                                            case "成交数量":
                                                #region  成交数量

                                                objss = GetProtityValue(item, "成交单位");
                                                if (objss != null)
                                                {
                                                    if (objss.ToString().ToUpper().Trim() == "千个")
                                                    {
                                                        style.Custom = "0.000";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString().ToUpper().Trim() == "千克" || objss.ToString().ToUpper().Trim() == "KG")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString().ToUpper().Trim() == "克" || objss.ToString().ToUpper().Trim() == "G")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (",个,块,台,张,".IndexOf("," + objss.ToString().ToUpper().Trim().Replace(" ", "").Replace(",", "，") + ",") >= 0)
                                                    {
                                                        style.Custom = "0";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                }

                                                #endregion
                                                break;
                                            case "法定数量":
                                                #region  法定数量

                                                objss = GetProtityValue(item, "法定单位");
                                                if (objss != null)
                                                {
                                                    if (objss.ToString() == "千个")
                                                    {
                                                        style.Custom = "0.000";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString() == "千克" || objss.ToString().ToUpper().Trim() == "KG")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString().ToUpper().Trim() == "克" || objss.ToString().ToUpper().Trim() == "G")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (",个,块,台,张,".IndexOf("," + objss.ToString().ToUpper().Trim().Replace(" ", "").Replace(",", "，") + ",") >= 0)
                                                    {
                                                        style.Custom = "0";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                }

                                                #endregion
                                                break;
                                            case "第二数量":
                                                #region  第二数量

                                                objss = GetProtityValue(item, "第二单位");
                                                if (objss != null)
                                                {
                                                    if (objss.ToString() == "千个")
                                                    {
                                                        style.Custom = "0.000";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString() == "千克" || objss.ToString().ToUpper().Trim() == "KG")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString().ToUpper().Trim() == "克" || objss.ToString().ToUpper().Trim() == "G")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (",个,块,台,张,".IndexOf("," + objss.ToString().ToUpper().Trim().Replace(" ", "").Replace(",", "，") + ",") >= 0)
                                                    {
                                                        style.Custom = "0";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                }

                                                #endregion
                                                break;
                                            case "总数量(PCS)":
                                                #region  总数量(PCS)

                                                objss = GetProtityValue(item, "申报单位");
                                                if (objss != null)
                                                {
                                                    if (objss.ToString().ToUpper().Trim() == "千个")
                                                    {
                                                        style.Custom = "0.000";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString().ToUpper().Trim() == "千克" || objss.ToString().ToUpper().Trim() == "KG")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (objss.ToString().ToUpper().Trim() == "克" || objss.ToString().ToUpper().Trim() == "G")
                                                    {
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (",个,块,台,张,".IndexOf("," + objss.ToString().ToUpper().Trim().Replace(" ", "").Replace(",", "，") + ",") >= 0)
                                                    {
                                                        style.Custom = "0";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                }

                                                #endregion
                                                break;
                                            default:
                                                break;
                                        }
                                        #endregion

                                        var retVal = obj;

                                        string DataType = fi.PropertyType.Name;
                                        if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                        {
                                            var Arguments = fi.PropertyType.GetGenericArguments();
                                            DataType = Arguments[0].Name;
                                        }

                                        //int decimal double float 统计 零时保存数组
                                        var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");

                                        #region 根据 数据类型 显示值和设置列宽

                                        if (retVal != null)
                                        {
                                            switch (DataType.ToLower())
                                            {
                                                case "int":
                                                    int Dftint = 0;
                                                    if (int.TryParse(retVal.ToString(), out Dftint))
                                                    {
                                                        style.Number = 1;
                                                        if (Dftint == 0)
                                                            obj = "";
                                                        //style.Font.Color = System.Drawing.Color.Blue;
                                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (IsToTal && Dftint == 0)//如果是 汇总行 不累加到 总汇总中
                                                        obj = "";
                                                    #region int decimal double 类型 汇总

                                                    if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                        break;
                                                    if (itemColumn.Key == "成交数量" || itemColumn.Key == "法定数量" || itemColumn.Key == "第二数量" || itemColumn.Key == "单价")//成交数量 不需要汇总
                                                        break;
                                                    wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                    if (wheredictTotal.Any())
                                                    {
                                                        var Temp = wheredictTotal.FirstOrDefault();
                                                        int _Total = 0;
                                                        if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                        {
                                                            Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                        Temp.ColumnName = itemColumn.Key;
                                                        Temp.ColumnIndex = itemColumn.Value;
                                                        Temp.ColumnTotal = Dftint.ToString();
                                                        ArrColumnTotal.Add(Temp);
                                                    }

                                                    #endregion
                                                    break;
                                                case "int32":
                                                    int Dftint32 = 0;
                                                    if (int.TryParse(retVal.ToString(), out Dftint32))
                                                    {
                                                        style.Number = 1;
                                                        if (Dftint32 == 0)
                                                            obj = "";
                                                        //style.Font.Color = System.Drawing.Color.Blue;
                                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (IsToTal && Dftint32 == 0)//如果是 汇总行 不累加到 总汇总中
                                                        obj = "";
                                                    #region int decimal double 类型 汇总

                                                    if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                        break;
                                                    if (itemColumn.Key == "成交数量" || itemColumn.Key == "法定数量" || itemColumn.Key == "第二数量" || itemColumn.Key == "单价")//成交数量 不需要汇总
                                                        break;
                                                    wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                    if (wheredictTotal.Any())
                                                    {
                                                        var Temp32 = wheredictTotal.FirstOrDefault();
                                                        int _Total32 = 0;
                                                        if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                        {
                                                            Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                        Temp32.ColumnName = itemColumn.Key;
                                                        Temp32.ColumnIndex = itemColumn.Value;
                                                        Temp32.ColumnTotal = Dftint32.ToString();
                                                        ArrColumnTotal.Add(Temp32);
                                                    }

                                                    #endregion
                                                    break;
                                                case "int64":
                                                    int Dftint64 = 0;
                                                    if (int.TryParse(retVal.ToString(), out Dftint64))
                                                    {
                                                        style.Number = 1;
                                                        if (Dftint64 == 0)
                                                            obj = "";
                                                        //style.Font.Color = System.Drawing.Color.Blue;
                                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (IsToTal && Dftint64 == 0)//如果是 汇总行 不累加到 总汇总中
                                                        obj = "";
                                                    #region int decimal double 类型 汇总

                                                    if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                        break;
                                                    if (itemColumn.Key == "成交数量" || itemColumn.Key == "法定数量" || itemColumn.Key == "第二数量" || itemColumn.Key == "单价")//成交数量 不需要汇总
                                                        break;
                                                    wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                    if (wheredictTotal.Any())
                                                    {
                                                        var Temp64 = wheredictTotal.FirstOrDefault();
                                                        int _Total64 = 0;
                                                        if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                        {
                                                            Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                        Temp64.ColumnName = itemColumn.Key;
                                                        Temp64.ColumnIndex = itemColumn.Value;
                                                        Temp64.ColumnTotal = Dftint64.ToString();
                                                        ArrColumnTotal.Add(Temp64);
                                                    }

                                                    #endregion
                                                    break;
                                                case "string":
                                                    style.Number = 9;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    break;
                                                case "datetime":
                                                    int TDatetime = 0;
                                                    if (int.TryParse(retVal.ToString(), out TDatetime))
                                                    {
                                                        style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    else
                                                    {
                                                        DateTime DftDateTime = new DateTime();
                                                        if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                        {
                                                            style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        }
                                                    }
                                                    break;
                                                case "bool":
                                                    bool Dftbool = false;
                                                    if (bool.TryParse(retVal.ToString(), out Dftbool))
                                                    {
                                                        style.Number = 9;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        if (Dftbool)
                                                        {
                                                            style.Font.Color = System.Drawing.Color.Green;
                                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                            obj = "是";
                                                        }
                                                        else
                                                        {
                                                            style.Font.Color = System.Drawing.Color.Red;
                                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                            obj = "否";
                                                        }
                                                    }
                                                    break;
                                                case "boolean":
                                                    bool Dftboolean = false;
                                                    if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                                    {
                                                        style.Number = 9;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        if (Dftboolean)
                                                        {
                                                            style.Font.Color = System.Drawing.Color.Green;
                                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                            obj = "是";
                                                        }
                                                        else
                                                        {
                                                            style.Font.Color = System.Drawing.Color.Red;
                                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                            obj = "否";
                                                        }
                                                    }
                                                    break;
                                                case "decimal":
                                                    decimal Dftdecimal = 0;
                                                    if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                                    {
                                                        //style.Number = 1;
                                                        if (Dftdecimal == 0)
                                                            obj = "";
                                                        //style.Font.Color = System.Drawing.Color.Pink;
                                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    //if (itemColumn.Key.IndexOf("净重") >= 0)
                                                    //{
                                                    //    obj = Dftdecimal.ToString("f2");
                                                    //}
                                                    //if (itemColumn.Key.IndexOf("毛重") >= 0)
                                                    //{
                                                    //    obj = Dftdecimal.ToString("f2");
                                                    //}
                                                    //if (itemColumn.Key.IndexOf("总价值") >= 0)
                                                    //{
                                                    //    obj = Dftdecimal.ToString("f2");
                                                    //}
                                                    if (itemColumn.Key.IndexOf("单价") >= 0)
                                                    {
                                                        //obj = Dftdecimal.ToString("f4");
                                                        style.Custom = "0.0000";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0 || itemColumn.Key.IndexOf("总价值") >= 0)
                                                    {
                                                        //obj = Dftdecimal.ToString("f2");
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                        obj = "";
                                                    #region int decimal double 类型 汇总

                                                    if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                        break;
                                                    if (itemColumn.Key == "成交数量" || itemColumn.Key == "法定数量" || itemColumn.Key == "第二数量" || itemColumn.Key == "单价")//成交数量 不需要汇总
                                                        break;
                                                    wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                    if (wheredictTotal.Any())
                                                    {
                                                        var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                        decimal _Totaldecimal = 0;
                                                        if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                        {
                                                            Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                        Tempdecima.ColumnName = itemColumn.Key;
                                                        Tempdecima.ColumnIndex = itemColumn.Value;
                                                        Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                        ArrColumnTotal.Add(Tempdecima);
                                                    }

                                                    #endregion
                                                    break;
                                                case "double":
                                                    double Dftdouble = 0;
                                                    if (double.TryParse(retVal.ToString(), out Dftdouble))
                                                    {
                                                        style.Number = 1;
                                                        if (Dftdouble == 0)
                                                            obj = "";
                                                        //style.Font.Color = System.Drawing.Color.Brown;
                                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (itemColumn.Key.IndexOf("单价") >= 0)
                                                    {
                                                        //obj = Dftdouble.ToString("f4");
                                                        style.Custom = "0.0000";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0 || itemColumn.Key.IndexOf("总价值") >= 0)
                                                    {
                                                        //obj = Dftdouble.ToString("f4");
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                        obj = "";
                                                    #region int decimal double 类型 汇总

                                                    if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                        break;
                                                    if (itemColumn.Key == "成交数量" || itemColumn.Key == "法定数量" || itemColumn.Key == "第二数量" || itemColumn.Key == "单价")//成交数量 不需要汇总
                                                        break;
                                                    wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                    if (wheredictTotal.Any())
                                                    {
                                                        var Tempdouble = wheredictTotal.FirstOrDefault();
                                                        double _Totaldouble = 0;
                                                        if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                        {
                                                            Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                        Tempdouble.ColumnName = itemColumn.Key;
                                                        Tempdouble.ColumnIndex = itemColumn.Value;
                                                        Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                        ArrColumnTotal.Add(Tempdouble);
                                                    }

                                                    #endregion
                                                    break;
                                                case "float":
                                                    float Dftfloat = 0;
                                                    if (float.TryParse(retVal.ToString(), out Dftfloat))
                                                    {
                                                        style.Number = 1;
                                                        if (Dftfloat == 0)
                                                            obj = "";
                                                        //style.Font.Color = System.Drawing.Color.Brown;
                                                        //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (itemColumn.Key.IndexOf("单价") >= 0)
                                                    {
                                                        //obj = Dftfloat.ToString("f4");
                                                        style.Custom = "0.0000";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0 || itemColumn.Key.IndexOf("总价值") >= 0)
                                                    {
                                                        //obj = Dftfloat.ToString("f2");
                                                        style.Custom = "0.00";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                    if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                        obj = "";
                                                    #region int decimal double 类型 汇总

                                                    if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                        break;
                                                    if (itemColumn.Key == "成交数量" || itemColumn.Key == "法定数量" || itemColumn.Key == "第二数量" || itemColumn.Key == "单价")//成交数量 不需要汇总
                                                        break;
                                                    wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                    if (wheredictTotal.Any())
                                                    {
                                                        var Tempfloat = wheredictTotal.FirstOrDefault();
                                                        float _Totalfloat = 0;
                                                        if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                        {
                                                            Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                        Tempfloat.ColumnName = itemColumn.Key;
                                                        Tempfloat.ColumnIndex = itemColumn.Value;
                                                        Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                        ArrColumnTotal.Add(Tempfloat);
                                                    }

                                                    #endregion
                                                    break;
                                                default:
                                                    style.Number = 9;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    break;
                                            }
                                        }

                                        #endregion

                                        #region  自动列宽

                                        if (obj != null)
                                        {
                                            System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                                            double StrWidth = 0;
                                            StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                                            if (StrWidth > MaxCellWidth)
                                            {
                                                StrWidth = MaxCellWidth;
                                            }
                                            if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                                sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                        }

                                        #endregion

                                        #region 设置背景色
                                        //else
                                        //{
                                        //    var _style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                                        //    //_style.BackgroundColor = System.Drawing.Color.LightSkyBlue;
                                        //    _style.ForegroundColor = System.Drawing.Color.FromArgb(230, 211, 211, 211);
                                        //    _style.Pattern = Aspose.Cells.BackgroundType.Solid;
                                        //    sheet.Cells[RowIndex, CellsIndex].SetStyle(_style);
                                        //}
                                        #endregion

                                        if (itemColumn.Key == "序号")
                                            sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineRowIndex;
                                        else
                                            sheet.Cells[RowIndex, CellsIndex].Value = obj;
                                        //CellsIndex++;
                                    }
                                    else
                                    {
                                        if (itemColumn.Key == "序号")
                                            sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineRowIndex;
                                        else
                                            sheet.Cells[RowIndex, CellsIndex].Value = "";
                                        //CellsIndex++;
                                    }
                                    #endregion
                                }

                                RowIndex++;
                            }

                            //统计 赋值
                            if (ArrColumnTotal.Any())
                            {
                                int NumIndex = 0;

                                #region 设置列格式

                                for (int i = 0; i < MaxCells; i++)
                                {
                                    CellStyle.Font.Color = System.Drawing.Color.Black;
                                    CellStyle.Number = 0;//设置默认单元格格式为 常规
                                    sheet.Cells[RowIndex, i].SetStyle(CellStyle);
                                }

                                #endregion

                                foreach (var itemTotal in ArrColumnTotal.OrderBy(x => x.ColumnIndex))
                                {
                                    //var style = sheet.Cells[RowIndex, itemTotal.ColumnIndex].GetStyle();
                                    var style = CellStyle;
                                    if (NumIndex == 0)
                                    {
                                        if (itemTotal.ColumnIndex > 0)
                                        {
                                            sheet.Cells[RowIndex, itemTotal.ColumnIndex - 1].Value = "合计：";
                                        }
                                        else
                                        {
                                            int ColIndex = itemTotal.ColumnIndex;
                                            while (ArrColumnTotal.Any(x => x.ColumnIndex == ColIndex))
                                            {
                                                ColIndex++;
                                            }

                                            sheet.Cells[RowIndex, ColIndex].Value = "合计：";
                                        }
                                    }
                                    object objVal = itemTotal.ColumnTotal;
                                    //int LevePointNum = 2;//保留小数位数
                                    #region 截取小数位
                                    if (itemTotal.ColumnName == "序号")
                                    {
                                        objVal = "";
                                    }
                                    else if (itemTotal.ColumnName.IndexOf("单价") >= 0)
                                    {
                                        //var indexpoint = objVal.IndexOf(".") + 1;
                                        //var objlen = objVal.Length;
                                        //if (indexpoint > 0)
                                        //{
                                        //    if (objlen > (indexpoint + 4))
                                        //        objVal = objVal.Substring(0, indexpoint + 4);
                                        //    else
                                        //        objVal = objVal.Substring(0, objlen);
                                        //}
                                        decimal dcparse = 0;
                                        if (decimal.TryParse(objVal.ToString(), out dcparse))
                                        {
                                            objVal = (object)dcparse;
                                        }
                                        style.Custom = "0.0000";
                                        sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                                    }
                                    else if (itemTotal.ColumnName.IndexOf("毛重") >= 0 || itemTotal.ColumnName.IndexOf("净重") >= 0 || itemTotal.ColumnName.IndexOf("金额") >= 0 || itemTotal.ColumnName.IndexOf("总价值") >= 0)
                                    {
                                        decimal dcparse = 0;
                                        if (decimal.TryParse(objVal.ToString(), out dcparse))
                                        {
                                            objVal = (object)dcparse;
                                        }
                                        style.Custom = "0.00";
                                    }
                                    else if (itemTotal.ColumnName.IndexOf("件数") >= 0)
                                    {
                                        style.Custom = "0";
                                        int dcparse = 0;
                                        if (int.TryParse(objVal.ToString(), out dcparse))
                                        {
                                            objVal = (object)dcparse;
                                        }
                                    }
                                    #endregion
                                    if (objVal.ToString() == "0.0000" || objVal.ToString() == "0.000" || objVal.ToString() == "0.00" || objVal.ToString() == "0.0" || objVal.ToString() == "0")
                                        objVal = "";

                                    style.Font.Color = System.Drawing.Color.Blue;
                                    sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                                    sheet.Cells[RowIndex, itemTotal.ColumnIndex].Value = objVal;
                                    NumIndex++;
                                }
                            }
                        }
                        #endregion

                        index++;
                    }

                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/DownLoad/"));
                    if (!dir.Exists)
                        dir.Create();

                    FilePath = (dir.FullName + FileStartName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff_") + new Random().Next(1, 10).ToString("00") + ".xls");
                    workbook.Save(FilePath, Aspose.Cells.SaveFormat.Excel97To2003);
                    return workbook.SaveToStream().ToArray();
                }
                catch (Exception)
                {
                    return OutPutExcelByListModel(ObjT);
                }
            }
            else
            {
                return OutPutExcelByListModel(ObjT);
            }
        }

        /// <summary>
        /// 根据 数组导出 指定模板Excel 数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="ObjT">List<obj></param>
        /// <param name="ExcelModelPath">模板Excel位置</param>
        /// <param name="HeadLineRowIndex">模板表头行开始号</param>
        /// <param name="HeadLineCellsIndex">模板表头列开始号</param>
        /// <returns></returns>
        public static byte[] OutPutExcelByArr<T>(IEnumerable<T> ArrT, string ExcelModelPath, int HeadLineRowIndex, int HeadLineCellsIndex, out string FilePath, string FileStartName)
             where T : class//List<obj>
        {
            FilePath = "";

            //单元格 最大宽度
            double MaxCellWidth = 80;

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //如果 是 int 或者 decimal 或者 double 的 合计
            List<AirOut.Web.Models.TotalColumn> ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();

            ////为Aspose添加License注册
            //Aspose.Cells.License license = new Aspose.Cells.License();
            //SetAsposeLicense(license);

            FileInfo ExcelFile = new FileInfo(HttpContext.Current.Server.MapPath(ExcelModelPath));
            if (ExcelFile.Exists)
            {
                try
                {
                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ExcelFile.FullName);

                    #region 单元格样式3
                    Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                    if (FileStartName == "拼货表")
                    {
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 靠左 
                        CellStyle.Font.Name = "Calibri";//文字字体 
                        CellStyle.Font.Size = 9;//文字大小 
                        //CellStyle.IsTextWrapped = true;//自动换行
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;
                        CellStyle.VerticalAlignment = TextAlignmentType.Center;
                        CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    }
                    else if (FileStartName == "操作清单")
                    {
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 靠左 
                        CellStyle.Font.Name = "Calibri";//文字字体 
                        CellStyle.Font.Size = 10;//文字大小 
                        CellStyle.IsTextWrapped = true;//自动换行
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;
                        CellStyle.VerticalAlignment = TextAlignmentType.Center;
                        CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    }
                    else if (FileStartName == "业务接单信息")
                    {
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 靠左 
                        CellStyle.Font.Name = "Calibri";//文字字体 
                        CellStyle.Font.Size = 10;//文字大小 
                        CellStyle.IsTextWrapped = true;//自动换行
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;
                        CellStyle.VerticalAlignment = TextAlignmentType.Center;
                        CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    }
                    else if (FileStartName == "仓单")
                    {
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 靠左 
                        CellStyle.Font.Name = "宋体";//文字字体 
                        CellStyle.Font.Size = 8;//文字大小 
                        CellStyle.IsTextWrapped = true;//自动换行
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;
                        CellStyle.VerticalAlignment = TextAlignmentType.Center;
                        CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;                        
                    }
                    else if (FileStartName == "应收费用明细")
                    {
                        CellStyle.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                        CellStyle.Font.Name = "宋体";//文字字体 
                        CellStyle.Font.Size = 10;//文字大小 
                        CellStyle.IsTextWrapped = true;//自动换行
                        CellStyle.VerticalAlignment = TextAlignmentType.Center;
                        CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                        
                    }
                    else if (FileStartName == "订舱单")
                    {
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 靠左 
                        CellStyle.Font.Name = "宋体";//文字字体 
                        CellStyle.Font.Size = 10;//文字大小                       
                        CellStyle.IsTextWrapped = true;//自动换行
                        CellStyle.VerticalAlignment = TextAlignmentType.Center;
                        CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                    }
                    else if (FileStartName == "应付对账单" || FileStartName == "应付对账单(含税)" || FileStartName == "应收对账单(含税)" || FileStartName == "应收对账单(含税)")
                    {
                        CellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 靠左 
                        CellStyle.Font.Name = "宋体";//文字字体 
                        CellStyle.Font.Size = 10;//文字大小 
                        CellStyle.IsTextWrapped = true;//自动换行
                        CellStyle.ShrinkToFit = true;
                        CellStyle.Font.Color = System.Drawing.Color.Black;
                        CellStyle.VerticalAlignment = TextAlignmentType.Center;
                        CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                    }
                    else {                        
                        //CellStyle.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                        CellStyle.Font.Name = "宋体";//文字字体 
                        CellStyle.Font.Size = 10;//文字大小 
                        CellStyle.IsTextWrapped = true;//自动换行
                        CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    }


                    #endregion

                    Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                    int RowIndex = HeadLineRowIndex;
                    int CellsIndex = HeadLineCellsIndex;

                    Type t = typeof(T);
                    System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                    RowIndex++;
                    foreach (var item in ArrT)
                    {
                        CellsIndex = HeadLineCellsIndex;
                        var IsToTal = false;//是否是 汇总行

                        #region 判断 是否是 汇总 行 如果是汇总 不需要 再总汇总中 累加

                        //foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                        //{
                        //    var retVal = fiitem.GetValue((object)item, null);
                        //    if (retVal != null)
                        //    {
                        //        if (retVal.ToString().IndexOf("汇总") >= 0 || retVal.ToString().IndexOf("合计") >= 0)
                        //        {
                        //            IsToTal = true;
                        //            break;
                        //        }
                        //    }
                        //}

                        #endregion

                        foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                        {
                            var itemIEType = fiitem.PropertyType;
                            //是否是泛型
                            if (itemIEType.IsGenericType)
                            {
                                var ArrGnricType = itemIEType.GetGenericArguments();
                                if (ArrGnricType.Any())
                                    itemIEType = ArrGnricType.FirstOrDefault();
                            }
                            //判断是否是 基元类型 string struct 为特殊的 基元类型
                            if (!(itemIEType.IsPrimitive || itemIEType.IsValueType || itemIEType == typeof(string) || itemIEType == typeof(decimal) || itemIEType == typeof(DateTime)) && itemIEType.Name.ToLower().IndexOf("struct") < 0)
                                continue;

                            DisplayAttribute disAttr = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                            System.Reflection.PropertyInfo fi = fiitem;
                            string itemColumn = fiitem.Name;

                            #region 赋值列值

                            if (fi != null)
                            {
                                object obj = fi.GetValue((object)item, null);
                                Type _type = fi.PropertyType;

                                sheet.Cells[RowIndex, CellsIndex].SetStyle(CellStyle);
                                var style = CellStyle;
                                style.Font.Color = System.Drawing.Color.Black;

                                var retVal = obj;
                                string DataType = fi.PropertyType.Name;

                                //判断 是都是泛型 int? List<int> 等
                                if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = fi.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;
                                }

                                var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");

                                #region 根据 数据类型 显示值和设置列宽

                                if (retVal != null)
                                {
                                    switch (DataType.ToLower())
                                    {
                                        case "int":
                                            int Dftint = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp = wheredictTotal.FirstOrDefault();
                                                int _Total = 0;
                                                if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                {
                                                    Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                Temp.ColumnName = fi.Name;
                                                Temp.ColumnIndex = CellsIndex;
                                                Temp.ColumnTotal = Dftint.ToString();
                                                ArrColumnTotal.Add(Temp);
                                            }

                                            #endregion
                                            break;
                                        case "int32":
                                            int Dftint32 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint32))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp32 = wheredictTotal.FirstOrDefault();
                                                int _Total32 = 0;
                                                if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                {
                                                    Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                Temp32.ColumnName = fi.Name;
                                                Temp32.ColumnIndex = CellsIndex;
                                                Temp32.ColumnTotal = Dftint32.ToString();
                                                ArrColumnTotal.Add(Temp32);
                                            }

                                            #endregion
                                            break;
                                        case "int64":
                                            int Dftint64 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint64))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp64 = wheredictTotal.FirstOrDefault();
                                                int _Total64 = 0;
                                                if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                {
                                                    Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                Temp64.ColumnName = fi.Name;
                                                Temp64.ColumnIndex = CellsIndex;
                                                Temp64.ColumnTotal = Dftint64.ToString();
                                                ArrColumnTotal.Add(Temp64);
                                            }

                                            #endregion
                                            break;
                                        case "string":
                                            style.Number = 49;
                                            if (retVal == "")
                                            {
                                                style.Number = 0;
                                            }
                                            break;
                                        case "datetime":
                                            int TDatetime = 0;
                                            if (int.TryParse(retVal.ToString(), out TDatetime))
                                            {
                                                style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else
                                            {
                                                DateTime DftDateTime = new DateTime();
                                                if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                {
                                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                            }
                                            break;
                                        case "bool":
                                            bool Dftbool = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftbool))
                                            {
                                                style.Number = 9;
                                                if (Dftbool)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "boolean":
                                            bool Dftboolean = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                            {
                                                style.Number = 9;
                                                if (Dftboolean)
                                                {
                                                    if (FileStartName == "应付对账单" || FileStartName == "应付对账单(含税)" || FileStartName == "应收对账单(含税)" || FileStartName == "应收对账单(含税)")
                                                    {
                                                        style.Font.Color = System.Drawing.Color.Black;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        obj = "是";
                                                    }
                                                    else {
                                                        style.Font.Color = System.Drawing.Color.Green;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        obj = "是";
                                                    }                                                 
                                                }
                                                else
                                                {
                                                    if (FileStartName == "应付对账单" || FileStartName == "应付对账单(含税)" || FileStartName == "应收对账单(含税)" || FileStartName == "应收对账单(含税)")
                                                    {
                                                        style.Font.Color = System.Drawing.Color.Black;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        obj = "否";
                                                    }
                                                    else
                                                    {
                                                        style.Font.Color = System.Drawing.Color.Red;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        obj = "否";
                                                    }
                                                }
                                            }
                                            break;
                                        case "decimal":
                                            decimal Dftdecimal = 0;
                                            if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                            {
                                                style.Number = 0;
                                            }
                                            if (FileStartName  == "ADDOC" ||FileStartName  == "应付对账单" || FileStartName == "应付对账单(含税)" || FileStartName == "应收对账单(含税)" || FileStartName == "应收对账单(含税)") 
                                            {
                                                style.Number = 2;
                                            }
                                            //if (fi.Name.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (fi.Name.IndexOf("毛重") >= 0 || fi.Name.IndexOf("净重") >= 0 || fi.Name.IndexOf("金额") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                decimal _Totaldecimal = 0;
                                                if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                {
                                                    Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                Tempdecima.ColumnName = fi.Name;
                                                Tempdecima.ColumnIndex = CellsIndex;
                                                Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                ArrColumnTotal.Add(Tempdecima);
                                            }

                                            #endregion
                                            break;
                                        case "double":
                                            double Dftdouble = 0;
                                            if (double.TryParse(retVal.ToString(), out Dftdouble))
                                            {
                                                style.Number = 0;
                                            }
                                            //if (fi.Name.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (fi.Name.IndexOf("毛重") >= 0 || fi.Name.IndexOf("净重") >= 0 || fi.Name.IndexOf("金额") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";

                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdouble = wheredictTotal.FirstOrDefault();
                                                double _Totaldouble = 0;
                                                if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                {
                                                    Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                Tempdouble.ColumnName = fi.Name;
                                                Tempdouble.ColumnIndex = CellsIndex;
                                                Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                ArrColumnTotal.Add(Tempdouble);
                                            }

                                            #endregion
                                            break;
                                        case "float":
                                            float Dftfloat = 0;
                                            if (float.TryParse(retVal.ToString(), out Dftfloat))
                                            {
                                                style.Number = 0;
                                            }
                                            //if (fi.Name.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (fi.Name.IndexOf("毛重") >= 0 || fi.Name.IndexOf("净重") >= 0 || fi.Name.IndexOf("金额") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempfloat = wheredictTotal.FirstOrDefault();
                                                float _Totalfloat = 0;
                                                if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                {
                                                    Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                Tempfloat.ColumnName = fi.Name;
                                                Tempfloat.ColumnIndex = CellsIndex;
                                                Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                ArrColumnTotal.Add(Tempfloat);
                                            }

                                            #endregion
                                            break;
                                        default:
                                            style.Number = 0;
                                            break;                                        

                                    }
                                    if (FileStartName == "操作清单" || FileStartName == "拼货表" || FileStartName == "业务接单信息" || FileStartName == "仓单") 
                                    {
                                        style.Number = 0;
                                        //FileStartName = "";
                                    }
                                }

                                #endregion

                                #region  自动列宽

                                if (obj != null)
                                {
                                    System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                                    double StrWidth = 0;
                                    StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                                    if (StrWidth > MaxCellWidth)
                                    {
                                        StrWidth = MaxCellWidth;
                                    }
                                    if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                        sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                }

                                #endregion

                                //设置样式
                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);

                                if (fi.Name == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineCellsIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = obj;
                            }
                            else
                            {
                                if (fi.Name == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineCellsIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = "";
                            }

                            #endregion


                            CellsIndex++;
                        }

                        RowIndex++;
                    }
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/DownLoad/"));
                    if (!dir.Exists)
                        dir.Create();

                    //统计 赋值
                    //if (ArrColumnTotal.Any())
                    //{
                    //    int NumIndex = 0;
                    //    foreach (var itemTotal in ArrColumnTotal.OrderBy(x => x.ColumnIndex))
                    //    {
                    //        if (NumIndex == 0)
                    //        {
                    //            if (itemTotal.ColumnIndex > 0)
                    //            {
                    //                sheet.Cells[RowIndex, itemTotal.ColumnIndex - 1].Value = "合计：";
                    //            }
                    //            else
                    //            {
                    //                int ColIndex = itemTotal.ColumnIndex;
                    //                while (ArrColumnTotal.Any(x => x.ColumnIndex == ColIndex))
                    //                {
                    //                    ColIndex++;
                    //                }

                    //                sheet.Cells[RowIndex, ColIndex].Value = "合计：";
                    //            }
                    //        }
                    //        object objVal = itemTotal.ColumnTotal;

                    //        if (objVal.ToString() == "0.0000" || objVal.ToString() == "0.000" || objVal.ToString() == "0.00" || objVal.ToString() == "0.0" || objVal.ToString() == "0")
                    //            objVal = "";

                    //        Style style = workbook.Styles[workbook.Styles.Add()];//新增样式 
                    //        style.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                    //        style.Font.Name = "宋体";//文字字体 
                    //        style.Font.Size = 10;//文字大小 
                    //        style.IsTextWrapped = true;//自动换行
                    //        style.Number = 0;
                    //        style.Font.Color = System.Drawing.Color.Red;
                    //        sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                    //        sheet.Cells[RowIndex, itemTotal.ColumnIndex].Value = objVal;
                    //        NumIndex++;
                    //    }
                    //}

                    FilePath = (dir.FullName + FileStartName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff_") + new Random().Next(1, 10).ToString("00") + ".xls");
                    workbook.Save(FilePath, Aspose.Cells.SaveFormat.Excel97To2003);
                    return workbook.SaveToStream().ToArray();
                }
                catch (Exception)
                {
                    return OutPutExcelByListModel(ArrT);
                }
            }
            else
            {
                return OutPutExcelByListModel(ArrT);
            }
        }


        /// <summary>
        /// 根据 数组导出 指定模板Excel 数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="ObjT">List<obj></param>
        /// <param name="ExcelModelPath">模板Excel位置</param>
        /// <param name="HeadLineRowIndex">模板表头行开始号</param>
        /// <param name="HeadLineCellsIndex">模板表头列开始号</param>
        /// <returns></returns>
        public static byte[] OutPutExcelByArr<T>(IEnumerable<T> ArrT, string ExcelModelPath, int listnum, string loginname, out string FilePath, string FileStartName)
             where T : class//List<obj>
        {
            FilePath = "";

            //单元格 最大宽度
            double MaxCellWidth = 80;

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //如果 是 int 或者 decimal 或者 double 的 合计
            List<AirOut.Web.Models.TotalColumn> ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();

            ////为Aspose添加License注册
            //Aspose.Cells.License license = new Aspose.Cells.License();
            //SetAsposeLicense(license);

            FileInfo ExcelFile = new FileInfo(HttpContext.Current.Server.MapPath(ExcelModelPath));
            if (ExcelFile.Exists)
            {
                try
                {
                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ExcelFile.FullName);

                    #region 单元格样式3
                    Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                    
                    CellStyle.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                    CellStyle.Font.Name = "宋体";//文字字体 
                    CellStyle.Font.Size = 14;//文字大小 
                    CellStyle.Font.IsBold = true;//粗体
                    CellStyle.IsTextWrapped = true;//自动换行
                    
                    #endregion

                    int SheetIndex = 0;
                    int RowIndex = 0;
                    int CellsIndex = 0;

                    Type t = typeof(T);
                    System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    var _objArrSheet = ArrT as System.Collections.IList;
                    if (_objArrSheet.Count > 1)
                        CopySheet(workbook, _objArrSheet.Count - 1);
                    RowIndex++;
                    foreach (var item in ArrT)
                    {
                        //CopySheet(workbook, SheetIndex);
                        Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[SheetIndex];
                        SheetIndex = SheetIndex + 1;
                        //CellsIndex = HeadLineCellsIndex;
                        var IsToTal = false;//是否是 汇总行

                        foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                        {
                            var itemIEType = fiitem.PropertyType;
                            //是否是泛型
                            if (itemIEType.IsGenericType)
                            {
                                var ArrGnricType = itemIEType.GetGenericArguments();
                                if (ArrGnricType.Any())
                                    itemIEType = ArrGnricType.FirstOrDefault();
                            }
                            //判断是否是 基元类型 string struct 为特殊的 基元类型
                            //if (!(itemIEType.IsPrimitive || itemIEType.IsValueType || itemIEType == typeof(string) || itemIEType == typeof(decimal) || itemIEType == typeof(DateTime)) && itemIEType.Name.ToLower().IndexOf("struct") < 0)
                            //{
                            //    continue;
                            //}

                            DisplayAttribute disAttr = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                            System.Reflection.PropertyInfo fi = fiitem;

                            #region 通过栏位名称 定位数据的位置

                            string itemColumn = fiitem.Name;
                            if (itemColumn == "Entry_Id")
                            {
                                RowIndex = 1;
                                CellsIndex = 2;
                            }
                            else if (itemColumn == "Warehouse_Id")
                            {
                                RowIndex = 2;
                                CellsIndex = 2;
                            }
                            else if (itemColumn == "Year")
                            {
                                RowIndex = 2;
                                CellsIndex = 6;
                            }
                            else if (itemColumn == "Month")
                            {
                                RowIndex = 2;
                                CellsIndex = 7;
                            }
                            else if (itemColumn == "Day")
                            {
                                RowIndex = 2;
                                CellsIndex = 8;
                            }
                            else if (itemColumn == "Out_Time")
                            {
                                RowIndex = 2;
                                CellsIndex = 10;
                            }
                            else if (itemColumn == "Consign_Code_CK_Name")
                            {
                                RowIndex = 4;
                                CellsIndex = 2;
                            }
                            else if (itemColumn == "End_Port")
                            {
                                RowIndex = 4;
                                CellsIndex = 10;
                            }
                            else if (itemColumn == "Pieces_CK")
                            {
                                RowIndex = 12;
                                CellsIndex = 1;
                            }
                            else if (itemColumn == "Weight_CK")
                            {
                                RowIndex = 12;
                                CellsIndex = 5;
                            }
                            else if (itemColumn == "Volume_CK")
                            {
                                RowIndex = 12;
                                CellsIndex = 9;
                            }
                            else if (itemColumn == "Mask")
                            {
                                RowIndex = 14;
                                CellsIndex = 1;
                            }
                            else if (itemColumn == "Packing_Name")
                            {
                                RowIndex = 14;
                                CellsIndex = 7;
                            }
                            else if (itemColumn == "Remark")
                            {
                                RowIndex = 17;
                                CellsIndex = 7;
                            }
                            else if (itemColumn == "ArrPdfJCDList")
                            {
                                RowIndex = 8;
                                CellsIndex = 1;
                            }

                            #endregion

                            #region 赋值列值

                            if (fi != null)
                            {
                                object obj = fi.GetValue((object)item, null);
                                Type _type = fi.PropertyType;
                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(CellStyle);
                                var style = CellStyle;
                                //style.Font.Color = System.Drawing.Color.Black;
                                var retVal = obj;
                                string DataType = fi.PropertyType.Name;
                                //判断 是都是泛型 int? List<int> 等
                                if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = fi.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;
                                }
                                var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");
                                #region 根据 数据类型 显示值和设置列宽

                                if (retVal != null)
                                {
                                    switch (DataType.ToLower())
                                    {
                                        case "int":
                                            int Dftint = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp = wheredictTotal.FirstOrDefault();
                                                int _Total = 0;
                                                if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                {
                                                    Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                Temp.ColumnName = fi.Name;
                                                Temp.ColumnIndex = CellsIndex;
                                                Temp.ColumnTotal = Dftint.ToString();
                                                ArrColumnTotal.Add(Temp);
                                            }

                                            #endregion
                                            break;
                                        case "int32":
                                            int Dftint32 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint32))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp32 = wheredictTotal.FirstOrDefault();
                                                int _Total32 = 0;
                                                if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                {
                                                    Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                Temp32.ColumnName = fi.Name;
                                                Temp32.ColumnIndex = CellsIndex;
                                                Temp32.ColumnTotal = Dftint32.ToString();
                                                ArrColumnTotal.Add(Temp32);
                                            }

                                            #endregion
                                            break;
                                        case "int64":
                                            int Dftint64 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint64))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp64 = wheredictTotal.FirstOrDefault();
                                                int _Total64 = 0;
                                                if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                {
                                                    Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                Temp64.ColumnName = fi.Name;
                                                Temp64.ColumnIndex = CellsIndex;
                                                Temp64.ColumnTotal = Dftint64.ToString();
                                                ArrColumnTotal.Add(Temp64);
                                            }

                                            #endregion
                                            break;
                                        case "string":
                                            style.Number = 9;
                                            break;
                                        case "datetime":
                                            int TDatetime = 0;
                                            if (int.TryParse(retVal.ToString(), out TDatetime))
                                            {
                                                style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else
                                            {
                                                DateTime DftDateTime = new DateTime();
                                                if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                {
                                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                            }
                                            break;
                                        case "bool":
                                            bool Dftbool = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftbool))
                                            {
                                                style.Number = 9;
                                                if (Dftbool)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "boolean":
                                            bool Dftboolean = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                            {
                                                style.Number = 9;
                                                if (Dftboolean)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "decimal":
                                            decimal Dftdecimal = 0;
                                            if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                            {
                                                style.Number = 0;
                                            }
                                            //if (fi.Name.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (fi.Name.IndexOf("毛重") >= 0 || fi.Name.IndexOf("净重") >= 0 || fi.Name.IndexOf("金额") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                decimal _Totaldecimal = 0;
                                                if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                {
                                                    Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                Tempdecima.ColumnName = fi.Name;
                                                Tempdecima.ColumnIndex = CellsIndex;
                                                Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                ArrColumnTotal.Add(Tempdecima);
                                            }

                                            #endregion
                                            break;
                                        case "double":
                                            double Dftdouble = 0;
                                            if (double.TryParse(retVal.ToString(), out Dftdouble))
                                            {
                                                style.Number = 0;
                                            }
                                            //if (fi.Name.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (fi.Name.IndexOf("毛重") >= 0 || fi.Name.IndexOf("净重") >= 0 || fi.Name.IndexOf("金额") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";

                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdouble = wheredictTotal.FirstOrDefault();
                                                double _Totaldouble = 0;
                                                if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                {
                                                    Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                Tempdouble.ColumnName = fi.Name;
                                                Tempdouble.ColumnIndex = CellsIndex;
                                                Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                ArrColumnTotal.Add(Tempdouble);
                                            }

                                            #endregion
                                            break;
                                        case "float":
                                            float Dftfloat = 0;
                                            if (float.TryParse(retVal.ToString(), out Dftfloat))
                                            {
                                                style.Number = 0;
                                            }
                                            //if (fi.Name.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (fi.Name.IndexOf("毛重") >= 0 || fi.Name.IndexOf("净重") >= 0 || fi.Name.IndexOf("金额") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempfloat = wheredictTotal.FirstOrDefault();
                                                float _Totalfloat = 0;
                                                if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                {
                                                    Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                Tempfloat.ColumnName = fi.Name;
                                                Tempfloat.ColumnIndex = CellsIndex;
                                                Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                ArrColumnTotal.Add(Tempfloat);
                                            }

                                            #endregion
                                            break;
                                        default:
                                            style.Number = 9;
                                            break;

                                    }
                                    if (FileStartName == "操作清单" || FileStartName == "拼货表" || FileStartName == "业务接单信息" || FileStartName == "仓单")
                                    {
                                        style.Number = 0;
                                        //FileStartName = "";
                                    }
                                }

                                #endregion

                                //设置样式
                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                #region  单元格赋值

                                if (itemColumn == "ArrPdfJCDList")
                                {
                                    #region 仓库接单明细数据
                                    int numrow = 0;
                                    dynamic temp = obj;
                                    foreach (var val in temp)
                                    {
                                        if (numrow < listnum)
                                        {
                                            switch (numrow)
                                            {
                                                case 0:
                                                    MethodWarehouse_receipts_details<T>(sheet, item, temp, numrow, 8, 1);
                                                    break;
                                                case 1:
                                                    MethodWarehouse_receipts_details<T>(sheet, item, temp, numrow, 8, 6);
                                                    break;
                                                case 2:
                                                    MethodWarehouse_receipts_details<T>(sheet, item, temp, numrow, 9, 1);
                                                    break;
                                                case 3:
                                                    MethodWarehouse_receipts_details<T>(sheet, item, temp, numrow, 9, 6);
                                                    break;
                                                case 4:
                                                    MethodWarehouse_receipts_details<T>(sheet, item, temp, numrow, 10, 1);
                                                    break;
                                                case 5:
                                                    MethodWarehouse_receipts_details<T>(sheet, item, temp, numrow, 10, 6);
                                                    break;
                                                case 6:
                                                    MethodWarehouse_receipts_details<T>(sheet, item, temp, numrow, 11, 1);
                                                    break;
                                                case 7:
                                                    MethodWarehouse_receipts_details<T>(sheet, item, temp, numrow, 11, 6);
                                                    break;
                                            }
                                        }
                                        numrow = numrow + 1;
                                    }
                                    #endregion

                                }else if (itemColumn == "Entry_Id")
                                {
                                        if (obj != null)
                                        {
                                            dynamic dy = obj;
                                            sheet.Name = dy;
                                            sheet.Cells[RowIndex, CellsIndex].Value = obj;
                                        }
                                }
                                else
                                {
                                    if (itemColumn == "Year")
                                    {

                                        if (obj != null)
                                        {
                                            dynamic dy = obj;
                                            sheet.Cells[RowIndex, CellsIndex].Value = Int32.Parse(dy);
                                        }
                                    }
                                    else
                                    {
                                        sheet.Cells[RowIndex, CellsIndex].Value = obj;
                                    }
                                }

                                #endregion

                            }


                            #endregion


                            CellsIndex++;
                        }
                        sheet.Cells[24, 7].Value = loginname;

                        RowIndex++;
                    }
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/DownLoad/"));
                    if (!dir.Exists)
                        dir.Create();

                    FilePath = (dir.FullName + FileStartName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff_") + new Random().Next(1, 10).ToString("00") + ".xls");
                    workbook.Save(FilePath, Aspose.Cells.SaveFormat.Excel97To2003);
                    return workbook.SaveToStream().ToArray();
                }
                catch (Exception)
                {
                    return OutPutExcelByListModel(ArrT);
                }
            }
            else
            {
                return OutPutExcelByListModel(ArrT);
            }
        }
        /// <summary>
        /// 仓库接单明细数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet"></param>
        /// <param name="item"></param>
        /// <param name="obj"></param>
        private static void MethodWarehouse_receipts_details<T>(Aspose.Cells.Worksheet sheet, T item, dynamic obj, int numrow, int nowrow, int nowColunm) where T : class
        {
            var valobj = Common.GetProtityValue(obj[numrow], "CM_Length");
            if (valobj != null && valobj != "")
            {
                sheet.Cells[nowrow, nowColunm].Value = Convert.ToInt32(Convert.ToDouble(valobj));
            }
            valobj = Common.GetProtityValue(obj[numrow], "CM_Width");
            if (valobj != null && valobj != "")
            {
                sheet.Cells[nowrow, nowColunm + 1].Value = Convert.ToInt32(Convert.ToDouble(valobj));
            }
            valobj = Common.GetProtityValue(obj[numrow], "CM_Height");
            if (valobj != null && valobj != "")
            {
                sheet.Cells[nowrow, nowColunm + 2].Value = Convert.ToInt32(Convert.ToDouble(valobj));
            }
            valobj = Common.GetProtityValue(obj[numrow], "CM_Piece");
            if (valobj != null && valobj != "")
            {
                sheet.Cells[nowrow, nowColunm + 4].Value = Convert.ToInt32(Convert.ToDouble(valobj));
            }
        }
        //导出两个sheet页
        public static byte[] OutPutExcelByArr_Acc<T>(IEnumerable<T> ArrT, string ExcelModelPath, int HeadLineRowIndex, int HeadLineCellsIndex, out string FilePath, string FileStartName)
             where T : class//List<obj>
        {
            FilePath = "";

            //单元格 最大宽度
            double MaxCellWidth = 80;

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //如果 是 int 或者 decimal 或者 double 的 合计
            List<AirOut.Web.Models.TotalColumn> ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();

            ////为Aspose添加License注册
            //Aspose.Cells.License license = new Aspose.Cells.License();
            //SetAsposeLicense(license);

            FileInfo ExcelFile = new FileInfo(HttpContext.Current.Server.MapPath(ExcelModelPath));
            if (ExcelFile.Exists)
            {
                try
                {
                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ExcelFile.FullName);

                    #region 单元格样式

                    Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                    CellStyle.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                    CellStyle.Font.Name = "宋体";//文字字体 
                    CellStyle.Font.Size = 10;//文字大小 
                    CellStyle.IsTextWrapped = true;//自动换行
                    CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                    #endregion

                    Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                    int RowIndex = HeadLineRowIndex;
                    int CellsIndex = HeadLineCellsIndex;

                    Type t = typeof(T);
                    System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                    RowIndex++;

                    foreach (var item in ArrT)
                    {
                        CellsIndex = HeadLineCellsIndex;
                        var IsToTal = false;//是否是 汇总行

                        foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                        {
                            var itemIEType = fiitem.PropertyType;
                            //是否是泛型
                            if (itemIEType.IsGenericType)
                            {
                                var ArrGnricType = itemIEType.GetGenericArguments();
                                if (ArrGnricType.Any())
                                    itemIEType = ArrGnricType.FirstOrDefault();
                            }
                            //判断是否是 基元类型 string struct 为特殊的 基元类型
                            if (!(itemIEType.IsPrimitive || itemIEType.IsValueType || itemIEType == typeof(string) || itemIEType == typeof(decimal) || itemIEType == typeof(DateTime)) && itemIEType.Name.ToLower().IndexOf("struct") < 0)
                                continue;

                            DisplayAttribute disAttr = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                            System.Reflection.PropertyInfo fi = fiitem;
                            string itemColumn = fiitem.Name;

                            #region 赋值列值

                            if (fi != null)
                            {
                                object obj = fi.GetValue((object)item, null);
                                Type _type = fi.PropertyType;

                                sheet.Cells[RowIndex, CellsIndex].SetStyle(CellStyle);
                                var style = CellStyle;
                                style.Font.Color = System.Drawing.Color.Black;

                                var retVal = obj;
                                string DataType = fi.PropertyType.Name;

                                //判断 是都是泛型 int? List<int> 等
                                if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = fi.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;
                                }

                                var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");

                                #region 根据 数据类型 显示值和设置列宽

                                if (retVal != null)
                                {
                                    switch (DataType.ToLower())
                                    {
                                        case "int":
                                            int Dftint = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp = wheredictTotal.FirstOrDefault();
                                                int _Total = 0;
                                                if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                {
                                                    Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                Temp.ColumnName = fi.Name;
                                                Temp.ColumnIndex = CellsIndex;
                                                Temp.ColumnTotal = Dftint.ToString();
                                                ArrColumnTotal.Add(Temp);
                                            }

                                            #endregion
                                            break;
                                        case "int32":
                                            int Dftint32 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint32))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp32 = wheredictTotal.FirstOrDefault();
                                                int _Total32 = 0;
                                                if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                {
                                                    Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                Temp32.ColumnName = fi.Name;
                                                Temp32.ColumnIndex = CellsIndex;
                                                Temp32.ColumnTotal = Dftint32.ToString();
                                                ArrColumnTotal.Add(Temp32);
                                            }

                                            #endregion
                                            break;
                                        case "int64":
                                            int Dftint64 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint64))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp64 = wheredictTotal.FirstOrDefault();
                                                int _Total64 = 0;
                                                if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                {
                                                    Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                Temp64.ColumnName = fi.Name;
                                                Temp64.ColumnIndex = CellsIndex;
                                                Temp64.ColumnTotal = Dftint64.ToString();
                                                ArrColumnTotal.Add(Temp64);
                                            }

                                            #endregion
                                            break;
                                        case "string":
                                            style.Number = 9;
                                            break;
                                        case "datetime":
                                            int TDatetime = 0;
                                            if (int.TryParse(retVal.ToString(), out TDatetime))
                                            {
                                                style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else
                                            {
                                                DateTime DftDateTime = new DateTime();
                                                if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                {
                                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                            }
                                            break;
                                        case "bool":
                                            bool Dftbool = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftbool))
                                            {
                                                style.Number = 9;
                                                if (Dftbool)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "boolean":
                                            bool Dftboolean = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                            {
                                                style.Number = 9;
                                                if (Dftboolean)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "decimal":
                                            decimal Dftdecimal = 0;
                                            if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                decimal _Totaldecimal = 0;
                                                if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                {
                                                    Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                Tempdecima.ColumnName = fi.Name;
                                                Tempdecima.ColumnIndex = CellsIndex;
                                                Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                ArrColumnTotal.Add(Tempdecima);
                                            }

                                            #endregion
                                            break;
                                        case "double":
                                            double Dftdouble = 0;
                                            if (double.TryParse(retVal.ToString(), out Dftdouble))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";

                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdouble = wheredictTotal.FirstOrDefault();
                                                double _Totaldouble = 0;
                                                if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                {
                                                    Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                Tempdouble.ColumnName = fi.Name;
                                                Tempdouble.ColumnIndex = CellsIndex;
                                                Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                ArrColumnTotal.Add(Tempdouble);
                                            }

                                            #endregion
                                            break;
                                        case "float":
                                            float Dftfloat = 0;
                                            if (float.TryParse(retVal.ToString(), out Dftfloat))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempfloat = wheredictTotal.FirstOrDefault();
                                                float _Totalfloat = 0;
                                                if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                {
                                                    Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                Tempfloat.ColumnName = fi.Name;
                                                Tempfloat.ColumnIndex = CellsIndex;
                                                Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                ArrColumnTotal.Add(Tempfloat);
                                            }

                                            #endregion
                                            break;
                                        default:
                                            style.Number = 9;
                                            break;
                                    }
                                }

                                #endregion

                                #region  自动列宽

                                if (obj != null)
                                {
                                    System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                                    double StrWidth = 0;
                                    StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                                    if (StrWidth > MaxCellWidth)
                                    {
                                        StrWidth = MaxCellWidth;
                                    }
                                    if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                        sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                }

                                #endregion

                                //设置样式
                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);

                                if (fi.Name == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineCellsIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = obj;
                            }
                            else
                            {
                                if (fi.Name == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineCellsIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = "";
                            }

                            #endregion


                            CellsIndex++;
                        }

                        RowIndex++;
                    }
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/DownLoad/"));
                    if (!dir.Exists)
                        dir.Create();

                    //统计 赋值
                    if (ArrColumnTotal.Any())
                    {
                        int NumIndex = 0;
                        foreach (var itemTotal in ArrColumnTotal.OrderBy(x => x.ColumnIndex))
                        {
                            if (NumIndex == 0)
                            {
                                if (itemTotal.ColumnIndex > 0)
                                {
                                    sheet.Cells[RowIndex, itemTotal.ColumnIndex - 1].Value = "合计：";
                                }
                                else
                                {
                                    int ColIndex = itemTotal.ColumnIndex;
                                    while (ArrColumnTotal.Any(x => x.ColumnIndex == ColIndex))
                                    {
                                        ColIndex++;
                                    }

                                    sheet.Cells[RowIndex, ColIndex].Value = "合计：";
                                }
                            }
                            object objVal = itemTotal.ColumnTotal;

                            if (objVal.ToString() == "0.0000" || objVal.ToString() == "0.000" || objVal.ToString() == "0.00" || objVal.ToString() == "0.0" || objVal.ToString() == "0")
                                objVal = "";

                            Style style = workbook.Styles[workbook.Styles.Add()];//新增样式 
                            style.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                            style.Font.Name = "宋体";//文字字体 
                            style.Font.Size = 10;//文字大小 
                            style.IsTextWrapped = true;//自动换行
                            style.Number = 0;
                            style.Font.Color = System.Drawing.Color.Red;
                            sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                            sheet.Cells[RowIndex, itemTotal.ColumnIndex].Value = objVal;
                            NumIndex++;
                        }
                    }

                    FilePath = (dir.FullName + FileStartName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff_") + new Random().Next(1, 10).ToString("00") + ".xls");
                    workbook.Save(FilePath, Aspose.Cells.SaveFormat.Excel97To2003);
                    return workbook.SaveToStream().ToArray();
                }
                catch (Exception)
                {
                    return OutPutExcelByListModel(ArrT);
                }
            }
            else
            {
                return OutPutExcelByListModel(ArrT);
            }
        }

        //应收/应付对账单 两个sheet页
        public static byte[] OutPutExcelByArrBMS<T, P>(IEnumerable<T> ArrT, IEnumerable<P> ArrT1, string ExcelModelPath, int HeadLineRowIndex, int HeadLineCellsIndex, out string FilePath, string FileStartName)
             where T : class//List<obj>
        {
            FilePath = "";

            //单元格 最大宽度
            double MaxCellWidth = 80;

            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //如果 是 int 或者 decimal 或者 double 的 合计
            List<AirOut.Web.Models.TotalColumn> ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();

            ////为Aspose添加License注册
            //Aspose.Cells.License license = new Aspose.Cells.License();
            //SetAsposeLicense(license);

            FileInfo ExcelFile = new FileInfo(HttpContext.Current.Server.MapPath(ExcelModelPath));
            if (ExcelFile.Exists)
            {
                try
                {
                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(ExcelFile.FullName);

                    #region 单元格样式

                    Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                    CellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 居中 左右 
                    CellStyle.VerticalAlignment = TextAlignmentType.Center;//文字 居中 上下
                    CellStyle.Font.Name = "宋体";//文字字体 
                    CellStyle.Font.Size = 10;//文字大小 
                    CellStyle.IsTextWrapped = true;//自动换行
                    CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                    #endregion

                    Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
                    Aspose.Cells.Worksheet sheet1 = (Aspose.Cells.Worksheet)workbook.Worksheets[1];
                    int RowIndex = HeadLineRowIndex;
                    int RowIndex1 = HeadLineRowIndex;
                    int CellsIndex = HeadLineCellsIndex;

                    Type t = typeof(T);
                    System.Reflection.PropertyInfo[] PropertyInfos = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    Type p = typeof(P);
                    System.Reflection.PropertyInfo[] PropertyInfos1 = p.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);



                    RowIndex++;
                    RowIndex1++;

                    //for (var i = 0; i < ArrT.Count() + 1; i++)
                    //{
                    //    sheet.AutoFitRow(i);
                    //}

                    //for (var i = 0; i < ArrT1.Count() + 1; i++)
                    //{
                    //    sheet1.AutoFitRow(i);
                    //}

                    foreach (var item in ArrT)
                    {
                        CellsIndex = HeadLineCellsIndex;
                        var IsToTal = false;//是否是 汇总行

                        foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos)
                        {
                            var itemIEType = fiitem.PropertyType;
                            //是否是泛型
                            if (itemIEType.IsGenericType)
                            {
                                var ArrGnricType = itemIEType.GetGenericArguments();
                                if (ArrGnricType.Any())
                                    itemIEType = ArrGnricType.FirstOrDefault();
                            }
                            //判断是否是 基元类型 string struct 为特殊的 基元类型
                            if (!(itemIEType.IsPrimitive || itemIEType.IsValueType || itemIEType == typeof(string) || itemIEType == typeof(decimal) || itemIEType == typeof(DateTime)) && itemIEType.Name.ToLower().IndexOf("struct") < 0)
                                continue;

                            DisplayAttribute disAttr = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                            System.Reflection.PropertyInfo fi = fiitem;
                            string itemColumn = fiitem.Name;

                            #region 赋值列值

                            if (fi != null)
                            {
                                object obj = fi.GetValue((object)item, null);
                                Type _type = fi.PropertyType;

                                sheet.Cells[RowIndex, CellsIndex].SetStyle(CellStyle);
                                var style = CellStyle;
                                style.Font.Color = System.Drawing.Color.Black;

                                var retVal = obj;
                                string DataType = fi.PropertyType.Name;

                                //判断 是都是泛型 int? List<int> 等
                                if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = fi.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;
                                }

                                var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");

                                #region 根据 数据类型 显示值和设置列宽

                                if (retVal != null)
                                {
                                    switch (DataType.ToLower())
                                    {
                                        case "int":
                                            int Dftint = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp = wheredictTotal.FirstOrDefault();
                                                int _Total = 0;
                                                if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                {
                                                    Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                Temp.ColumnName = fi.Name;
                                                Temp.ColumnIndex = CellsIndex;
                                                Temp.ColumnTotal = Dftint.ToString();
                                                ArrColumnTotal.Add(Temp);
                                            }

                                            #endregion
                                            break;
                                        case "int32":
                                            int Dftint32 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint32))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp32 = wheredictTotal.FirstOrDefault();
                                                int _Total32 = 0;
                                                if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                {
                                                    Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                Temp32.ColumnName = fi.Name;
                                                Temp32.ColumnIndex = CellsIndex;
                                                Temp32.ColumnTotal = Dftint32.ToString();
                                                ArrColumnTotal.Add(Temp32);
                                            }

                                            #endregion
                                            break;
                                        case "int64":
                                            int Dftint64 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint64))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp64 = wheredictTotal.FirstOrDefault();
                                                int _Total64 = 0;
                                                if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                {
                                                    Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                Temp64.ColumnName = fi.Name;
                                                Temp64.ColumnIndex = CellsIndex;
                                                Temp64.ColumnTotal = Dftint64.ToString();
                                                ArrColumnTotal.Add(Temp64);
                                            }

                                            #endregion
                                            break;
                                        case "string":
                                            style.Number = 9;
                                            break;
                                        case "datetime":
                                            int TDatetime = 0;
                                            if (int.TryParse(retVal.ToString(), out TDatetime))
                                            {
                                                style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else
                                            {
                                                DateTime DftDateTime = new DateTime();
                                                if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                {
                                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                            }
                                            break;
                                        case "bool":
                                            bool Dftbool = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftbool))
                                            {
                                                style.Number = 9;
                                                if (Dftbool)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "boolean":
                                            bool Dftboolean = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                            {
                                                style.Number = 9;
                                                if (Dftboolean)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "decimal":
                                            decimal Dftdecimal = 0;
                                            if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                decimal _Totaldecimal = 0;
                                                if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                {
                                                    Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                Tempdecima.ColumnName = fi.Name;
                                                Tempdecima.ColumnIndex = CellsIndex;
                                                Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                ArrColumnTotal.Add(Tempdecima);
                                            }

                                            #endregion
                                            break;
                                        case "double":
                                            double Dftdouble = 0;
                                            if (double.TryParse(retVal.ToString(), out Dftdouble))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";

                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdouble = wheredictTotal.FirstOrDefault();
                                                double _Totaldouble = 0;
                                                if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                {
                                                    Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                Tempdouble.ColumnName = fi.Name;
                                                Tempdouble.ColumnIndex = CellsIndex;
                                                Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                ArrColumnTotal.Add(Tempdouble);
                                            }

                                            #endregion
                                            break;
                                        case "float":
                                            float Dftfloat = 0;
                                            if (float.TryParse(retVal.ToString(), out Dftfloat))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempfloat = wheredictTotal.FirstOrDefault();
                                                float _Totalfloat = 0;
                                                if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                {
                                                    Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                Tempfloat.ColumnName = fi.Name;
                                                Tempfloat.ColumnIndex = CellsIndex;
                                                Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                ArrColumnTotal.Add(Tempfloat);
                                            }

                                            #endregion
                                            break;
                                        default:
                                            style.Number = 9;
                                            break;
                                    }
                                }

                                #endregion

                                #region  自动列宽

                                if (obj != null)
                                {
                                    System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                                    double StrWidth = 0;
                                    StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                                    if (StrWidth > MaxCellWidth)
                                    {
                                        StrWidth = MaxCellWidth;
                                    }
                                    if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                        sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                }

                                #endregion

                                //设置样式
                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);

                                if (fi.Name == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineCellsIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = obj;
                            }
                            else
                            {
                                if (fi.Name == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineCellsIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = "";
                            }

                            #endregion


                            CellsIndex++;
                        }

                        RowIndex++;
                    }
                    foreach (var item in ArrT1)
                    {
                        CellsIndex = HeadLineCellsIndex;
                        var IsToTal = false;//是否是 汇总行

                        foreach (System.Reflection.PropertyInfo fiitem in PropertyInfos1)
                        {
                            var itemIEType = fiitem.PropertyType;
                            //是否是泛型
                            if (itemIEType.IsGenericType)
                            {
                                var ArrGnricType = itemIEType.GetGenericArguments();
                                if (ArrGnricType.Any())
                                    itemIEType = ArrGnricType.FirstOrDefault();
                            }
                            //判断是否是 基元类型 string struct 为特殊的 基元类型
                            if (!(itemIEType.IsPrimitive || itemIEType.IsValueType || itemIEType == typeof(string) || itemIEType == typeof(decimal) || itemIEType == typeof(DateTime)) && itemIEType.Name.ToLower().IndexOf("struct") < 0)
                                continue;

                            DisplayAttribute disAttr = (DisplayAttribute)fiitem.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                            System.Reflection.PropertyInfo fi = fiitem;
                            string itemColumn = fiitem.Name;

                            #region 赋值列值

                            if (fi != null)
                            {
                                object obj = fi.GetValue((object)item, null);
                                Type _type = fi.PropertyType;

                                sheet1.Cells[RowIndex1, CellsIndex].SetStyle(CellStyle);
                                var style = CellStyle;
                                style.Font.Color = System.Drawing.Color.Black;

                                var retVal = obj;
                                string DataType = fi.PropertyType.Name;

                                //判断 是都是泛型 int? List<int> 等
                                if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = fi.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;
                                }

                                var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");

                                #region 根据 数据类型 显示值和设置列宽

                                if (retVal != null)
                                {
                                    switch (DataType.ToLower())
                                    {
                                        case "int":
                                            int Dftint = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp = wheredictTotal.FirstOrDefault();
                                                int _Total = 0;
                                                if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                {
                                                    Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                Temp.ColumnName = fi.Name;
                                                Temp.ColumnIndex = CellsIndex;
                                                Temp.ColumnTotal = Dftint.ToString();
                                                ArrColumnTotal.Add(Temp);
                                            }

                                            #endregion
                                            break;
                                        case "int32":
                                            int Dftint32 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint32))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp32 = wheredictTotal.FirstOrDefault();
                                                int _Total32 = 0;
                                                if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                {
                                                    Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                Temp32.ColumnName = fi.Name;
                                                Temp32.ColumnIndex = CellsIndex;
                                                Temp32.ColumnTotal = Dftint32.ToString();
                                                ArrColumnTotal.Add(Temp32);
                                            }

                                            #endregion
                                            break;
                                        case "int64":
                                            int Dftint64 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint64))
                                            {
                                                style.Number = 0;
                                            }
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp64 = wheredictTotal.FirstOrDefault();
                                                int _Total64 = 0;
                                                if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                {
                                                    Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                Temp64.ColumnName = fi.Name;
                                                Temp64.ColumnIndex = CellsIndex;
                                                Temp64.ColumnTotal = Dftint64.ToString();
                                                ArrColumnTotal.Add(Temp64);
                                            }

                                            #endregion
                                            break;
                                        case "string":
                                            style.Number = 9;
                                            break;
                                        case "datetime":
                                            int TDatetime = 0;
                                            if (int.TryParse(retVal.ToString(), out TDatetime))
                                            {
                                                style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                sheet1.Cells[RowIndex1, CellsIndex].SetStyle(style);
                                            }
                                            else
                                            {
                                                DateTime DftDateTime = new DateTime();
                                                if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                {
                                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                    sheet1.Cells[RowIndex1, CellsIndex].SetStyle(style);
                                                }
                                            }
                                            break;
                                        case "bool":
                                            bool Dftbool = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftbool))
                                            {
                                                style.Number = 9;
                                                if (Dftbool)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet1.Cells[RowIndex1, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet1.Cells[RowIndex1, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "boolean":
                                            bool Dftboolean = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                            {
                                                style.Number = 9;
                                                if (Dftboolean)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet1.Cells[RowIndex1, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet1.Cells[RowIndex1, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "decimal":
                                            decimal Dftdecimal = 0;
                                            if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                decimal _Totaldecimal = 0;
                                                if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                {
                                                    Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                Tempdecima.ColumnName = fi.Name;
                                                Tempdecima.ColumnIndex = CellsIndex;
                                                Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                ArrColumnTotal.Add(Tempdecima);
                                            }

                                            #endregion
                                            break;
                                        case "double":
                                            double Dftdouble = 0;
                                            if (double.TryParse(retVal.ToString(), out Dftdouble))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";

                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdouble = wheredictTotal.FirstOrDefault();
                                                double _Totaldouble = 0;
                                                if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                {
                                                    Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                Tempdouble.ColumnName = fi.Name;
                                                Tempdouble.ColumnIndex = CellsIndex;
                                                Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                ArrColumnTotal.Add(Tempdouble);
                                            }

                                            #endregion
                                            break;
                                        case "float":
                                            float Dftfloat = 0;
                                            if (float.TryParse(retVal.ToString(), out Dftfloat))
                                            {
                                                style.Number = 0;
                                            }
                                            if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == fi.Name);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempfloat = wheredictTotal.FirstOrDefault();
                                                float _Totalfloat = 0;
                                                if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                {
                                                    Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                Tempfloat.ColumnName = fi.Name;
                                                Tempfloat.ColumnIndex = CellsIndex;
                                                Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                ArrColumnTotal.Add(Tempfloat);
                                            }

                                            #endregion
                                            break;
                                        default:
                                            style.Number = 9;
                                            break;
                                    }
                                }

                                #endregion

                                #region  自动列宽

                                if (obj != null)
                                {
                                    System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                                    double StrWidth = 0;
                                    StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                                    if (StrWidth > MaxCellWidth)
                                    {
                                        StrWidth = MaxCellWidth;
                                    }
                                    if (sheet1.Cells.Columns[CellsIndex].Width < StrWidth)
                                        sheet1.Cells.Columns[CellsIndex].Width = StrWidth;
                                }

                                #endregion

                                //设置样式
                                sheet1.Cells[RowIndex1, CellsIndex].SetStyle(style);

                                if (fi.Name == "序号")
                                    sheet1.Cells[RowIndex1, CellsIndex].Value = RowIndex1 - HeadLineCellsIndex;
                                else
                                    sheet1.Cells[RowIndex1, CellsIndex].Value = obj;
                            }
                            else
                            {
                                if (fi.Name == "序号")
                                    sheet1.Cells[RowIndex1, CellsIndex].Value = RowIndex1 - HeadLineCellsIndex;
                                else
                                    sheet1.Cells[RowIndex1, CellsIndex].Value = "";
                            }

                            #endregion


                            CellsIndex++;
                        }

                        RowIndex1++;
                    }
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("/DownLoad/"));
                    if (!dir.Exists)
                        dir.Create();

                    //统计 赋值
                    //if (ArrColumnTotal.Any())
                    //{
                    //    int NumIndex = 0;
                    //    foreach (var itemTotal in ArrColumnTotal.OrderBy(x => x.ColumnIndex))
                    //    {
                    //        if (NumIndex == 0)
                    //        {
                    //            if (itemTotal.ColumnIndex > 0)
                    //            {
                    //                sheet.Cells[RowIndex, itemTotal.ColumnIndex - 1].Value = "合计：";
                    //            }
                    //            else
                    //            {
                    //                int ColIndex = itemTotal.ColumnIndex;
                    //                while (ArrColumnTotal.Any(x => x.ColumnIndex == ColIndex))
                    //                {
                    //                    ColIndex++;
                    //                }

                    //                sheet.Cells[RowIndex, ColIndex].Value = "合计：";
                    //            }
                    //        }
                    //        object objVal = itemTotal.ColumnTotal;

                    //        if (objVal.ToString() == "0.0000" || objVal.ToString() == "0.000" || objVal.ToString() == "0.00" || objVal.ToString() == "0.0" || objVal.ToString() == "0")
                    //            objVal = "";

                    //        Style style = workbook.Styles[workbook.Styles.Add()];//新增样式 
                    //        style.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                    //        style.Font.Name = "宋体";//文字字体 
                    //        style.Font.Size = 10;//文字大小 
                    //        style.IsTextWrapped = true;//自动换行
                    //        style.Number = 0;
                    //        style.Font.Color = System.Drawing.Color.Red;
                    //        sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                    //        sheet.Cells[RowIndex, itemTotal.ColumnIndex].Value = objVal;
                    //        NumIndex++;
                    //    }
                    //}

                    FilePath = (dir.FullName + FileStartName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff_") + new Random().Next(1, 10).ToString("00") + ".xls");
                    workbook.Save(FilePath, Aspose.Cells.SaveFormat.Excel97To2003);
                    return workbook.SaveToStream().ToArray();
                }
                catch (Exception)
                {
                    return OutPutExcelByListModel(ArrT);
                }
            }
            else
            {
                return OutPutExcelByListModel(ArrT);
            }
        }

        /// <summary>
        /// 根据 匿名数组 指定模板Excel 数据
        /// </summary>
        /// <typeparam name="T">读取名称Model类型</typeparam>
        /// <param name="ObjT">匿名数组</param>
        /// <param name="ExcelModelPath">模板Excel位置</param>
        /// <param name="HeadLineNum">模板列表头行开始号</param>
        /// <param name="HeadLineNum">模板列表头列开始号</param>
        /// <returns></returns>
        public static byte[] OutPutExcelByListModel<T>(Object ObjT) where T : class,new()
        {
            //项目顶层命名空间
            string Top_NameSpace = System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"] == null ? "AirOut" : System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"].ToString();
            //网站顶层命名空间
            string WebTop_NameSpace = System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"] == null ? "AirOut.Web" : System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"].ToString();
            //项目类命名空间
            string Models_NameSpace = System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"] == null ? "Models" : System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"].ToString();

            string ModelsNameSpace = WebTop_NameSpace + "." + Models_NameSpace;//"AirOut.Web.Models";
            string ModelsTopNameSpace = Top_NameSpace + ".";// "AirOut.";
            //单元格 最大宽度
            double MaxCellWidth = 80;
            //Excel起始行和列
            int HeadLineRowIndex = 0, HeadLineCellsIndex = 0;
            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //如果 是 int 或者 decimal 或者 double 的 合计
            List<AirOut.Web.Models.TotalColumn> ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();

            Aspose.Cells.License license = new Aspose.Cells.License();
            SetAsposeLicense(license);
            try
            {
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();

                #region 单元格样式3

                Style HeadCellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                HeadCellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 靠左 
                HeadCellStyle.Font.Name = "宋体";//文字字体 
                HeadCellStyle.Font.Size = 12;//文字大小 
                HeadCellStyle.Font.IsBold = true;//文字加粗
                HeadCellStyle.IsTextWrapped = true;//自动换行
                HeadCellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                HeadCellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                HeadCellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                HeadCellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                CellStyle.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                CellStyle.Font.Name = "宋体";//文字字体 
                CellStyle.Font.Size = 10;//文字大小 
                CellStyle.IsTextWrapped = true;//自动换行
                CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                #endregion

                var _objArrSheet = ObjT as System.Collections.IList;

                List<Aspose.Cells.Worksheet> Sheet_s = new List<Aspose.Cells.Worksheet>();
                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {
                    Sheet_s.Add(workbook.Worksheets[i]);
                }
                //匿名类的Type
                Type _T = null;
                if (_objArrSheet != null)
                {
                    if (_objArrSheet.Count > 0)
                        _T = _objArrSheet[0].GetType();
                }
                T objT = new T();
                Type GetType = objT.GetType();
                PropertyInfo[] getPi = GetType.GetProperties();
                PropertyInfo[] basePi = _T == null ? new PropertyInfo[] { } : _T.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                //最大列序号
                int MaxCells = 0;
                //保存列名
                Dictionary<string, int> ColumnNames = new Dictionary<string, int>();

                #region 添加表头信息

                var FirstSheet = Sheet_s[0];
                int _CellIndex = 0;
                foreach (PropertyInfo pi in basePi)
                {
                    string ColumnName = pi.Name;
                    var WhereGetPi = getPi.Where(x => x.Name == pi.Name);
                    if (WhereGetPi.Any())
                    {
                        ColumnName = AirOut.Web.Extensions.Common.GetMetaDataDisplayName(GetType, pi.Name);
                    }
                    else
                    {
                        WhereGetPi = getPi.Where(x => x.Name == pi.Name.Replace("NAME", ""));
                        if (WhereGetPi.Any())
                        {
                            ColumnName = AirOut.Web.Extensions.Common.GetMetaDataDisplayName(GetType, pi.Name.Replace("NAME", "")) + "名称";
                        }
                    }
                    //设置单元格 名称
                    FirstSheet.Cells[0, _CellIndex].Value = ColumnName;
                    FirstSheet.Cells[0, _CellIndex].SetStyle(HeadCellStyle);
                    //记录列名
                    ColumnNames.Add(ColumnName, _CellIndex);

                    #region  自动列宽

                    if (!string.IsNullOrEmpty(ColumnName))
                    {
                        System.Drawing.Font font = new System.Drawing.Font(HeadCellStyle.Font.Name, HeadCellStyle.Font.Size);
                        double StrWidth = 0;
                        StrWidth = g.MeasureString(ColumnName, font).Width / 7.384;
                        if (StrWidth > MaxCellWidth)
                        {
                            StrWidth = MaxCellWidth;
                        }
                        if (FirstSheet.Cells.Columns[_CellIndex].Width < StrWidth)
                            FirstSheet.Cells.Columns[_CellIndex].Width = StrWidth;
                        else if (0 == HeadLineRowIndex)
                        {
                            FirstSheet.Cells.Columns[_CellIndex].Width = StrWidth;
                        }
                    }

                    #endregion

                    _CellIndex++;
                    if (_CellIndex > MaxCells)
                        MaxCells = _CellIndex;
                }

                #endregion

                //int index = 0;
                //int StartNum = 0;
                //int EndNum = 0;
                ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();
                Aspose.Cells.Worksheet sheet = Sheet_s[0];

                #region  每个Sheet 添加值

                int RowIndex = HeadLineRowIndex;
                int CellsIndex = HeadLineCellsIndex;
                foreach (var itemObj in _objArrSheet)
                {
                    object objList = itemObj;
                    if (objList != null)
                    {
                        System.Reflection.PropertyInfo[] PropertyInfos = basePi;
                        RowIndex++;

                        var IsToTal = false;//是否是 汇总行

                        #region 设置列格式

                        for (int i = 0; i < MaxCells; i++)
                        {
                            CellStyle.Font.Color = System.Drawing.Color.Black;
                            CellStyle.Number = 0;//设置默认单元格格式为 常规
                            sheet.Cells[RowIndex, i].SetStyle(CellStyle);
                        }

                        #endregion

                        foreach (var itemColumn in ColumnNames)
                        {
                            CellsIndex = itemColumn.Value;
                            DisplayAttribute disAttr = null;
                            System.Reflection.PropertyInfo fi = null;

                            #region 是否 包含列

                            //bool HasColumn = true;
                            fi = basePi.Skip(CellsIndex).Take(1).FirstOrDefault();

                            #endregion

                            #region 添加列

                            if (fi != null)
                            {
                                object obj = fi.GetValue((object)objList, null);
                                string str = "";
                                Type _type = null;
                                if (obj != null)
                                {
                                    str = obj.ToString();
                                    _type = obj.GetType();
                                }

                                #region 如果是 数组

                                if (str.IndexOf(ModelsTopNameSpace) >= 0 || str.IndexOf("System.") >= 0)
                                {
                                    try
                                    {
                                        if (_type != null)
                                        {
                                            if (str.IndexOf(ModelsTopNameSpace) >= 0 && _type.Name.IndexOf("HashSet") < 0)
                                            {
                                                string FieldName = "Name";
                                                System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                                var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                                if (_PropertyInfos.Any())
                                                {
                                                    obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null);
                                                }
                                            }
                                            if (str.IndexOf("System.Collections.") >= 0)
                                            {
                                                string FieldName = "Count";
                                                System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                                var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                                if (_PropertyInfos.Any())
                                                {
                                                    string ModelName = obj.ToString();
                                                    try
                                                    {
                                                        if (disAttr != null)
                                                            ModelName = disAttr.Name;
                                                        else
                                                        {
                                                            string Split = ModelsNameSpace + ".";
                                                            int Sindex = ModelName.IndexOf(Split);
                                                            if (Sindex > 0)
                                                            {
                                                                ModelName = ModelName.Substring(Sindex + Split.Length);
                                                                if (ModelName.IndexOf(']') >= 0)
                                                                {
                                                                    ModelName = ModelName.Substring(0, ModelName.IndexOf(']'));
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {

                                                    }
                                                    obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null).ToString() + "-" + ModelName;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                                #endregion

                                var style = sheet.Cells[RowIndex, CellsIndex].GetStyle();

                                var retVal = obj;

                                string DataType = fi.PropertyType.Name;
                                if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = fi.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;
                                }

                                //int decimal double float 统计 零时保存数组
                                var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");

                                #region 根据 数据类型 显示值和设置列宽

                                if (retVal != null)
                                {
                                    switch (DataType.ToLower())
                                    {
                                        case "int":
                                            int Dftint = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint))
                                            {
                                                style.Number = 1;
                                                if (Dftint == 0)
                                                    obj = "";
                                                //style.Font.Color = System.Drawing.Color.Blue;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (IsToTal && Dftint == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp = wheredictTotal.FirstOrDefault();
                                                int _Total = 0;
                                                if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                {
                                                    Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                Temp.ColumnName = itemColumn.Key;
                                                Temp.ColumnIndex = itemColumn.Value;
                                                Temp.ColumnTotal = Dftint.ToString();
                                                ArrColumnTotal.Add(Temp);
                                            }

                                            #endregion
                                            break;
                                        case "int32":
                                            int Dftint32 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint32))
                                            {
                                                style.Number = 1;
                                                if (Dftint32 == 0)
                                                    obj = "";
                                                //style.Font.Color = System.Drawing.Color.Blue;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (IsToTal && Dftint32 == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp32 = wheredictTotal.FirstOrDefault();
                                                int _Total32 = 0;
                                                if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                {
                                                    Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                Temp32.ColumnName = itemColumn.Key;
                                                Temp32.ColumnIndex = itemColumn.Value;
                                                Temp32.ColumnTotal = Dftint32.ToString();
                                                ArrColumnTotal.Add(Temp32);
                                            }

                                            #endregion
                                            break;
                                        case "int64":
                                            int Dftint64 = 0;
                                            if (int.TryParse(retVal.ToString(), out Dftint64))
                                            {
                                                style.Number = 1;
                                                if (Dftint64 == 0)
                                                    obj = "";
                                                //style.Font.Color = System.Drawing.Color.Blue;
                                                //sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            if (IsToTal && Dftint64 == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Temp64 = wheredictTotal.FirstOrDefault();
                                                int _Total64 = 0;
                                                if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                {
                                                    Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                Temp64.ColumnName = itemColumn.Key;
                                                Temp64.ColumnIndex = itemColumn.Value;
                                                Temp64.ColumnTotal = Dftint64.ToString();
                                                ArrColumnTotal.Add(Temp64);
                                            }

                                            #endregion
                                            break;
                                        case "string":
                                            style.Number = 9;
                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            break;
                                        case "datetime":
                                            int TDatetime = 0;
                                            if (int.TryParse(retVal.ToString(), out TDatetime))
                                            {
                                                style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            }
                                            else
                                            {
                                                DateTime DftDateTime = new DateTime();
                                                if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                {
                                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                            }
                                            break;
                                        case "bool":
                                            bool Dftbool = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftbool))
                                            {
                                                style.Number = 9;
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                if (Dftbool)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "boolean":
                                            bool Dftboolean = false;
                                            if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                            {
                                                style.Number = 9;
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                if (Dftboolean)
                                                {
                                                    style.Font.Color = System.Drawing.Color.Green;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "是";
                                                }
                                                else
                                                {
                                                    style.Font.Color = System.Drawing.Color.Red;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    obj = "否";
                                                }
                                            }
                                            break;
                                        case "decimal":
                                            decimal Dftdecimal = 0;
                                            if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                            {
                                                if (Dftdecimal == 0)
                                                    obj = "";
                                            }
                                            //if (itemColumn.Key.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0 || itemColumn.Key.IndexOf("总价值") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                decimal _Totaldecimal = 0;
                                                if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                {
                                                    Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                Tempdecima.ColumnName = itemColumn.Key;
                                                Tempdecima.ColumnIndex = itemColumn.Value;
                                                Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                ArrColumnTotal.Add(Tempdecima);
                                            }

                                            #endregion
                                            break;
                                        case "double":
                                            double Dftdouble = 0;
                                            if (double.TryParse(retVal.ToString(), out Dftdouble))
                                            {
                                                style.Number = 1;
                                                if (Dftdouble == 0)
                                                    obj = "";
                                            }
                                            //if (itemColumn.Key.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0 || itemColumn.Key.IndexOf("总价值") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempdouble = wheredictTotal.FirstOrDefault();
                                                double _Totaldouble = 0;
                                                if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                {
                                                    Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                Tempdouble.ColumnName = itemColumn.Key;
                                                Tempdouble.ColumnIndex = itemColumn.Value;
                                                Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                ArrColumnTotal.Add(Tempdouble);
                                            }

                                            #endregion
                                            break;
                                        case "float":
                                            float Dftfloat = 0;
                                            if (float.TryParse(retVal.ToString(), out Dftfloat))
                                            {
                                                style.Number = 1;
                                                if (Dftfloat == 0)
                                                    obj = "";
                                            }
                                            //if (itemColumn.Key.IndexOf("单价") >= 0)
                                            //{
                                            //    style.Custom = "0.0000";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            //else if (itemColumn.Key.IndexOf("毛重") >= 0 || itemColumn.Key.IndexOf("净重") >= 0 || itemColumn.Key.IndexOf("金额") >= 0 || itemColumn.Key.IndexOf("总价值") >= 0)
                                            //{
                                            //    style.Custom = "0.00";
                                            //    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            //}
                                            if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                obj = "";
                                            #region int decimal double 类型 汇总

                                            if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                break;
                                            wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                            if (wheredictTotal.Any())
                                            {
                                                var Tempfloat = wheredictTotal.FirstOrDefault();
                                                float _Totalfloat = 0;
                                                if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                {
                                                    Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                }
                                            }
                                            else
                                            {
                                                AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                Tempfloat.ColumnName = itemColumn.Key;
                                                Tempfloat.ColumnIndex = itemColumn.Value;
                                                Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                ArrColumnTotal.Add(Tempfloat);
                                            }

                                            #endregion
                                            break;
                                        default:
                                            style.Number = 9;
                                            sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                            break;
                                    }
                                }

                                #endregion

                                #region  自动列宽

                                if (obj != null)
                                {
                                    System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                                    double StrWidth = 0;
                                    StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                                    if (StrWidth > MaxCellWidth)
                                    {
                                        StrWidth = MaxCellWidth;
                                    }
                                    if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                        sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                }

                                #endregion

                                #region 设置背景色
                                //else
                                //{
                                //    var _style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                                //    //_style.BackgroundColor = System.Drawing.Color.LightSkyBlue;
                                //    _style.ForegroundColor = System.Drawing.Color.FromArgb(230, 211, 211, 211);
                                //    _style.Pattern = Aspose.Cells.BackgroundType.Solid;
                                //    sheet.Cells[RowIndex, CellsIndex].SetStyle(_style);
                                //}
                                #endregion

                                if (itemColumn.Key == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineRowIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = obj;
                                //CellsIndex++;
                            }
                            else
                            {
                                if (itemColumn.Key == "序号")
                                    sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineRowIndex;
                                else
                                    sheet.Cells[RowIndex, CellsIndex].Value = "";
                                //CellsIndex++;
                            }
                            #endregion
                        }

                        RowIndex++;
                    }
                }

                #endregion

                //统计 赋值
                if (ArrColumnTotal.Any())
                {
                    int NumIndex = 0;

                    #region 设置列格式

                    for (int i = 0; i < MaxCells; i++)
                    {
                        CellStyle.Font.Color = System.Drawing.Color.Black;
                        CellStyle.Number = 0;//设置默认单元格格式为 常规
                        sheet.Cells[RowIndex, i].SetStyle(CellStyle);
                    }

                    #endregion

                    foreach (var itemTotal in ArrColumnTotal.OrderBy(x => x.ColumnIndex))
                    {
                        var style = CellStyle;
                        if (NumIndex == 0)
                        {
                            if (itemTotal.ColumnIndex > 0)
                            {
                                sheet.Cells[RowIndex, itemTotal.ColumnIndex - 1].Value = "合计：";
                            }
                            else
                            {
                                int ColIndex = itemTotal.ColumnIndex;
                                while (ArrColumnTotal.Any(x => x.ColumnIndex == ColIndex))
                                {
                                    ColIndex++;
                                }

                                sheet.Cells[RowIndex, ColIndex].Value = "合计：";
                            }
                        }
                        object objVal = itemTotal.ColumnTotal;

                        style.Font.Color = System.Drawing.Color.Blue;
                        sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                        sheet.Cells[RowIndex, itemTotal.ColumnIndex].Value = objVal;
                        NumIndex++;
                    }
                }

                string FileDownLoadPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "\\FileDownLoad\\";
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath(FileDownLoadPath));
                if (!dir.Exists)
                    dir.Create();
                workbook.Save((dir.FullName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls"), Aspose.Cells.SaveFormat.Excel97To2003);
                return workbook.SaveToStream().ToArray();
            }
            catch (Exception)
            {
                return OutPutExcelByListModel(ObjT);
            }
        }

        /// <summary>
        /// 根据 匿名数组 指定模板Excel 数据
        /// </summary>
        /// <typeparam name="T">读取名称Model类型</typeparam>
        /// <param name="ObjT">List<匿名数组>(List<List<a'>>)</param>
        /// <param name="ExcelModelPath">模板Excel位置</param>
        /// <param name="HeadLineNum">模板列表头行开始号</param>
        /// <param name="HeadLineNum">模板列表头列开始号</param>
        /// <returns></returns>
        public static string OutPutExcelByListObjModel<T>(List<Object> ObjT) where T : class,new()
        {
            //项目顶层命名空间
            string Top_NameSpace = System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"] == null ? "AirOut" : System.Configuration.ConfigurationManager.AppSettings["Top_NameSpace"].ToString();
            //网站顶层命名空间
            string WebTop_NameSpace = System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"] == null ? "AirOut.Web" : System.Configuration.ConfigurationManager.AppSettings["WebTop_NameSpace"].ToString();
            //项目类命名空间
            string Models_NameSpace = System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"] == null ? "Models" : System.Configuration.ConfigurationManager.AppSettings["ModelsNameSpace"].ToString();

            string ModelsNameSpace = WebTop_NameSpace + "." + Models_NameSpace;//"AirOut.Web.Models";
            string ModelsTopNameSpace = Top_NameSpace + ".";// "AirOut.";
            //单元格 最大宽度
            double MaxCellWidth = 80;
            //Excel起始行和列
            int HeadLineRowIndex = 0, HeadLineCellsIndex = 0;
            //创建Graphics 测量 文字宽度
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //如果 是 int 或者 decimal 或者 double 的 合计
            List<AirOut.Web.Models.TotalColumn> ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();

            Aspose.Cells.License license = new Aspose.Cells.License();
            SetAsposeLicense(license);
            try
            {
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();

                #region 单元格样式3

                Style HeadCellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                HeadCellStyle.HorizontalAlignment = TextAlignmentType.Center;//文字 靠左 
                HeadCellStyle.Font.Name = "宋体";//文字字体 
                HeadCellStyle.Font.Size = 12;//文字大小 
                HeadCellStyle.Font.IsBold = true;//文字加粗
                HeadCellStyle.IsTextWrapped = true;//自动换行
                HeadCellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                HeadCellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                HeadCellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                HeadCellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                Style CellStyle = workbook.Styles[workbook.Styles.Add()];//新增样式 
                CellStyle.HorizontalAlignment = TextAlignmentType.Left;//文字 靠左 
                CellStyle.Font.Name = "宋体";//文字字体 
                CellStyle.Font.Size = 10;//文字大小 
                CellStyle.IsTextWrapped = true;//自动换行
                CellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                CellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                CellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                CellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                #endregion

                var _objArrSheet = ObjT as System.Collections.IList;
                //保存Sheet
                List<Aspose.Cells.Worksheet> Sheet_s = new List<Aspose.Cells.Worksheet>();
                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {
                    Sheet_s.Add(workbook.Worksheets[i]);
                }
                T objT = new T();
                Type GetType = objT.GetType();
                PropertyInfo[] getPi = GetType.GetProperties();
                int index = 0;
                //int StartNum = 0;
                //int EndNum = 0;

                //最大列序号
                int MaxCells = 0;
                //保存列名
                Dictionary<string, int> ColumnNames = new Dictionary<string, int>();
                PropertyInfo[] basePi = new PropertyInfo[] { };

                foreach (var objList in _objArrSheet)
                {
                    ArrColumnTotal = new List<AirOut.Web.Models.TotalColumn>();
                    Aspose.Cells.Worksheet sheet = null;
                    //获取数据集
                    object obj_List = null;
                    var item_Obj = objList;

                    #region 获取List 数据

                    //判断是否派生自IEnumerable
                    if (objList.GetType().GetInterface("IEnumerable", false) != null &&
                       (objList.GetType().Name.ToLower().IndexOf("string") < 0 ||
                       (objList.GetType().Name.ToLower().IndexOf("string") >= 0 &&
                       (objList.GetType().Name.ToLower().IndexOf("[]") > 0 || objList.GetType().Name.ToLower().IndexOf("<") > 0))))
                    {
                        obj_List = objList;
                    }
                    else
                    {
                        System.Reflection.PropertyInfo[] itemPropertyInfos = item_Obj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        foreach (System.Reflection.PropertyInfo fiitem in itemPropertyInfos)
                        {
                            object obj = fiitem.GetValue((object)item_Obj, null);
                            string str = "";
                            Type _type = null;
                            if (obj != null)
                            {
                                str = obj.ToString();
                                _type = obj.GetType();
                            }
                            if (str.IndexOf(Top_NameSpace + ".") >= 0 || str.IndexOf("System.") >= 0)
                            {
                                if (_type != null)
                                {
                                    if (str.IndexOf("System.Collections.") >= 0)
                                    {
                                        obj_List = obj;
                                    }
                                    else if (_type.GetInterface("IEnumerable", false) != null &&
                                      (_type.Name.ToLower().IndexOf("string") < 0 ||
                                      (_type.Name.ToLower().IndexOf("string") >= 0 &&
                                      (_type.Name.ToLower().IndexOf("[]") > 0 || _type.Name.ToLower().IndexOf("<") > 0))))
                                    {
                                        obj_List = obj;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    //转换为IList
                    var ArrObj = obj_List as System.Collections.IList;

                    #region 添加表头信息

                    if (index == 0)
                    {
                        //匿名类的Type
                        Type _T = null;
                        if (ArrObj != null)
                        {
                            if (ArrObj.Count > 0)
                                _T = ArrObj[0].GetType();
                        }
                        basePi = _T == null ? new PropertyInfo[] { } : _T.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                        Aspose.Cells.Worksheet FirstSheet = workbook.Worksheets[0];
                        int _CellIndex = 0;
                        foreach (PropertyInfo pi in basePi)
                        {
                            string ColumnName = pi.Name;
                            var WhereGetPi = getPi.Where(x => x.Name == pi.Name);
                            if (WhereGetPi.Any())
                            {
                                ColumnName = AirOut.Web.Extensions.Common.GetMetaDataDisplayName(GetType, pi.Name);
                            }
                            else
                            {
                                WhereGetPi = getPi.Where(x => x.Name == pi.Name.Replace("NAME", ""));
                                if (WhereGetPi.Any())
                                {
                                    ColumnName = AirOut.Web.Extensions.Common.GetMetaDataDisplayName(GetType, pi.Name.Replace("NAME", "")) + "名称";
                                }
                            }
                            //设置单元格 名称
                            FirstSheet.Cells[0, _CellIndex].Value = ColumnName;
                            FirstSheet.Cells[0, _CellIndex].SetStyle(HeadCellStyle);
                            //记录列名
                            ColumnNames.Add(ColumnName, _CellIndex);

                            #region  自动列宽

                            if (!string.IsNullOrEmpty(ColumnName))
                            {
                                System.Drawing.Font font = new System.Drawing.Font(HeadCellStyle.Font.Name, HeadCellStyle.Font.Size);
                                double StrWidth = 0;
                                StrWidth = g.MeasureString(ColumnName, font).Width / 7.384;
                                if (StrWidth > MaxCellWidth)
                                {
                                    StrWidth = MaxCellWidth;
                                }
                                if (FirstSheet.Cells.Columns[_CellIndex].Width < StrWidth)
                                    FirstSheet.Cells.Columns[_CellIndex].Width = StrWidth;
                                else if (0 == HeadLineRowIndex)
                                {
                                    FirstSheet.Cells.Columns[_CellIndex].Width = StrWidth;
                                }
                            }

                            #endregion

                            _CellIndex++;
                            if (_CellIndex > MaxCells)
                                MaxCells = _CellIndex;
                        }
                        //复制 Excel Sheet
                        CopySheet(workbook, _objArrSheet.Count - 1);
                    }

                    #endregion

                    #region  每个Sheet 添加值

                    sheet = workbook.Worksheets[index];
                    int RowIndex = HeadLineRowIndex;
                    int CellsIndex = HeadLineCellsIndex;
                    RowIndex++;
                    foreach (var itemObj in ArrObj)
                    {
                        if (itemObj != null)
                        {
                            System.Reflection.PropertyInfo[] PropertyInfos = basePi;

                            var IsToTal = false;//是否是 汇总行

                            #region 设置列格式

                            for (int i = 0; i < MaxCells; i++)
                            {
                                CellStyle.Font.Color = System.Drawing.Color.Black;
                                CellStyle.Number = 0;//设置默认单元格格式为 常规
                                sheet.Cells[RowIndex, i].SetStyle(CellStyle);
                            }

                            #endregion

                            foreach (var itemColumn in ColumnNames)
                            {
                                CellsIndex = itemColumn.Value;
                                DisplayAttribute disAttr = null;
                                System.Reflection.PropertyInfo fi = null;

                                #region 是否 包含列

                                //bool HasColumn = true;
                                fi = basePi.Skip(CellsIndex).Take(1).FirstOrDefault();

                                #endregion

                                #region 添加列

                                if (fi != null)
                                {
                                    object obj = fi.GetValue((object)itemObj, null);
                                    string str = "";
                                    Type _type = null;
                                    if (obj != null)
                                    {
                                        str = obj.ToString();
                                        _type = obj.GetType();
                                    }

                                    #region 如果是 数组

                                    if (str.IndexOf(ModelsTopNameSpace) >= 0 || str.IndexOf("System.") >= 0)
                                    {
                                        try
                                        {
                                            if (_type != null)
                                            {
                                                if (str.IndexOf(ModelsTopNameSpace) >= 0 && _type.Name.IndexOf("HashSet") < 0)
                                                {
                                                    string FieldName = "Name";
                                                    System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                                    var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                                    if (_PropertyInfos.Any())
                                                    {
                                                        obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null);
                                                    }
                                                }
                                                if (str.IndexOf("System.Collections.") >= 0)
                                                {
                                                    string FieldName = "Count";
                                                    System.Reflection.PropertyInfo[] _PropertyInfos = _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                                    var WhereProperty = _PropertyInfos.Where(x => x.Name == FieldName);
                                                    if (_PropertyInfos.Any())
                                                    {
                                                        string ModelName = obj.ToString();
                                                        try
                                                        {
                                                            if (disAttr != null)
                                                                ModelName = disAttr.Name;
                                                            else
                                                            {
                                                                string Split = ModelsNameSpace + ".";
                                                                int Sindex = ModelName.IndexOf(Split);
                                                                if (Sindex > 0)
                                                                {
                                                                    ModelName = ModelName.Substring(Sindex + Split.Length);
                                                                    if (ModelName.IndexOf(']') >= 0)
                                                                    {
                                                                        ModelName = ModelName.Substring(0, ModelName.IndexOf(']'));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        obj = WhereProperty.FirstOrDefault().GetValue((object)obj, null).ToString() + "-" + ModelName;
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    #endregion

                                    var style = sheet.Cells[RowIndex, CellsIndex].GetStyle();

                                    var retVal = obj;

                                    string DataType = fi.PropertyType.Name;
                                    if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        var Arguments = fi.PropertyType.GetGenericArguments();
                                        DataType = Arguments[0].Name;
                                    }

                                    //int decimal double float 统计 零时保存数组
                                    var wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName != "");

                                    #region 根据 数据类型 显示值和设置列宽

                                    if (retVal != null)
                                    {
                                        switch (DataType.ToLower())
                                        {
                                            case "int":
                                                int Dftint = 0;
                                                if (int.TryParse(retVal.ToString(), out Dftint))
                                                {
                                                    style.Number = 1;
                                                    if (Dftint == 0)
                                                        obj = "";
                                                    style.Font.Color = System.Drawing.Color.Blue;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                                if (IsToTal && Dftint == 0)//如果是 汇总行 不累加到 总汇总中
                                                    obj = "";
                                                #region int decimal double 类型 汇总

                                                if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                    break;
                                                wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                if (wheredictTotal.Any())
                                                {
                                                    var Temp = wheredictTotal.FirstOrDefault();
                                                    int _Total = 0;
                                                    if (int.TryParse(Temp.ColumnTotal, out _Total))
                                                    {
                                                        Temp.ColumnTotal = (_Total + Dftint).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    AirOut.Web.Models.TotalColumn Temp = new AirOut.Web.Models.TotalColumn();
                                                    Temp.ColumnName = itemColumn.Key;
                                                    Temp.ColumnIndex = itemColumn.Value;
                                                    Temp.ColumnTotal = Dftint.ToString();
                                                    ArrColumnTotal.Add(Temp);
                                                }

                                                #endregion
                                                break;
                                            case "int32":
                                                int Dftint32 = 0;
                                                if (int.TryParse(retVal.ToString(), out Dftint32))
                                                {
                                                    style.Number = 1;
                                                    if (Dftint32 == 0)
                                                        obj = "";
                                                    style.Font.Color = System.Drawing.Color.Blue;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                                if (IsToTal && Dftint32 == 0)//如果是 汇总行 不累加到 总汇总中
                                                    obj = "";
                                                #region int decimal double 类型 汇总

                                                if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                    break;
                                                wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                if (wheredictTotal.Any())
                                                {
                                                    var Temp32 = wheredictTotal.FirstOrDefault();
                                                    int _Total32 = 0;
                                                    if (int.TryParse(Temp32.ColumnTotal, out _Total32))
                                                    {
                                                        Temp32.ColumnTotal = (_Total32 + Dftint32).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    AirOut.Web.Models.TotalColumn Temp32 = new AirOut.Web.Models.TotalColumn();
                                                    Temp32.ColumnName = itemColumn.Key;
                                                    Temp32.ColumnIndex = itemColumn.Value;
                                                    Temp32.ColumnTotal = Dftint32.ToString();
                                                    ArrColumnTotal.Add(Temp32);
                                                }

                                                #endregion
                                                break;
                                            case "int64":
                                                int Dftint64 = 0;
                                                if (int.TryParse(retVal.ToString(), out Dftint64))
                                                {
                                                    style.Number = 1;
                                                    if (Dftint64 == 0)
                                                        obj = "";
                                                    style.Font.Color = System.Drawing.Color.Blue;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                                if (IsToTal && Dftint64 == 0)//如果是 汇总行 不累加到 总汇总中
                                                    obj = "";
                                                #region int decimal double 类型 汇总

                                                if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                    break;
                                                wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                if (wheredictTotal.Any())
                                                {
                                                    var Temp64 = wheredictTotal.FirstOrDefault();
                                                    int _Total64 = 0;
                                                    if (int.TryParse(Temp64.ColumnTotal, out _Total64))
                                                    {
                                                        Temp64.ColumnTotal = (_Total64 + Dftint64).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    AirOut.Web.Models.TotalColumn Temp64 = new AirOut.Web.Models.TotalColumn();
                                                    Temp64.ColumnName = itemColumn.Key;
                                                    Temp64.ColumnIndex = itemColumn.Value;
                                                    Temp64.ColumnTotal = Dftint64.ToString();
                                                    ArrColumnTotal.Add(Temp64);
                                                }

                                                #endregion
                                                break;
                                            case "string":
                                                style.Number = 9;
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                break;
                                            case "datetime":
                                                int TDatetime = 0;
                                                if (int.TryParse(retVal.ToString(), out TDatetime))
                                                {
                                                    style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                }
                                                else
                                                {
                                                    DateTime DftDateTime = new DateTime();
                                                    if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                                    {
                                                        style.Custom = "yyyy-MM-dd HH:mm:ss";
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    }
                                                }
                                                break;
                                            case "bool":
                                                bool Dftbool = false;
                                                if (bool.TryParse(retVal.ToString(), out Dftbool))
                                                {
                                                    style.Number = 9;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    if (Dftbool)
                                                    {
                                                        style.Font.Color = System.Drawing.Color.Green;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        obj = "是";
                                                    }
                                                    else
                                                    {
                                                        style.Font.Color = System.Drawing.Color.Red;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        obj = "否";
                                                    }
                                                }
                                                break;
                                            case "boolean":
                                                bool Dftboolean = false;
                                                if (bool.TryParse(retVal.ToString(), out Dftboolean))
                                                {
                                                    style.Number = 9;
                                                    sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                    if (Dftboolean)
                                                    {
                                                        style.Font.Color = System.Drawing.Color.Green;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        obj = "是";
                                                    }
                                                    else
                                                    {
                                                        style.Font.Color = System.Drawing.Color.Red;
                                                        sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                        obj = "否";
                                                    }
                                                }
                                                break;
                                            case "decimal":
                                                decimal Dftdecimal = 0;
                                                if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                                {
                                                    if (Dftdecimal == 0)
                                                        obj = "";
                                                }
                                                if (IsToTal && Dftdecimal == 0)//如果是 汇总行 不累加到 总汇总中
                                                    obj = "";
                                                #region int decimal double 类型 汇总

                                                if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                    break;
                                                wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                if (wheredictTotal.Any())
                                                {
                                                    var Tempdecimal = wheredictTotal.FirstOrDefault();
                                                    decimal _Totaldecimal = 0;
                                                    if (decimal.TryParse(Tempdecimal.ColumnTotal, out _Totaldecimal))
                                                    {
                                                        Tempdecimal.ColumnTotal = (_Totaldecimal + Dftdecimal).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    AirOut.Web.Models.TotalColumn Tempdecima = new AirOut.Web.Models.TotalColumn();
                                                    Tempdecima.ColumnName = itemColumn.Key;
                                                    Tempdecima.ColumnIndex = itemColumn.Value;
                                                    Tempdecima.ColumnTotal = Dftdecimal.ToString();
                                                    ArrColumnTotal.Add(Tempdecima);
                                                }

                                                #endregion
                                                break;
                                            case "double":
                                                double Dftdouble = 0;
                                                if (double.TryParse(retVal.ToString(), out Dftdouble))
                                                {
                                                    style.Number = 1;
                                                    if (Dftdouble == 0)
                                                        obj = "";
                                                }
                                                if (IsToTal && Dftdouble == 0)//如果是 汇总行 不累加到 总汇总中
                                                    obj = "";
                                                #region int decimal double 类型 汇总

                                                if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                    break;
                                                wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                if (wheredictTotal.Any())
                                                {
                                                    var Tempdouble = wheredictTotal.FirstOrDefault();
                                                    double _Totaldouble = 0;
                                                    if (double.TryParse(Tempdouble.ColumnTotal, out _Totaldouble))
                                                    {
                                                        Tempdouble.ColumnTotal = (_Totaldouble + Dftdouble).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    AirOut.Web.Models.TotalColumn Tempdouble = new AirOut.Web.Models.TotalColumn();
                                                    Tempdouble.ColumnName = itemColumn.Key;
                                                    Tempdouble.ColumnIndex = itemColumn.Value;
                                                    Tempdouble.ColumnTotal = Dftdouble.ToString();
                                                    ArrColumnTotal.Add(Tempdouble);
                                                }

                                                #endregion
                                                break;
                                            case "float":
                                                float Dftfloat = 0;
                                                if (float.TryParse(retVal.ToString(), out Dftfloat))
                                                {
                                                    style.Number = 1;
                                                    if (Dftfloat == 0)
                                                        obj = "";
                                                }
                                                if (IsToTal && Dftfloat == 0)//如果是 汇总行 不累加到 总汇总中
                                                    obj = "";
                                                #region int decimal double 类型 汇总

                                                if (IsToTal)//如果是 汇总行 不累加到 总汇总中
                                                    break;
                                                wheredictTotal = ArrColumnTotal.Where(x => x.ColumnName == itemColumn.Key);
                                                if (wheredictTotal.Any())
                                                {
                                                    var Tempfloat = wheredictTotal.FirstOrDefault();
                                                    float _Totalfloat = 0;
                                                    if (float.TryParse(Tempfloat.ColumnTotal, out _Totalfloat))
                                                    {
                                                        Tempfloat.ColumnTotal = (_Totalfloat + Dftfloat).ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    AirOut.Web.Models.TotalColumn Tempfloat = new AirOut.Web.Models.TotalColumn();
                                                    Tempfloat.ColumnName = itemColumn.Key;
                                                    Tempfloat.ColumnIndex = itemColumn.Value;
                                                    Tempfloat.ColumnTotal = Dftfloat.ToString();
                                                    ArrColumnTotal.Add(Tempfloat);
                                                }

                                                #endregion
                                                break;
                                            default:
                                                style.Number = 9;
                                                sheet.Cells[RowIndex, CellsIndex].SetStyle(style);
                                                break;
                                        }
                                    }

                                    #endregion

                                    #region  自动列宽

                                    if (obj != null)
                                    {
                                        System.Drawing.Font font = new System.Drawing.Font(style.Font.Name, style.Font.Size);
                                        double StrWidth = 0;
                                        StrWidth = g.MeasureString(obj.ToString(), font).Width / 7.384;
                                        if (StrWidth > MaxCellWidth)
                                        {
                                            StrWidth = MaxCellWidth;
                                        }
                                        if (sheet.Cells.Columns[CellsIndex].Width < StrWidth)
                                            sheet.Cells.Columns[CellsIndex].Width = StrWidth;
                                    }

                                    #endregion

                                    #region 设置背景色
                                    //else
                                    //{
                                    //    var _style = sheet.Cells[RowIndex, CellsIndex].GetStyle();
                                    //    //_style.BackgroundColor = System.Drawing.Color.LightSkyBlue;
                                    //    _style.ForegroundColor = System.Drawing.Color.FromArgb(230, 211, 211, 211);
                                    //    _style.Pattern = Aspose.Cells.BackgroundType.Solid;
                                    //    sheet.Cells[RowIndex, CellsIndex].SetStyle(_style);
                                    //}
                                    #endregion

                                    if (itemColumn.Key == "序号")
                                        sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineRowIndex;
                                    else
                                        sheet.Cells[RowIndex, CellsIndex].Value = obj;
                                    //CellsIndex++;
                                }
                                else
                                {
                                    if (itemColumn.Key == "序号")
                                        sheet.Cells[RowIndex, CellsIndex].Value = RowIndex - HeadLineRowIndex;
                                    else
                                        sheet.Cells[RowIndex, CellsIndex].Value = "";
                                    //CellsIndex++;
                                }
                                #endregion
                            }

                            RowIndex++;
                        }
                    }

                    #endregion

                    //统计 赋值
                    if (ArrColumnTotal.Any())
                    {
                        int NumIndex = 0;

                        #region 设置列格式

                        for (int i = 0; i < MaxCells; i++)
                        {
                            CellStyle.Font.Color = System.Drawing.Color.Black;
                            CellStyle.Number = 0;//设置默认单元格格式为 常规
                            sheet.Cells[RowIndex, i].SetStyle(CellStyle);
                        }

                        #endregion

                        foreach (var itemTotal in ArrColumnTotal.OrderBy(x => x.ColumnIndex))
                        {
                            var style = CellStyle;
                            if (NumIndex == 0)
                            {
                                if (itemTotal.ColumnIndex > 0)
                                {
                                    sheet.Cells[RowIndex, itemTotal.ColumnIndex - 1].Value = "合计：";
                                }
                                else
                                {
                                    int ColIndex = itemTotal.ColumnIndex;
                                    while (ArrColumnTotal.Any(x => x.ColumnIndex == ColIndex))
                                    {
                                        ColIndex++;
                                    }

                                    sheet.Cells[RowIndex, ColIndex].Value = "合计：";
                                }
                            }
                            object objVal = itemTotal.ColumnTotal;

                            style.Font.Color = System.Drawing.Color.Blue;
                            sheet.Cells[RowIndex, itemTotal.ColumnIndex].SetStyle(style);
                            sheet.Cells[RowIndex, itemTotal.ColumnIndex].Value = objVal;
                            NumIndex++;
                        }
                    }
                    index++;
                }

                string FileDownLoadPath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "\\FileDownLoad\\";
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath(FileDownLoadPath));
                if (!dir.Exists)
                    dir.Create();
                string FileName = (dir.FullName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + (new Random()).Next(1, 999).ToString("000") + ".xls");
                workbook.Save(FileName, Aspose.Cells.SaveFormat.Excel97To2003);
                System.Threading.Thread.Sleep(100);
                //return workbook.SaveToStream().ToArray();
                return FileName;
            }
            catch (Exception)
            {
                //return OutPutExcelByListModel(ObjT);
                return "";
            }
        }

        /// <summary>
        /// 复制Sheet
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="Num"></param>
        public static void CopySheet(Aspose.Cells.Workbook workbook, int Num)
        {
            List<Aspose.Cells.Worksheet> ArrWorksheet = new List<Aspose.Cells.Worksheet>();
            //赋值源
            Aspose.Cells.Worksheet sheet = (Aspose.Cells.Worksheet)workbook.Worksheets[0];
            for (int i = 1; i <= Num; i++)
            {
                workbook.Worksheets.Add(sheet.Name + (i).ToString("00"));
                Aspose.Cells.Worksheet _sheet = workbook.Worksheets[i];
                _sheet.Copy(sheet);
            }
        }

        /// <summary>
        /// 获取类的中文名
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetDisplayName(Type dataType, string fieldName)
        {
            // First look into attributes on a type and it's parents
            DisplayAttribute attr;
            attr = (DisplayAttribute)dataType.GetProperty(fieldName).GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

            if (attr == null)
            {
                return String.Empty;
            }
            else
                return (attr != null) ? attr.GetName() : String.Empty;
        }

        public static string GetDisplayName1(Type dataType, string fieldName)
        {
            // First look into attributes on a type and it's parents
            DisplayNameAttribute attr;
            attr = (DisplayNameAttribute)dataType.GetProperty(fieldName).GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();

            // Look for [MetadataType] attribute in type hierarchy
            // http://stackoverflow.com/questions/1910532/attribute-isdefined-doesnt-see-attributes-applied-with-metadatatype-class
            if (attr == null)
            {
                MetadataTypeAttribute metadataType = (MetadataTypeAttribute)dataType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                if (metadataType != null)
                {
                    var property = metadataType.MetadataClassType.GetProperty(fieldName);
                    if (property != null)
                    {
                        attr = (DisplayNameAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    }
                }
            }
            return (attr != null) ? attr.DisplayName : String.Empty;
        }

        //根据表名和字段名 获取数据类型
        public static string GetDataTypeByTable_Column(Object TableClass = null, string ColumnName = "")
        {
            string ret = "";
            try
            {
                Object Obj_Table = TableClass;
                if (Obj_Table != null)
                {
                    System.Reflection.PropertyInfo[] PropertyInfos = Obj_Table.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    //遍历该model实体的所有字段
                    foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                    {
                        if (fi.Name.ToLower() == ColumnName.ToLower())
                        {
                            ret = fi.PropertyType.Name;
                            if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                var Arguments = fi.PropertyType.GetGenericArguments();
                                ret = Arguments[0].Name;
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {

            }
            return ret;
        }

        //根据 类 和 字段 设置值
        public static void setProtityValue(Object TableClass = null, string FiledName = "", object DefaultValue = null)
        {
            try
            {
                if (TableClass == null || string.IsNullOrEmpty(FiledName))
                    return;

                System.Reflection.PropertyInfo[] PropertyInfos = TableClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                {
                    //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                    String _FiledName = fi.Name;
                    object s = fi.GetValue(TableClass, null);
                    string DataType = "";
                    if (fi.Name.ToLower() == FiledName.ToLower())
                    {
                        DataType = fi.PropertyType.Name;
                        if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var Arguments = fi.PropertyType.GetGenericArguments();
                            DataType = Arguments[0].Name;
                        }
                        switch (DataType.ToLower())
                        {
                            case "int":
                                int Dftint = 0;
                                if (int.TryParse(DefaultValue.ToString(), out Dftint))
                                {
                                    fi.SetValue(TableClass, Dftint, null);
                                }
                                break;
                            case "int32":
                                int Dftint32 = 0;
                                if (int.TryParse(DefaultValue.ToString(), out Dftint32))
                                {
                                    fi.SetValue(TableClass, Dftint32, null);
                                }
                                break;
                            case "int64":
                                int Dftint64 = 0;
                                if (int.TryParse(DefaultValue.ToString(), out Dftint64))
                                {
                                    fi.SetValue(TableClass, Dftint64, null);
                                }
                                break;
                            case "decimal":
                                decimal Dftdecimal = 0;
                                if (decimal.TryParse(DefaultValue.ToString(), out Dftdecimal))
                                {
                                    fi.SetValue(TableClass, Dftdecimal, null);
                                }
                                break;
                            case "double":
                                double Dftdouble = 0;
                                if (double.TryParse(DefaultValue.ToString(), out Dftdouble))
                                {
                                    fi.SetValue(TableClass, Dftdouble, null);
                                }
                                break;
                            case "float":
                                float Dftfloat = 0;
                                if (float.TryParse(DefaultValue.ToString(), out Dftfloat))
                                {
                                    fi.SetValue(TableClass, Dftfloat, null);
                                }
                                break;
                            case "string":
                                fi.SetValue(TableClass, DefaultValue, null);
                                break;
                            case "datetime":
                                int TDatetime = 0;
                                if (int.TryParse(DefaultValue.ToString(), out TDatetime))
                                {
                                    fi.SetValue(TableClass, DateTime.Now.AddDays(TDatetime), null);
                                }
                                else
                                {
                                    DateTime DftDateTime = new DateTime();
                                    if (DateTime.TryParse(DefaultValue.ToString(), out DftDateTime))
                                    {
                                        fi.SetValue(TableClass, DftDateTime, null);
                                    }
                                }
                                break;
                            case "bool":
                                bool Dftbool = false;
                                if (bool.TryParse(DefaultValue.ToString(), out Dftbool))
                                {
                                    fi.SetValue(TableClass, Dftbool, null);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
            }
            catch
            {

            }
        }

        //根据 类 和 字段 获取值
        public static object GetProtityValue(Object TableClass = null, string FiledName = "")
        {
            try
            {
                object retValue = "";
                Dictionary<string, object> dict = new Dictionary<string, object>();

                if (TableClass == null || string.IsNullOrEmpty(FiledName))
                    return null;

                System.Reflection.PropertyInfo[] PropertyInfos = TableClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                {
                    DisplayAttribute disAttritem = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                    //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                    String _FiledName = fi.Name;
                    string _displayName = disAttritem == null ? "" : disAttritem.Name;
                    object fival = fi.GetValue(TableClass, null);
                    string DataType = "";
                    if (fi.Name.ToLower() == FiledName.ToLower() || _displayName == FiledName)
                    {
                        var retVal = fi.GetValue(TableClass);
                        DataType = fi.PropertyType.Name;
                        if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var Arguments = fi.PropertyType.GetGenericArguments();
                            DataType = Arguments[0].Name;
                        }
                        switch (DataType.ToLower())
                        {
                            case "int":
                                int Dftint = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint))
                                {
                                    retVal = (object)Dftint;
                                    retValue = retVal;
                                }
                                break;
                            case "int32":
                                int Dftint32 = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint32))
                                {
                                    retVal = (object)Dftint32;
                                    retValue = retVal;
                                }
                                break;
                            case "int64":
                                int Dftint64 = 0;
                                if (int.TryParse(retVal.ToString(), out Dftint64))
                                {
                                    retVal = (object)Dftint64;
                                    retValue = retVal;
                                }
                                break;

                            case "string":
                                retValue = retVal;
                                break;
                            case "datetime":
                                int TDatetime = 0;
                                if (int.TryParse(retVal.ToString(), out TDatetime))
                                {
                                    retVal = (object)TDatetime;
                                    retValue = retVal;
                                }
                                else
                                {
                                    DateTime DftDateTime = new DateTime();
                                    if (DateTime.TryParse(retVal.ToString(), out DftDateTime))
                                    {
                                        retVal = (object)DftDateTime;
                                        retValue = retVal;
                                    }
                                }
                                break;
                            case "bool":
                                bool Dftbool = false;
                                if (bool.TryParse(retVal.ToString(), out Dftbool))
                                {
                                    retVal = (object)Dftbool;
                                    retValue = retVal;
                                }
                                break;
                            case "decimal":
                                decimal Dftdecimal = 0;
                                if (decimal.TryParse(retVal.ToString(), out Dftdecimal))
                                {
                                    string str = Dftdecimal.ToString("f2");
                                    retVal = (object)str;
                                    retValue = retVal;
                                }
                                break;
                            case "double":
                                double Dftdouble = 0;
                                if (double.TryParse(retVal.ToString(), out Dftdouble))
                                {
                                    string str = Dftdouble.ToString("f2");
                                    retVal = (object)str;
                                    retValue = retVal;
                                }
                                break;
                            case "float":
                                float Dftfloat = 0;
                                if (float.TryParse(retVal.ToString(), out Dftfloat))
                                {
                                    string str = Dftfloat.ToString("f2");
                                    retVal = (object)str;
                                    retValue = retVal;
                                }
                                break;
                            default:
                                retValue = retVal;
                                break;
                        }
                        dict.Add(DataType, fival);
                    }
                }
                return retValue;// dict;
            }
            catch
            {
                return null;
            }
        }

    }
}