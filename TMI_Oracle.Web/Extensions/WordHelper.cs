using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Aspose.Words;
using TMI.Web.Extensions;
using System.Drawing;
using Aspose.Words.Tables;

namespace TMI.Web
{
    public class WordHelper
    {
        public WordHelper()
        {
            Aspose.Words.License license = new Aspose.Words.License();
            SetAsposeLicense(license);
        }

        private static void SetAsposeLicense(Aspose.Words.License license)
        {
            //            string strLic = @"<License>
            //                                  <Data>
            //                                    <SerialNumber>aed83727-21cc-4a91-bea4-2607bf991c21</SerialNumber>
            //                                    <EditionType>Enterprise</EditionType>
            //                                    <Products>
            //                                      <Product>Aspose.Total</Product>
            //                                    </Products>
            //                                  </Data>
            //                                  <Signature>CxoBmxzcdRLLiQi1kzt5oSbz9GhuyHHOBgjTf5w/wJ1V+lzjBYi8o7PvqRwkdQo4tT4dk3PIJPbH9w5Lszei1SV/smkK8SCjR8kIWgLbOUFBvhD1Fn9KgDAQ8B11psxIWvepKidw8ZmDmbk9kdJbVBOkuAESXDdtDEDZMB/zL7Y=</Signature>
            //                                </License>";

            //            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(strLic));
            //license.SetLicense(ms);

            string path = HttpContext.Current.Server.MapPath("/Aspose/License.lic");
            license.SetLicense(path);
        }

        /// <summary>
        /// 设置word模板的 书签值，生成新的word文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wordModelPath">模板名称，会自动找模板</param>
        /// <param name="_SaveFormat">输出文件格式</param>
        /// <returns></returns>
        public static ReturnMsg SetWordModel_BookMarkByT<T>(T ObjT, string wordModelPath = "", SaveFormat _SaveFormat = SaveFormat.Doc)
            where T : class,new()
        {
            ReturnMsg retMsg = new ReturnMsg(true, "");
            try
            {
                wordModelPath = GetWordModelPath<T>(ObjT, wordModelPath);

                string wordModel_Path = wordModelPath;
                if (wordModelPath.IndexOf(":") < 0)
                    wordModel_Path = HttpContext.Current.Server.MapPath(wordModelPath);

                if (!File.Exists(wordModel_Path))
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件不存在";
                    return retMsg;
                }
                string FileClass = Common.GetFileClass(wordModel_Path);
                if (FileClass != "208207" && FileClass != "8075")
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件格式不正确";
                    return retMsg;
                }
                Document doc = new Aspose.Words.Document();

                try
                {
                    doc = new Aspose.Words.Document(wordModel_Path);
                }
                catch (Exception ex)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = Common.GetExceptionMsg(ex);
                    return retMsg;
                }

                if (doc == null)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件解析出错";
                    return retMsg;
                }
                if (ObjT == null)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "设置模板文件的数据不能为空";
                    return retMsg;
                }
                BookmarkCollection bookMarks = doc.Range.Bookmarks;
                if (bookMarks.Count <= 0)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件，不存在任何书签";
                    return retMsg;
                }
                else
                {
                    //使用DocumentBuilder对象插入一些文档对象，如插入书签，插入文本框，插入复选框，插入一个段落，插入空白页，追加或另一个word文件的内容等。
                    var builder = new DocumentBuilder(doc);

                    foreach (var bookmark in bookMarks)
                    {
                        var book_mark = bookmark as Aspose.Words.Bookmark;
                        book_mark.Text = "";// 清掉标示
                        //定位到指定位置进行插入操作
                        builder.MoveToBookmark(book_mark.Name);
                        PropertyInfo OPropertyInfo = Common.GetProtityInfoByFieldName(ObjT, book_mark.Name);
                        if (OPropertyInfo != null)
                        {
                            var objval = OPropertyInfo.GetValue(ObjT);
                            string DataType = OPropertyInfo.PropertyType.Name;
                            var DataTypeInfo = OPropertyInfo.PropertyType.GetTypeInfo();
                            if (objval != null)
                            {
                                //判断是否是泛型
                                if (OPropertyInfo.PropertyType.IsGenericType && OPropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = OPropertyInfo.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;

                                    #region  创建List<T> 实例 并赋值

                                    Type ListType = typeof(List<>);
                                    ListType = ListType.MakeGenericType(OPropertyInfo.PropertyType);
                                    var ObjListT = Activator.CreateInstance(ListType);
                                    MethodInfo OListMethodInfo = ListType.GetMethod("Add");
                                    List<object> arrRepParam = new List<object>();
                                    arrRepParam.Add(objval);
                                    OListMethodInfo.Invoke(ObjListT, arrRepParam.ToArray());

                                    #endregion

                                    #region  创建WordHelper 实例 并调用 WriteTableByList 方法

                                    Type WordHelperType = typeof(WordHelper);
                                    var ObjWordHelper = Activator.CreateInstance(WordHelperType);
                                    MethodInfo OMethodInfo = WordHelperType.GetMethod("WriteTableByList").MakeGenericMethod(OPropertyInfo.PropertyType);
                                    List<object> arrWriteTableByListParam = new List<object>();
                                    arrWriteTableByListParam.Add(bookmark);
                                    arrWriteTableByListParam.Add(doc);
                                    arrWriteTableByListParam.Add(ObjListT);
                                    OMethodInfo.Invoke(ObjWordHelper, arrWriteTableByListParam.ToArray());

                                    #endregion
                                }
                                //判断是否派生自IEnumerable
                                else if (OPropertyInfo.PropertyType.GetInterface("IEnumerable", false) != null && DataType.ToLower().IndexOf("string") < 0)
                                {
                                    var Arrobjval = objval as System.Collections.IEnumerable;
                                    #region  创建List<T> 实例 并赋值

                                    Type ListTType = null;//泛型类
                                    var IEnumerableTypes = OPropertyInfo.PropertyType.GetGenericArguments();
                                    if (IEnumerableTypes.Any())
                                        ListTType = IEnumerableTypes[0];

                                    Type ListType = typeof(List<>);
                                    ListType = ListType.MakeGenericType(ListTType);
                                    var ObjListT = Activator.CreateInstance(ListType);
                                    MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                    foreach (var item in Arrobjval)
                                    {
                                        AddMethodInfo.Invoke(ObjListT, new object[] { item });
                                    }

                                    //var Methods = objval.GetType().GetMethods();
                                    //MethodInfo OListMethodInfo = objval.GetType().GetMethod("Count");
                                    //if (OListMethodInfo != null) 
                                    //{
                                    //    //OListMethodInfo = OListMethodInfo.MakeGenericMethod(ListTType);
                                    //    MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                    //    MethodInfo ToListMethodInfo = ListType.GetMethod("Add");
                                    //    if (ToListMethodInfo != null)
                                    //    {
                                    //        ObjListT = ToListMethodInfo.Invoke(retobj, null);
                                    //    }
                                    //}

                                    //MethodInfo OListMethodInfo = objval.GetType().GetMethod("GetEnumerator");
                                    //if (OListMethodInfo != null)
                                    //{
                                    //    var Enumeratorobj = OListMethodInfo.Invoke(objval, null) as System.Collections.IDictionaryEnumerator;
                                    //    Enumeratorobj.
                                    //}

                                    //List<object> arrRepParam = new List<object>();
                                    //foreach (var item in (objval as System.Collections.IList))
                                    //{
                                    //    arrRepParam = new List<object>();
                                    //    arrRepParam.Add(item);
                                    //    OListMethodInfo.Invoke(ObjListT, arrRepParam.ToArray());
                                    //}

                                    #endregion

                                    #region  创建WordHelper 实例 并调用 WordHelper 方法

                                    Type WordHelperType = typeof(WordHelper);
                                    var ObjWordHelper = Activator.CreateInstance(WordHelperType);
                                    MethodInfo[] MethodInfos = WordHelperType.GetMethods();
                                    MethodInfo OMethodInfo = MethodInfos.Where(x => x.Name.StartsWith("WriteTableByList") && x.IsGenericMethod).FirstOrDefault().MakeGenericMethod(ListTType);
                                    List<object> arrWriteTableByListParam = new List<object>();
                                    arrWriteTableByListParam.Add(bookmark);
                                    arrWriteTableByListParam.Add(doc);
                                    arrWriteTableByListParam.Add(ObjListT);

                                    OMethodInfo.Invoke(ObjWordHelper, arrWriteTableByListParam.ToArray());

                                    #endregion
                                }
                                else
                                {
                                    builder.Write(objval.ToString());
                                }
                            }
                        }
                    }

                    string NewFilePath = SaveDoc(doc, _SaveFormat);
                    retMsg.retResult = true;
                    retMsg.MsgStr = NewFilePath;
                }
            }
            catch (Exception ex)
            {
                retMsg.retResult = false;
                retMsg.MsgStr = Common.GetExceptionMsg(ex);
            }
            return retMsg;
        }


        /// <summary>
        /// 设置word模板的 书签值，生成新的word文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wordModelPath">模板名称，会自动找模板</param>
        /// <param name="ArrValue">数据列表</param>
        /// <param name="arrnum">arrnum</param>
        /// <returns></returns>
        public static ReturnMsg SetWordModel_BookMarkByT<T>(T ObjT, string wordModelPath = "", List<T> ArrValue = null, int arrnum = 0, string loginName = "" ,SaveFormat _SaveFormat = SaveFormat.Doc)
            where T : class,new()
        {
            ReturnMsg retMsg = new ReturnMsg(true, "");
            try
            {
                wordModelPath = GetWordModelPath<T>(ObjT, wordModelPath);

                string wordModel_Path = wordModelPath;
                if (wordModelPath.IndexOf(":") < 0)
                    wordModel_Path = HttpContext.Current.Server.MapPath(wordModelPath);

                if (!File.Exists(wordModel_Path))
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件不存在";
                    return retMsg;
                }
                string FileClass = Common.GetFileClass(wordModel_Path);
                if (FileClass != "208207" && FileClass != "8075")
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件格式不正确";
                    return retMsg;
                }
                Document doc = new Aspose.Words.Document();

                try
                {
                    doc = new Aspose.Words.Document(wordModel_Path);
                }
                catch (Exception ex)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = Common.GetExceptionMsg(ex);
                    return retMsg;
                }

                if (doc == null)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件解析出错";
                    return retMsg;
                }
                if (ObjT == null)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "设置模板文件的数据不能为空";
                    return retMsg;
                }
                BookmarkCollection bookMarks = doc.Range.Bookmarks;
                if (bookMarks.Count <= 0)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件，不存在任何书签";
                    return retMsg;
                }
                else
                {
                    //使用DocumentBuilder对象插入一些文档对象，如插入书签，插入文本框，插入复选框，插入一个段落，插入空白页，追加或另一个word文件的内容等。
                    var builder = new DocumentBuilder(doc);
                    double TotalPieces = 0;
                    double TotalWeight = 0;
                    if (ArrValue != null)
                    {
                        for (var i = 0; i < ArrValue.Count; i++)
                        {
                            object Pieces = Common.GetProtityValue(ArrValue[i], "Pieces");
                            if (Pieces != null && Pieces != "")
                            {
                                TotalPieces += double.Parse(Pieces.ToString());
                            }
                            object Weight = Common.GetProtityValue(ArrValue[i], "Weight");
                            if (Weight != null && Weight != "")
                            {
                                TotalWeight += double.Parse(Weight.ToString());
                            }
                        }
                    }
                    foreach (var bookmark in bookMarks)
                    {
                        var book_mark = bookmark as Aspose.Words.Bookmark;
                        book_mark.Text = "";// 清掉标示

                        //定位到指定位置进行插入操作
                        builder.MoveToBookmark(book_mark.Name);

                        PropertyInfo OPropertyInfo = Common.GetProtityInfoByFieldName(ObjT, book_mark.Name);
                        if (OPropertyInfo != null)
                        {
                            var objval = OPropertyInfo.GetValue(ObjT);
                            string DataType = OPropertyInfo.PropertyType.Name;
                            var DataTypeInfo = OPropertyInfo.PropertyType.GetTypeInfo();
                            if (objval != null)
                            {
                                //判断是否是泛型
                                if (OPropertyInfo.PropertyType.IsGenericType && OPropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var Arguments = OPropertyInfo.PropertyType.GetGenericArguments();
                                    DataType = Arguments[0].Name;

                                    #region  创建List<T> 实例 并赋值

                                    Type ListType = typeof(List<>);
                                    ListType = ListType.MakeGenericType(OPropertyInfo.PropertyType);
                                    var ObjListT = Activator.CreateInstance(ListType);
                                    MethodInfo OListMethodInfo = ListType.GetMethod("Add");
                                    List<object> arrRepParam = new List<object>();
                                    arrRepParam.Add(objval);
                                    OListMethodInfo.Invoke(ObjListT, arrRepParam.ToArray());

                                    #endregion

                                    #region  创建WordHelper 实例 并调用 WriteTableByList 方法

                                    Type WordHelperType = typeof(WordHelper);
                                    var ObjWordHelper = Activator.CreateInstance(WordHelperType);
                                    MethodInfo OMethodInfo = WordHelperType.GetMethod("WriteTableByList").MakeGenericMethod(OPropertyInfo.PropertyType);
                                    List<object> arrWriteTableByListParam = new List<object>();
                                    arrWriteTableByListParam.Add(bookmark);
                                    arrWriteTableByListParam.Add(doc);
                                    arrWriteTableByListParam.Add(ObjListT);
                                    OMethodInfo.Invoke(ObjWordHelper, arrWriteTableByListParam.ToArray());

                                    #endregion
                                }
                                //判断是否派生自IEnumerable
                                else if (OPropertyInfo.PropertyType.GetInterface("IEnumerable", false) != null && DataType.ToLower().IndexOf("string") < 0)
                                {
                                    var Arrobjval = objval as System.Collections.IEnumerable;
                                    #region  创建List<T> 实例 并赋值

                                    Type ListTType = null;//泛型类
                                    var IEnumerableTypes = OPropertyInfo.PropertyType.GetGenericArguments();
                                    if (IEnumerableTypes.Any())
                                        ListTType = IEnumerableTypes[0];

                                    Type ListType = typeof(List<>);
                                    ListType = ListType.MakeGenericType(ListTType);
                                    var ObjListT = Activator.CreateInstance(ListType);
                                    MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                    foreach (var item in Arrobjval)
                                    {
                                        AddMethodInfo.Invoke(ObjListT, new object[] { item });
                                    }

                                    //var Methods = objval.GetType().GetMethods();
                                    //MethodInfo OListMethodInfo = objval.GetType().GetMethod("Count");
                                    //if (OListMethodInfo != null) 
                                    //{
                                    //    //OListMethodInfo = OListMethodInfo.MakeGenericMethod(ListTType);
                                    //    MethodInfo AddMethodInfo = ListType.GetMethod("Add");
                                    //    MethodInfo ToListMethodInfo = ListType.GetMethod("Add");
                                    //    if (ToListMethodInfo != null)
                                    //    {
                                    //        ObjListT = ToListMethodInfo.Invoke(retobj, null);
                                    //    }
                                    //}

                                    //MethodInfo OListMethodInfo = objval.GetType().GetMethod("GetEnumerator");
                                    //if (OListMethodInfo != null)
                                    //{
                                    //    var Enumeratorobj = OListMethodInfo.Invoke(objval, null) as System.Collections.IDictionaryEnumerator;
                                    //    Enumeratorobj.
                                    //}

                                    //List<object> arrRepParam = new List<object>();
                                    //foreach (var item in (objval as System.Collections.IList))
                                    //{
                                    //    arrRepParam = new List<object>();
                                    //    arrRepParam.Add(item);
                                    //    OListMethodInfo.Invoke(ObjListT, arrRepParam.ToArray());
                                    //}

                                    #endregion

                                    #region  创建WordHelper 实例 并调用 WordHelper 方法

                                    Type WordHelperType = typeof(WordHelper);
                                    var ObjWordHelper = Activator.CreateInstance(WordHelperType);
                                    MethodInfo[] MethodInfos = WordHelperType.GetMethods();
                                    MethodInfo OMethodInfo = MethodInfos.Where(x => x.Name.StartsWith("WriteTableByList") && x.IsGenericMethod).FirstOrDefault().MakeGenericMethod(ListTType);
                                    List<object> arrWriteTableByListParam = new List<object>();
                                    arrWriteTableByListParam.Add(bookmark);
                                    arrWriteTableByListParam.Add(doc);
                                    arrWriteTableByListParam.Add(ObjListT);

                                    OMethodInfo.Invoke(ObjWordHelper, arrWriteTableByListParam.ToArray());

                                    #endregion
                                }
                                else
                                {
                                    builder.Write(objval.ToString());
                                }
                            }
                        }

                        #region 仓库明细数据长宽高件数
                        if (book_mark.Name.IndexOf("CM_Length") > -1 || book_mark.Name.IndexOf("CM_Width") > -1 || book_mark.Name.IndexOf("CM_Height") > -1 || book_mark.Name.IndexOf("CM_Piece") > -1)
                        {
                            foreach (var value in ArrValue)
                            {
                                var valStr = Common.GetProtityValue(value, "ArrPdfJCDList");
                                dynamic temp = valStr;
                                
                                if (book_mark.Name.IndexOf("CM_Length") > -1 || book_mark.Name.IndexOf("CM_Height") > -1)
                                {
                                    var num = Convert.ToInt32(book_mark.Name.Substring(9, 1).ToString());
                                    if (arrnum > num)
                                    {
                                        var name = book_mark.Name.Substring(0, 9).ToString();
                                        valStr.GetType().GetProperties();
                                        var item = temp[num];
                                        var val = Common.GetProtityValue(item, name);
                                        builder.Write(val == null ? "" : val);
                                    }
                                }
                                else if (book_mark.Name.IndexOf("CM_Width") > -1 || book_mark.Name.IndexOf("CM_Piece") > -1)
                                {
                                    var num = Convert.ToInt32(book_mark.Name.Substring(8, 1).ToString());
                                    if (arrnum > num)
                                    {
                                        var name = book_mark.Name.Substring(0, 8).ToString();
                                        var item = temp[num];
                                        var val = Common.GetProtityValue(item, name);
                                        builder.Write(val == null ? "" : val);
                                    }
                                }
                                
                            }
                        }
                        if (book_mark.Name == "loginName")
                        {
                            builder.Write(loginName);
                        }
                        #endregion

                        if (book_mark.Name == "BilOLdingList")
                        {
                            WriteTableByList<T>(book_mark, doc, ArrValue, "ExportBilOfLadinPDF");
                        }
                        if (book_mark.Name == "List")
                        {
                            WriteTableByList<T>(book_mark, doc, ArrValue, "");
                        }
                        if (book_mark.Name == "TotalHawb")
                        {
                            builder.Write(ArrValue.Count.ToString());
                        }
                        if (book_mark.Name == "TotalWeight")
                        {
                            builder.Write(TotalWeight.ToString());
                        }
                        if (book_mark.Name == "TotalPieces")
                        {
                            builder.Write(TotalPieces.ToString());
                        }
                        if (book_mark.Name == "KGS")
                        {
                            builder.Write("KGS");
                        }
                        #region 导出进仓单PDF
                        #endregion
                    }
                    string NewFilePath = SaveDoc(doc, _SaveFormat);
                    retMsg.retResult = true;
                    retMsg.MsgStr = NewFilePath;
                }
            }
            catch (Exception ex)
            {
                retMsg.retResult = false;
                retMsg.MsgStr = Common.GetExceptionMsg(ex);
            }
            return retMsg;
        }

        /// <summary>
        /// 获取word模板 路径
        /// </summary>
        /// <param name="wordModelPath">模板名称，会自动找模板</param>
        /// <returns></returns>
        private static string GetWordModelPath<T>(T ObjT, string wordModelPath = "") where T : class,new()
        {
            if (string.IsNullOrEmpty(wordModelPath))
            {
                var TypeName = ObjT.GetType().ToString();
                if (TypeName.LastIndexOf('.') >= 0)
                    TypeName = TypeName.Substring(TypeName.LastIndexOf('.') + 1);
                wordModelPath = "/FileModel/WordModel/" + TypeName + ".docx";
                if (!File.Exists(HttpContext.Current.Server.MapPath(wordModelPath)))
                {
                    wordModelPath = "/FileModel/WordModel/" + TypeName + ".doc";
                    if (!File.Exists(HttpContext.Current.Server.MapPath(wordModelPath)))
                    {
                        wordModelPath = "/FileModel/" + TypeName + ".doc";
                        if (!File.Exists(HttpContext.Current.Server.MapPath(wordModelPath)))
                        {
                            wordModelPath = "/FileModel/" + TypeName + ".docx";
                        }
                    }
                }
            }
            else
            {
                string FileModelPath = System.Configuration.ConfigurationManager.AppSettings["FileModelPath"] == null ? "/FileModel/" : System.Configuration.ConfigurationManager.AppSettings["FileModelPath"].ToString();
                if (wordModelPath.IndexOf(FileModelPath) < 0)
                {
                    wordModelPath = "/" + FileModelPath + "/WordModel/" + wordModelPath;
                    if (!File.Exists(HttpContext.Current.Server.MapPath(wordModelPath)))
                    {
                        wordModelPath = "/" + FileModelPath + "/" + wordModelPath;
                    }
                }
            }

            return wordModelPath;
        }

        /// <summary>
        /// 保存word文档
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="_SaveFormat"></param>
        /// <param name="NewFilePath"></param>
        /// <returns>返回新文件路径</returns>
        private static string SaveDoc(Document doc, SaveFormat _SaveFormat = SaveFormat.Doc, string _NewFilePath = "")
        {
            string NewFilePath = _NewFilePath;
            if (string.IsNullOrEmpty(NewFilePath))
            {
                NewFilePath = System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"] == null ? "/FileDownLoad/" : System.Configuration.ConfigurationManager.AppSettings["FileDownLoadPath"].ToString();
                NewFilePath = NewFilePath + "/WordModelOutPut/" + DateTime.Now.ToString("yyyy-MM/dd/");
                if (NewFilePath.IndexOf(":") < 0)
                    NewFilePath = HttpContext.Current.Server.MapPath(NewFilePath);
                //文件夹不存在的话，创建 文件夹路径
                if (!Directory.Exists(NewFilePath))
                    Directory.CreateDirectory(NewFilePath);

                NewFilePath += "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + (new Random()).Next(1, 999).ToString("000");
            }
            NewFilePath += "." + _SaveFormat.ToString().ToLower();
            doc.Save(NewFilePath, _SaveFormat);
            return NewFilePath;
        }

        /// <summary>
        /// 设置word模板的 书签值，生成多张图片
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ObjT"></param>
        /// <param name="wordModelPath"></param>
        /// <returns></returns>
        public static ReturnMsg SetWordModel_BookMarkImgByT<T>(T ObjT, string wordModelPath = "")
            where T : class,new()
        {
            ReturnMsg retMsg = new ReturnMsg(true, "");
            List<Image> ArrWordImg = new List<Image>();
            try
            {
                wordModelPath = GetWordModelPath<T>(ObjT, wordModelPath);
                string wordModel_Path = wordModelPath;
                if (wordModelPath.IndexOf(":") < 0)
                    wordModel_Path = HttpContext.Current.Server.MapPath(wordModelPath);

                if (!File.Exists(wordModel_Path))
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件不存在";
                    return retMsg;
                }
                string FileClass = Common.GetFileClass(wordModel_Path);
                if (FileClass != "208207" && FileClass != "8075")
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件格式不正确";
                    return retMsg;
                }
                Document doc = new Aspose.Words.Document();

                try
                {
                    doc = new Aspose.Words.Document(wordModel_Path);
                }
                catch (Exception ex)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = Common.GetExceptionMsg(ex);
                    return retMsg;
                }

                if (doc == null)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件解析出错";
                    return retMsg;
                }
                if (ObjT == null)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "设置模板文件的数据不能为空";
                    return retMsg;
                }
                BookmarkCollection bookMarks = doc.Range.Bookmarks;
                if (bookMarks.Count <= 0)
                {
                    retMsg.retResult = false;
                    retMsg.MsgStr = "模板文件，不存在任何书签";
                    return retMsg;
                }
                else
                {
                    //使用DocumentBuilder对象插入一些文档对象，如插入书签，插入文本框，插入复选框，插入一个段落，插入空白页，追加或另一个word文件的内容等。
                    var builder = new DocumentBuilder(doc);

                    foreach (var bookmark in bookMarks)
                    {
                        var book_mark = bookmark as Aspose.Words.Bookmark;
                        book_mark.Text = "";// 清掉标示
                        //定位到指定位置进行插入操作
                        builder.MoveToBookmark(book_mark.Name);
                        object objval = Common.GetProtityValue(ObjT, book_mark.Name);
                        if (objval != null)
                        {
                            builder.Write(objval.ToString());
                        }
                    }
                    Stream imgStream = new MemoryStream();

                    #region Aspose.Words 6.5.0.0

                    //Aspose.Words.Rendering.ImageOptions ImageOpts = new Aspose.Words.Rendering.ImageOptions();
                    //ImageOpts.JpegQuality = 100;
                    //if (doc.BuiltInDocumentProperties.Pages > 1)
                    //{
                    //    doc.SaveToImage(0, doc.BuiltInDocumentProperties.Pages, HttpContext.Current.Server.MapPath("/DownLoad") + "/word.tiff", ImageOpts);
                    //    doc.SaveToImage(0, doc.BuiltInDocumentProperties.Pages, imgStream, System.Drawing.Imaging.ImageFormat.Tiff, ImageOpts);
                    //}
                    //else
                    //{
                    //    doc.SaveToImage(0, doc.BuiltInDocumentProperties.Pages, HttpContext.Current.Server.MapPath("/DownLoad") + "/word.tiff", ImageOpts);
                    //    doc.SaveToImage(0, doc.BuiltInDocumentProperties.Pages, imgStream, System.Drawing.Imaging.ImageFormat.Jpeg, ImageOpts);
                    //}

                    //if (imgStream.Length > 0)
                    //{
                    //    Image img = Image.FromStream(imgStream);
                    //    ArrWordImg.Add(img);
                    //}

                    #endregion

                    //for (int pageindex = 0; pageindex < doc.BuiltInDocumentProperties.Pages; pageindex++)
                    //{
                    //    imgStream = null;
                    //    doc.SaveToImage(pageindex, doc.BuiltInDocumentProperties.Pages, imgStream, System.Drawing.Imaging.ImageFormat.Tiff);
                    //    if (imgStream != null)
                    //    {
                    //        if (imgStream.Length > 0)
                    //        {
                    //            Image img = Image.FromStream(imgStream);
                    //            ArrWordImg.Add(img);
                    //        }
                    //    }
                    //}
                    #region Aspose.Words 15.2.0.0

                    Aspose.Words.Saving.ImageSaveOptions iso = new Aspose.Words.Saving.ImageSaveOptions(SaveFormat.Jpeg);
                    iso.PrettyFormat = true;
                    iso.UseAntiAliasing = true;

                    for (int pageindex = 0; pageindex < doc.PageCount; pageindex++)
                    {
                        iso.PageIndex = pageindex;
                        doc.Save(imgStream, iso);
                        if (imgStream != null)
                        {
                            if (imgStream.Length > 0)
                            {
                                Image img = Image.FromStream(imgStream);
                                ArrWordImg.Add(img);
                            }
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                var ErrMsg = Common.GetExceptionMsg(ex);
                retMsg.ArrWordImg = null;
                retMsg.retResult = false;
                retMsg.MsgStr = ErrMsg;
                return retMsg;
            }
            retMsg.ArrWordImg = ArrWordImg;
            return retMsg;
        }

        /// <summary>
        /// 标签插入Table
        /// </summary>
        /// <typeparam name="T">Model类</typeparam>
        /// <param name="bookMark">书签</param>
        /// <param name="doc">word文档</param>
        /// <param name="ArrValue">要插入的 List数据</param>
        public static void WriteTableByList<T>(Bookmark bookMark, Document doc, List<T> ArrValue, string wordModelPath = "")
            where T : class,new()
        {
            bookMark.Text = "";
            var builder = new DocumentBuilder(doc);
            builder.MoveToBookmark(bookMark.Name);

            int rowCount = ArrValue.Count();
            var ArrFields = Common.GetAllFieldNameByModel<T>();
            int columnCount = ArrFields.Count;

            builder.MoveToBookmark(bookMark.Name);
            builder.StartTable();//开始画Table              
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center; // RowAlignment.Center;                  

            string str = string.Empty;
            double MaxWidth = 50;//列最大宽度

            //列宽
            Dictionary<string, float> dictHeaherWidth = new Dictionary<string, float>();
            //用于测量文字宽度
            Image img = Image.FromFile(HttpContext.Current.Server.MapPath("/Images/file.png"));
            Graphics g = Graphics.FromImage(img);

            System.Drawing.Font font = new System.Drawing.Font(new FontFamily("宋体"), 8, FontStyle.Regular);

            #region 飞力达空运放货凭证
            //builder.RowFormat.Height = 20;
            if (wordModelPath == "ExportBilOfLadinPDF")
            {
                int ivalue = 0;

                double TotalPieces = 0;
                double TotalWeight = 0;
                double TotalVolume = 0;
                foreach (var value in ArrValue)
                {
                    ivalue = ivalue + 1;

                    foreach (var item in ArrFields)
                    {
                        if (item == null)
                            continue;
                        dynamic dmic = item as dynamic;
                        var name = dmic.Name;

                        //var valStr = Common.GetProtityValue(item, dmic.Name);
                        //valStr = valStr == null ? "" : valStr.ToString().Replace("\\r\\n", "\n");
                        var valStr = "";
                        builder.InsertCell();
                        if (name == "Entry_Id")
                        {
                            valStr = Common.GetProtityValue(value, dmic.Name);
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "进仓编号", dictHeaherWidth, g, ivalue);
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Entry_Id_Data", MaxWidth, 8, valStr == null ? "" : valStr, dictHeaherWidth, g, ivalue);//表头 - 进仓编号
                            builder.CellFormat.HorizontalMerge = CellMerge.First;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                        }
                        else if (name == "In_Date")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "进仓时间", dictHeaherWidth, g, ivalue);
                        }
                        else if (name == "Packing")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "包装情况", dictHeaherWidth, g, ivalue);
                        }
                        else if (name == "DamageCondition")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "货损情况", dictHeaherWidth, g, ivalue);
                        }
                        else if (name == "Closure_Remark")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "随附文件", dictHeaherWidth, g, ivalue);
                            builder.EndRow();
                        }
                        else if (name == "Consign_Code_CK")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "委托方", dictHeaherWidth, g, ivalue);
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.First;

                            valStr = Common.GetProtityValue(value, dmic.Name);
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Consign_Code_CK_data", MaxWidth, 8, valStr == null ? "" : valStr, dictHeaherWidth, g, ivalue);//表头 - 委托方
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                            valStr = Common.GetProtityValue(value, "In_Date") == null ? "" : Common.GetProtityValue(value, "In_Date").ToString();
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "In_Date_Data", MaxWidth, 8, valStr, dictHeaherWidth, g, ivalue);//进仓时间

                            valStr = Common.GetProtityValue(value, "Packing") == null ? "" : Common.GetProtityValue(value, "Packing").ToString();
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Packing_data", MaxWidth, 8, valStr, dictHeaherWidth, g, ivalue);//包装情况

                            valStr = Common.GetProtityValue(value, "DamageCondition") == null ? null : Common.GetProtityValue(value, "DamageCondition").ToString();
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "DamageCondition_data", MaxWidth, 8, valStr, dictHeaherWidth, g, ivalue);//货损情况

                            valStr = Common.GetProtityValue(value, "Closure_Remark") == null ? "" : Common.GetProtityValue(value, "Closure_Remark").ToString();
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Closure_Remark_data", MaxWidth, 8, valStr, dictHeaherWidth, g, ivalue);//随附文件
                            builder.EndRow();

                        }
                        else if (name == "Pieces_CK")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "件数", dictHeaherWidth, g, ivalue);
                        }
                        else if (name == "Weight_CK")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "毛重", dictHeaherWidth, g, ivalue);
                        }
                        else if (name == "Volume_CK")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "体积", dictHeaherWidth, g, ivalue);
                        }
                        else if (name == "Size")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "尺寸", dictHeaherWidth, g, ivalue);
                            builder.CellFormat.HorizontalMerge = CellMerge.First;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                        }
                        else if (name == "Remark")
                        {
                            setCell(builder, font, dmic.MetaDataTypeName, dmic.Name, MaxWidth, 8, "备注", dictHeaherWidth, g, ivalue);
                            builder.CellFormat.HorizontalMerge = CellMerge.First;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                            builder.EndRow();

                            valStr = Common.GetProtityValue(value, "Pieces_CK") == null ? "" : Common.GetProtityValue(value, "Pieces_CK").ToString();
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Pieces_CK_data", MaxWidth, 8, valStr, dictHeaherWidth, g, ivalue);
                            if (valStr != null && valStr != "")
                            {//统计件数
                                TotalPieces += double.Parse(valStr);
                            }

                            valStr = Common.GetProtityValue(value, "Weight_CK") == null ? "" : Common.GetProtityValue(value, "Weight_CK").ToString();
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Weight_CK_data", MaxWidth, 8, valStr, dictHeaherWidth, g, ivalue);
                            if (valStr != null && valStr != "")
                            {//统计毛重
                                TotalWeight += double.Parse(valStr);
                            }

                            valStr = Common.GetProtityValue(value, "Volume_CK") == null ? "" : Common.GetProtityValue(value, "Volume_CK").ToString();
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Volume_CK_data", MaxWidth, 8, valStr, dictHeaherWidth, g, ivalue);
                            if (valStr != null && valStr != "")
                            {//统计体积
                                TotalVolume += double.Parse(valStr);
                            }

                            valStr = Common.GetProtityValue(value, "Size") == null ? "" : Common.GetProtityValue(value, "Size").ToString();
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Size_data", MaxWidth, 8, valStr, dictHeaherWidth, g, ivalue);
                            builder.CellFormat.HorizontalMerge = CellMerge.First;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                            valStr = Common.GetProtityValue(value, dmic.Name);
                            builder.InsertCell();
                            setCell(builder, font, dmic.MetaDataTypeName, "Remark_data", MaxWidth, 8, valStr == null ? "" : valStr, dictHeaherWidth, g, ivalue);
                            builder.CellFormat.HorizontalMerge = CellMerge.First;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                            builder.EndRow();

                            //插入一条空行
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.First;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                            builder.EndRow();
                        }

                    }
                }


                builder.InsertCell();
                setCell(builder, font, "", "TotalPieces", MaxWidth, 8, "合计件数", dictHeaherWidth, g, ivalue);
                builder.InsertCell();
                setCell(builder, font, "", "TotalPiecesNum", MaxWidth, 8, TotalPieces.ToString(), dictHeaherWidth, g, ivalue);

                builder.InsertCell();
                setCell(builder, font, "", "TotalWeight", MaxWidth, 8, "合计毛重", dictHeaherWidth, g, ivalue);
                builder.InsertCell();
                setCell(builder, font, "", "TotalWeightNum", MaxWidth, 8, TotalWeight.ToString(), dictHeaherWidth, g, ivalue);
                builder.CellFormat.HorizontalMerge = CellMerge.First;
                builder.InsertCell();
                builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                builder.InsertCell();
                setCell(builder, font, "", "TotalVolume", MaxWidth, 8, "合计体积", dictHeaherWidth, g, ivalue);
                builder.InsertCell();
                setCell(builder, font, "", "TotalVolumeNum", MaxWidth, 8, TotalVolume.ToString(), dictHeaherWidth, g, ivalue);
                builder.CellFormat.HorizontalMerge = CellMerge.First;
                builder.InsertCell();
                builder.CellFormat.HorizontalMerge = CellMerge.Previous;

                builder.EndTable();

                return;
            }
            #endregion 

            #region 添加列头
            foreach (var item in ArrFields)
            {
                
                if (item == null)
                    continue;
                dynamic dmic = item as dynamic;
                var name = dmic.Name;
                //if (name == "Pieces") 
                //{
                //    ArrFields.Remove(item);
                //}
                var cellwidth = 0;
                if (name == "MBL" || name == "End_PortEng" || name == "Depart_PortEng" || name == "Flight_No" || name == "FWD_Code" || name == "Pieces")
                {
                    continue;
                }
                var displayname = "";
                if (name == "HBL")
                {
                    cellwidth = 70;
                    displayname = "HAWB NO.";
                }
                //else if (name == "Pieces")
                //{
                //    //cellwidth = 107;
                //    //displayname = "NO.OF PKG.";
                //    continue;
                //}
                else if (name == "Piecestest")
                {
                    cellwidth = 50;              
                    displayname = "NO.OF PKG.";
                }
                else if (name == "Weight")
                {
                    cellwidth = 50;
                    displayname = "Gross Weight";
                }
                else if (name == "EN_Name_H")
                {
                    cellwidth = 90;
                    displayname = "NATURE OF GOODS";
                }
                else if (name == "End_Port")
                {
                    cellwidth = 50;
                    displayname = "FINAL DEST";
                }
                else if (name == "Shipper_H")
                {
                    cellwidth = 134;
                    displayname = "NAME &ADDRESS OF " + "\n" + "SHIPPER";
                }
                else if (name == "Consignee_H")
                {
                    cellwidth = 121;
                    displayname = "NAME & ADDRESS OF " + "\n" + "CONSIGNEE";
                }
                else if (name == "Pay_Mode_H")
                {
                    cellwidth = 40;;
                    displayname = "RE";
                }
                builder.InsertCell();
                //Table单元格边框线样式  
                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                SizeF TextSize = g.MeasureString(dmic.MetaDataTypeName, font, (int)MaxWidth / 8);

                //Table此单元格宽度  
                builder.CellFormat.Width = cellwidth;//TextSize.Width / 8;
                //此单元格中内容垂直对齐方式  
                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                //字体大小  
                builder.Font.Size = font.Size;
                //是否加粗  
                builder.Bold = font.Style.Equals(FontStyle.Bold);
                //向此单元格中添加内容  
                builder.Write(displayname);
                dictHeaherWidth.Add(dmic.Name, TextSize.Width / 8);
            }
            builder.EndRow();
            #endregion

            #region 添加每行数据
            for (int i = 0; i < rowCount; i++)
            {
                var j = 0;
                foreach (var dmicitem in ArrFields)
                {
                    if (dmicitem == null)
                        continue;
                    dynamic dmic = dmicitem as dynamic;
                    var name = dmic.Name;
                    if (name == "MBL" || name == "End_PortEng" || name == "Depart_PortEng" || name == "Flight_No" || name == "FWD_Code" || name == "Pieces")
                    {
                        continue;
                    }
                    var valStr = Common.GetProtityValue(ArrValue[i], dmic.Name);

                    str = valStr == null ? "" : valStr.ToString().Replace("\\r\\n", "\n");

                    //插入Table单元格  
                    builder.InsertCell();
                    //Table单元格边框线样式  
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //SizeF TextSize = g.MeasureString(dmic.Name, font, (int)MaxWidth);
                    float textwidth = 10;
                    var rowheight = 70;
                    var Where = dictHeaherWidth.Where(x => x.Key == dmic.Name);
                    if (Where.Any())
                    {
                        textwidth = Where.FirstOrDefault().Value;
                    }

                    //Table此单元格宽度 跟随列头宽度                     
                    builder.CellFormat.Width = textwidth;
                    if (name == "HBL" || name == "Piecestest" || name == "Weight" || name == "EN_Name_H" || name == "End_Port" || name == "Shipper_H" || name == "Consignee_H" || name == "Pay_Mode_H") 
                    {
                        builder.RowFormat.Height = rowheight;
                    }
                    //此单元格中内容垂直对齐方式  
                    builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                    builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                    builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                    //字体大小  
                    builder.Font.Size = font.Size;
                    //是否加粗  
                    builder.Bold = font.Style.Equals(FontStyle.Bold);
                    //向此单元格中添加内容  
                    builder.Write(str);
                    j++;
                }
                //Table行结束  
                builder.EndRow();
            }
            #endregion
            builder.EndTable();
        }

        private static void setCell(DocumentBuilder builder, System.Drawing.Font font, string MetaDataTypeName, string name
            , double MaxWidth, double cellwidth, string displayname, Dictionary<string, float> dictHeaherWidth, Graphics g, int i)
        {
            //Table单元格边框线样式  
            builder.CellFormat.Borders.LineStyle = LineStyle.Single;
            SizeF TextSize = g.MeasureString(MetaDataTypeName, font, (int)MaxWidth / 8);

            //Table此单元格宽度  
            builder.CellFormat.Width = TextSize.Width / 8;
            //此单元格中内容垂直对齐方式  
            builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
            builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
            builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
            //字体大小  
            builder.Font.Size = font.Size;
            //是否加粗  
            builder.Bold = font.Style.Equals(FontStyle.Bold);
            //向此单元格中添加内容  
            builder.Write(displayname);
            name = name + i.ToString();
            dictHeaherWidth.Add(name, TextSize.Width / 8);
        }

        /// <summary>
        /// 标签插入Table
        /// </summary>
        /// <typeparam name="T">Model类</typeparam>
        /// <param name="bookMark">书签</param>
        /// <param name="doc">word文档</param>
        /// <param name="ArrValue">要插入的 List数据</param>
        public static void WriteTableByList(Bookmark bookMark, Document doc, DataTable dt)
        {
            bookMark.Text = "";
            var builder = new DocumentBuilder(doc);
            builder.MoveToBookmark(bookMark.Name);

            int rowCount = dt.Rows.Count;
            int columnCount = dt.Columns.Count;

            builder.MoveToBookmark(bookMark.Name);
            builder.StartTable();//开始画Table              
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center; // RowAlignment.Center;                  

            string str = string.Empty;

            builder.RowFormat.Height = 20;

            //添加列头  
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                builder.InsertCell();
                //Table单元格边框线样式  
                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                //Table此单元格宽度  
                builder.CellFormat.Width = 600;
                //此单元格中内容垂直对齐方式  
                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                //字体大小  
                builder.Font.Size = 10;
                //是否加粗  
                builder.Bold = true;
                //向此单元格中添加内容  
                builder.Write(dt.Columns[i].ColumnName);
            }
            builder.EndRow();

            //添加每行数据  
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    str = dt.Rows[i][j].ToString();

                    //插入Table单元格  
                    builder.InsertCell();

                    //Table单元格边框线样式  
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;

                    //Table此单元格宽度 跟随列头宽度  
                    //builder.CellFormat.Width = 500;  

                    //此单元格中内容垂直对齐方式  
                    builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                    builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                    builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;

                    //字体大小  
                    builder.Font.Size = 10;

                    //是否加粗  
                    builder.Bold = false;

                    //向此单元格中添加内容  
                    builder.Write(str);

                    j++;
                }
                //Table行结束  
                builder.EndRow();
            }
            builder.EndTable();
        }

        public class ReturnMsg
        {
            public ReturnMsg()
            {
            }

            public ReturnMsg(bool _retResult, string _ErrMsg)
            {
                retResult = _retResult;
                MsgStr = _ErrMsg;
            }

            /// <summary>
            /// 返回 成功/失败
            /// </summary>
            public bool retResult { get; set; }

            /// <summary>
            /// 错误/成功信息，成功时是 路径
            /// </summary>
            public string MsgStr { get; set; }

            /// <summary>
            /// 输出图片时，使用
            /// </summary>
            public List<Image> ArrWordImg { get; set; }
        }
    }
}