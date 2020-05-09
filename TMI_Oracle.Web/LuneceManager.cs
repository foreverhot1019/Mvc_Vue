using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using TMI.Web.Models;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PanGu;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.QueryParsers;
using System.Diagnostics;

namespace TMI.Web
{
    public class LuneceManager
    {
        public class IndexManager
        {
            public static readonly IndexManager OIndexManager = new IndexManager();
            //索引文档保存位置   
            public static readonly string indexPath = HttpContext.Current.Server.MapPath("~/Lucene.Net/IndexData");
            //当前程序集
            private static System.Reflection.Assembly Assembly = TMI.Web.Extensions.Common.Assembly;
            //索引类的 公共属性
            private static PropertyInfo[] LuceneModelProtyInfos = typeof(LuneceModel).GetProperties();

            /// <summary>
            /// 开启全文检索
            /// </summary>
            private static bool OpenLuncene
            {
                get
                {
                    bool OpenLuncene = false;
                    string OpenLunceneStr = System.Configuration.ConfigurationManager.AppSettings["OpenLuncene"] ?? "";
                    if (!string.IsNullOrEmpty(OpenLunceneStr))
                    {
                        if (OpenLunceneStr.Trim().ToLower() == "true")
                            OpenLuncene = true;
                    }
                    return OpenLuncene;
                }
            }

            //自动设置值
            public static List<string> SetLuceneAutoNames = new List<string>() { 
                "ADDWHO",
                "ADDTS",
                "EDITWHO",
                "EDITTS",
                "OperatingPoint"
            };
            //设置全文检索的表 
            public static List<string> AutoLunceneTable = new List<string> { 
                "VIP_CDARETURNXML",
                "VIPRETURNMQ",
                "CDARETURNMQ",
                "CQZGRETURNMQ",
                "CQZGDECRETURNMQ",
                "CQZGSHRETURNMQ",
                "TMLOCKJSON",
                "MQAUTOSENDXML",
                "MESSAGE"
            };

            /// <summary>
            /// 错误日志 文件夹名称
            /// </summary>
            private string LuneceErrorDir
            {
                get
                {
                    var LEDir = System.Configuration.ConfigurationManager.AppSettings["LuneceErrorDir"] ?? "LuneceErrorDir";
                    return LEDir;
                }
            }

            private IndexManager()
            {
            }

            //请求队列 解决索引目录同时操作的并发问题
            private Queue<LuneceModel> LuneceModelQueue = new Queue<LuneceModel>();

            /// <summary>
            /// 加入队列
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="_entityProptys"></param>
            /// <param name="OLuneceIndexType"></param>
            private void EnqueueLuceneModel(object entity, PropertyInfo[] _entityProptys, LuneceIndexType OLuneceIndexType)
            {
                if (!OpenLuncene)
                    return;
                if (!AutoLunceneTable.Any(n => entity.GetType().ToString().ToUpper().EndsWith(n)))
                    return;

                LuneceModel OLuneceModel = new LuneceModel();
                try
                {
                    OLuneceModel.TableName = entity.GetType().ToString();
                    OLuneceModel.TableKey = GetKeyValue(entity, _entityProptys);
                    OLuneceModel.IndexType = OLuneceIndexType;
                    OLuneceModel.Content = GetStrinValue(entity, _entityProptys);

                    var WhereLuceneModelProtyInfos = LuceneModelProtyInfos.Where(x => SetLuceneAutoNames.Any(n => n == x.Name.ToUpper()));
                    foreach (var item in WhereLuceneModelProtyInfos)
                    {
                        var Where_entityProptys = _entityProptys.Where(x => x.Name.ToUpper() == item.Name);
                        if (Where_entityProptys.Any())
                        {
                            var objValue = Where_entityProptys.FirstOrDefault().GetValue(entity);
                            if (objValue != null)
                            {
                                item.SetValue(OLuneceModel, objValue);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    #region 去除 序列化Json无限循环回路

                    //去除Json无限循环回路
                    Newtonsoft.Json.JsonSerializerSettings OJsonSrlizerSettingsNoLoop = new Newtonsoft.Json.JsonSerializerSettings();

                    //循环引用时 忽略
                    OJsonSrlizerSettingsNoLoop.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    ////定义序列化Json的行为，使其忽略引用对象（导航属性,主键 会被转义-$主键）
                    //OJsonSrlizerSettingsNoLoop.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

                    #endregion

                    string ErrMsg = TMI.Web.Extensions.Common.GetExceptionMsg(ex);
                    if (!(string.IsNullOrEmpty(OLuneceModel.TableName) || string.IsNullOrEmpty(OLuneceModel.TableKey) || string.IsNullOrEmpty(OLuneceModel.Content)))
                        SQLDALHelper.WriteLogHelper.WriteLog(ErrMsg + "-" + Newtonsoft.Json.JsonConvert.SerializeObject(OLuneceModel, OJsonSrlizerSettingsNoLoop), LuneceErrorDir, true);
                    return;
                }
                if (!(string.IsNullOrEmpty(OLuneceModel.TableName) || string.IsNullOrEmpty(OLuneceModel.TableKey) || string.IsNullOrEmpty(OLuneceModel.Content)))
                    LuneceModelQueue.Enqueue(OLuneceModel);
            }

            /// <summary>
            /// 添加索引请求至队列
            /// </summary>
            /// <param name="books"></param>
            public void LuneceInsert(object entity, PropertyInfo[] _entityProptys)
            {
                EnqueueLuceneModel(entity, _entityProptys, LuneceIndexType.Insert);
            }

            /// <summary>
            /// 删除索引请求至队列
            /// </summary>
            /// <param name="bid"></param>
            public void LuneceDelete(object entity, PropertyInfo[] _entityProptys)
            {
                EnqueueLuceneModel(entity, _entityProptys, LuneceIndexType.Delete);
            }

            /// <summary>
            /// 添加修改索引(实质上是先删除原有索引 再新增修改后索引)请求至队列
            /// </summary>
            /// <param name="books"></param>
            public void LuneceModify(object entity, PropertyInfo[] _entityProptys)
            {
                EnqueueLuceneModel(entity, _entityProptys, LuneceIndexType.Modify);
            }

            public void StartNewThread()
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(QueueToIndex));
            }

            //定义一个线程 将队列中的数据取出来 插入索引库中
            private void QueueToIndex(object para)
            {
                while (true)
                {
                    if (LuneceModelQueue.Count > 0)
                    {
                        try
                        {
                            UpdateLunceneIndex();
                        }
                        catch (Exception ex)
                        {
                            SQLDALHelper.WriteLogHelper.WriteLog("全文检索错误(线程)：" + TMI.Web.Extensions.Common.GetExceptionMsg(ex), LuneceErrorDir, true);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }

            /// <summary>
            /// 获取索引添加器
            /// </summary>
            /// <returns></returns>
            public IndexWriter GetIndexWriter(FSDirectory directory)
            {
                //IndexReader:对索引库进行读取的类
                bool isExist = IndexReader.IndexExists(directory); //是否存在索引库文件夹以及索引库特征文件
                if (isExist)
                {
                    //如果索引目录被锁定（比如索引过程中程序异常退出或另一进程在操作索引库），则解锁
                    //Q:存在问题 如果一个用户正在对索引库写操作 此时是上锁的 而另一个用户过来操作时 将锁解开了 于是产生冲突 --解决方法后续
                    if (IndexWriter.IsLocked(directory))
                    {
                        IndexWriter.Unlock(directory);
                    }
                }

                //创建向索引库写操作对象  IndexWriter(索引目录,指定使用盘古分词进行切词,最大写入长度限制)
                //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
                IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);

                return writer;
            }

            /// <summary>
            /// 添加索引
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="OLuneceModel"></param>
            private void IndexAddDocument(IndexWriter writer, LuneceModel OLuneceModel)
            {
                if (OLuneceModel.IndexType == LuneceIndexType.Modify)
                {
                    //先删除 再新增
                    IndexDeleteDocuments(writer, OLuneceModel);
                }
                Document document = new Document();
                try
                {
                    //Field.Store:表示是否保存字段原值。指定Field.Store.YES的字段在检索时才能用document.Get取出原值  
                    //Field.Index.NOT_ANALYZED:指定不按照分词后的结果保存--是否按分词后结果保存取决于是否对该列内容进行模糊查询
                    document.Add(new Field("TableName", OLuneceModel.TableName ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("TableKey", OLuneceModel.TableKey ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
                    //Field.Index.ANALYZED:指定文章内容按照分词后结果保存 否则无法实现后续的模糊查询 
                    //WITH_POSITIONS_OFFSETS:指示不仅保存分割后的词 还保存词之间的距离
                    document.Add(new Field("Content", OLuneceModel.Content ?? "", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                    document.Add(new Field("ADDWHO", OLuneceModel.ADDWHO ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("ADDTS", (OLuneceModel.ADDTS == null ? "" : Convert.ToDateTime(OLuneceModel.ADDTS).ToString("yyyy-MM-dd")), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("EDITWHO", OLuneceModel.EDITWHO ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("EDITTS", (OLuneceModel.EDITTS == null ? "" : Convert.ToDateTime(OLuneceModel.EDITTS).ToString("yyyy-MM-dd")), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("OperatingPoint", OLuneceModel.OperatingPoint <= 0 ? "" : OLuneceModel.OperatingPoint.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    writer.AddDocument(document);
                }
                catch (Exception ex)
                {
                    SQLDALHelper.WriteLogHelper.WriteLog("创建索引出错：" + TMI.Web.Extensions.Common.GetExceptionMsg(ex), LuneceErrorDir, true);
                    writer.Close();
                    Thread.Sleep(10);
                    FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
                    writer = GetIndexWriter(directory);
                }
            }

            /// <summary>
            /// 删除索引
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="OLuneceModel"></param>
            private void IndexDeleteDocuments(IndexWriter writer, LuneceModel OLuneceModel)
            {
                writer.DeleteDocuments(new Term[]{ 
                            new Term("TableName", OLuneceModel.TableName), 
                            new Term("TableKey", OLuneceModel.TableKey),
                            new Term("OperatingPoint", OLuneceModel.OperatingPoint.ToString())
                        });
            }

            /// <summary>
            /// 更新索引库操作
            /// </summary>
            private void UpdateLunceneIndex()
            {
                FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
                //创建向索引库写操作对象  IndexWriter(索引目录,指定使用盘古分词进行切词,最大写入长度限制)
                //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
                IndexWriter writer = GetIndexWriter(directory);

                while (LuneceModelQueue.Count > 0)
                {
                    LuneceModel OLuneceModel = LuneceModelQueue.Dequeue();
                    if (OLuneceModel.IndexType == LuneceIndexType.Delete)
                    {
                        IndexDeleteDocuments(writer, OLuneceModel);
                    }
                    else
                    {
                        IndexAddDocument(writer, OLuneceModel);
                    }
                }

                writer.Close();
                directory.Close();
            }

            #region 反射获取索引所需数据集

            //合并string格式全文检索 最小长度
            private int StringMaxLength = 51;

            /// <summary>
            /// 获取
            /// </summary>
            /// <returns></returns>
            private string GetKeyValue(object entity, PropertyInfo[] _entityProptys)
            {
                string KeyVal = "";
                foreach (var propinfo in _entityProptys)
                {
                    var ArrKeyAttr = propinfo.GetCustomAttributes(typeof(KeyAttribute), false);
                    if (ArrKeyAttr.Any())
                    {
                        KeyVal = propinfo.GetValue(entity).ToString();
                        break;
                    }
                }
                if (string.IsNullOrEmpty(KeyVal))
                {
                    var KeyProptyS = _entityProptys.Where(x => x.Name.ToLower() == "ID");
                    if (KeyProptyS.Any())
                    {
                        KeyVal = KeyProptyS.FirstOrDefault().GetValue(entity).ToString();
                    }
                }
                return KeyVal;
            }

            /// <summary>
            /// 获取对象所有string字符串
            /// 以，连接
            /// </summary>
            /// <returns></returns>
            private string GetStrinValue(object entity, PropertyInfo[] _entityProptys)
            {
                string StringVal = "";
                foreach (var propinfo in _entityProptys)
                {
                    var propInfoType = propinfo.PropertyType;
                    if (propInfoType == typeof(string) || propInfoType == typeof(String))
                    {
                        if (SetLuceneAutoNames.Any(x => x == propinfo.Name.ToUpper()))
                        {
                            continue;
                        }
                        var ArrMax_StrLenAttr = propinfo.GetCustomAttributes(typeof(StringLengthAttribute), false).Union(propinfo.GetCustomAttributes(typeof(MaxLengthAttribute), false));

                        if (ArrMax_StrLenAttr.Any())
                        {
                            if (ArrMax_StrLenAttr.Any())
                            {
                                var ObjStrLenAttr = ArrMax_StrLenAttr.FirstOrDefault();
                                var OStrLenAttrPropertyS = ObjStrLenAttr.GetType().GetProperties();
                                var WhereOStrLenAttrPropertyS = OStrLenAttrPropertyS.Where(x => x.Name == "MaximumLength" || x.Name == "Length");
                                if (WhereOStrLenAttrPropertyS.Any())
                                {
                                    var _MaximumLength = WhereOStrLenAttrPropertyS.FirstOrDefault().GetValue(ObjStrLenAttr);
                                    if (_MaximumLength != null)
                                    {
                                        int MaximumLength = 0;
                                        int.TryParse(_MaximumLength.ToString(), out MaximumLength);
                                        if (MaximumLength > StringMaxLength)
                                        {
                                            var objVal = propinfo.GetValue(entity);
                                            if (objVal != null)
                                                StringVal += (objVal ?? "") + "，";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var objVal = propinfo.GetValue(entity);
                            if (objVal != null)
                                StringVal += (objVal ?? "") + "，";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(StringVal))
                {
                    StringVal = StringVal.Substring(0, StringVal.Length - 1);
                }
                return StringVal;
            }

            #endregion

            /// <summary>
            /// 创建索引
            /// </summary>
            public void CreateIndex(LuneceModel OLuneceModel)
            {
                FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
                //创建向索引库写操作对象  IndexWriter(索引目录,指定使用盘古分词进行切词,最大写入长度限制)
                //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
                IndexWriter writer = GetIndexWriter(directory);
                IndexAddDocument(writer, OLuneceModel);
                writer.Close();//会自动解锁
                directory.Close(); //不要忘了Close，否则索引结果搜不到
            }

            /// <summary>
            /// 创建索引
            /// </summary>
            public void CreateIndex(List<LuneceModel> ArrLuneceModel)
            {
                FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
                //创建向索引库写操作对象  IndexWriter(索引目录,指定使用盘古分词进行切词,最大写入长度限制)
                //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
                IndexWriter writer = GetIndexWriter(directory);

                foreach (var OLuneceModel in ArrLuneceModel)
                {
                    IndexAddDocument(writer, OLuneceModel);
                }

                writer.Close();//会自动解锁
                directory.Close(); //不要忘了Close，否则索引结果搜不到
            }

            /// <summary>
            /// 从索引库中检索关键字
            /// </summary>
            public List<LuneceModel> SearchFromIndexData(string SearchKey, int PageSize = 10, int PageIndex = 1, string TableName = "", string TableKey = "", string OperatingPoint = "")
            {
                FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
                IndexReader reader = IndexReader.Open(directory, true);
                IndexSearcher searcher = new IndexSearcher(reader);
                //搜索条件
                BooleanQuery bQuery = new BooleanQuery();
                //1．MUST和MUST：取得连个查询子句的交集。 
                //2．MUST和MUST_NOT：表示查询结果中不能包含MUST_NOT所对应得查询子句的检索结果。 
                //3．SHOULD与MUST_NOT：连用时，功能同MUST和MUST_NOT。 
                //4．SHOULD与MUST连用时，结果为MUST子句的检索结果,但是SHOULD可影响排序。 
                //5．SHOULD与SHOULD：表示“或”关系，最终检索结果为所有检索子句的并集。 
                //6．MUST_NOT和MUST_NOT：无意义，检索无结果。 
                if (!string.IsNullOrEmpty(SearchKey))
                {
                    #region 方法1

                    ////只能相同的字段
                    //PhraseQuery _query = new PhraseQuery();
                    //string[] ArrSplit_Words = SplitContent.SplitWords(SearchKey);
                    ////把用户输入的关键字进行分词
                    //foreach (string word in ArrSplit_Words)
                    //{
                    //    _query.Add(new Term("Content", word));
                    //}
                    //if (ArrSplit_Words.Any())
                    //    bQuery.Add(_query, BooleanClause.Occur.MUST);
                    //query.SetSlop(100); //指定关键词相隔最大距离

                    #endregion

                    #region 方法2

                    QueryParser parse = new QueryParser("Content", new PanGuAnalyzer());
                    Query _query = parse.Parse(SearchKey);
                    //_query.SetBoost();//设置搜索权重比
                    parse.SetDefaultOperator(QueryParser.Operator.AND);
                    bQuery.Add(_query, BooleanClause.Occur.MUST);

                    #endregion
                }

                if (!string.IsNullOrEmpty(TableName))
                {
                    Term term = new Term("TableName", TableName);
                    Query _query = new TermQuery(term);

                    #region 分词 搜索

                    //QueryParser parse = new QueryParser("TableName", new PanGuAnalyzer());
                    //Query _query = parse.Parse(TableName);
                    //parse.SetDefaultOperator(QueryParser.Operator.AND);

                    #endregion

                    bQuery.Add(_query, BooleanClause.Occur.MUST);
                    //query.Add(new Term("TableName", TableName));
                }
                if (!string.IsNullOrEmpty(TableKey))
                {
                    Term term = new Term("TableKey", TableKey);
                    Query _query = new TermQuery(term);

                    #region 分词 搜索

                    //QueryParser parse = new QueryParser("TableKey", new PanGuAnalyzer());
                    //Query _query = parse.Parse(TableKey);
                    //parse.SetDefaultOperator(QueryParser.Operator.AND);

                    #endregion

                    bQuery.Add(_query, BooleanClause.Occur.MUST);
                    //query.Add(new Term("TableKey", TableKey));
                }
                if (!string.IsNullOrEmpty(OperatingPoint))
                {
                    Term term = new Term("OperatingPoint", OperatingPoint);
                    Query _query = new TermQuery(term);

                    #region 分词 搜索

                    //QueryParser parse = new QueryParser("OperatingPoint", new PanGuAnalyzer());
                    //Query _query = parse.Parse(OperatingPoint);
                    //parse.SetDefaultOperator(QueryParser.Operator.AND);

                    #endregion

                    bQuery.Add(_query, BooleanClause.Occur.MUST);
                    //query.Add(new Term("OperatingPoint", OperatingPoint));
                }

                ScoreDoc[] docs = null;//搜索结果

                #region Lucene搜索

                long lSearchTime = 0;//搜索耗时
                Stopwatch stopwatch = Stopwatch.StartNew();
                var total = 0;
                var skip = PageSize * (PageIndex - 1);
                var take = 0;

                #region 方法1

                //TopScoreDocCollector盛放查询结果的容器
                TopScoreDocCollector collector = TopScoreDocCollector.create(PageSize, true);
                searcher.Search(bQuery, collector);//根据query查询条件进行查询，查询结果放入collector容器
                lSearchTime = stopwatch.ElapsedMilliseconds;
                total = collector.GetTotalHits();
                skip = PageSize * (PageIndex - 1);
                take = (total - skip) > PageSize ? PageSize : (total - skip);
                //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
                docs = collector.TopDocs(skip, take).scoreDocs;

                #endregion

                #region 方法2

                ////SortField构造函数第三个字段true为降序,false为升序
                //Sort sort = new Sort(new SortField("TableKey", SortField.DOC, true));
                //TopDocs _docs = searcher.Search(bQuery, (Filter)null, PageSize * PageIndex, sort);
                //stopwatch.Stop();
                //total = _docs.totalHits;
                //skip = PageSize * (PageIndex - 1);
                //take = (total - skip) > PageSize ? PageSize : (total - skip);
                //if (_docs != null && total > 0)
                //{
                //    lSearchTime = stopwatch.ElapsedMilliseconds;
                //    docs = _docs.scoreDocs;
                //}
                //else
                //    docs = new ScoreDoc[] { };

                #endregion

                #endregion

                //展示数据实体对象集合
                List<LuneceModel> ArrOLuneceModel = new List<LuneceModel>();
                for (int i = 0; i < docs.Length; i++)
                {
                    if (i < take)
                    {
                        int docId = docs[i].doc;//得到查询结果文档的id（Lucene内部分配的id）
                        Document doc = searcher.Doc(docId);//根据文档id来获得文档对象Document

                        LuneceModel OLuneceModel = new LuneceModel();
                        OLuneceModel.TableName = doc.Get("TableName");
                        OLuneceModel.TableKey = doc.Get("TableKey");
                        //搜索关键字高亮显示 使用盘古提供高亮插件
                        OLuneceModel.Content = SplitContent.HightLight(SearchKey, doc.Get("Content"));
                        OLuneceModel.ADDWHO = doc.Get("ADDWHO");
                        OLuneceModel.ADDTS = string.IsNullOrEmpty(doc.Get("ADDTS")) ? null : (DateTime?)Convert.ToDateTime(doc.Get("ADDTS"));
                        OLuneceModel.EDITTS = string.IsNullOrEmpty(doc.Get("EDITTS")) ? null : (DateTime?)Convert.ToDateTime(doc.Get("EDITTS"));
                        OLuneceModel.EDITWHO = doc.Get("EDITWHO");
                        OLuneceModel.OperatingPoint = string.IsNullOrEmpty(doc.Get("EDITTS")) ? 0 : Convert.ToInt32(doc.Get("EDITWHO"));
                        ArrOLuneceModel.Add(OLuneceModel);
                    }
                    else
                        break;
                }
                return ArrOLuneceModel;
            }
        }

        /// <summary>
        /// 分词
        /// </summary>
        public class SplitContent
        {
            public static string[] SplitWords(string content)
            {
                List<string> strList = new List<string>();
                Analyzer analyzer = new PanGuAnalyzer();//指定使用盘古 PanGuAnalyzer 分词算法
                TokenStream tokenStream = analyzer.TokenStream("", new StringReader(content));
                Lucene.Net.Analysis.Token token = null;
                while ((token = tokenStream.Next()) != null)
                {
                    if (token != null)
                    {
                        if (token.TermText() != null)
                        {
                            //Next继续分词 直至返回null
                            strList.Add(token.TermText()); //得到分词后结果
                        }
                    }
                }
                return strList.ToArray();
            }

            /// <summary>
            /// 搜索结果高亮显示
            /// 需要添加PanGu.HighLight.dll的引用
            /// </summary>
            /// <param name="keyword"> 关键字 </param>
            /// <param name="content"> 搜索结果 </param>
            /// <returns> 高亮后结果 </returns>
            public static string HightLight(string keyword, string content)
            {
                //创建HTMLFormatter,参数为高亮单词的前后缀
                PanGu.HighLight.SimpleHTMLFormatter simpleHTMLFormatter = new PanGu.HighLight.SimpleHTMLFormatter("<font style=\"font-style:normal;color:#cc0000;\"><b>", "</b></font>");
                //创建 Highlighter ，输入HTMLFormatter 和 盘古分词对象Semgent
                PanGu.HighLight.Highlighter highlighter = new PanGu.HighLight.Highlighter(simpleHTMLFormatter, new Segment());
                //设置每个摘要段的字符数
                highlighter.FragmentSize = 1000;
                //获取最匹配的摘要段
                return highlighter.GetBestFragment(keyword, content);
            }
        }
    }

    //操作类型枚举
    public enum LuneceIndexType
    {
        Insert = 0,
        Modify = 1,
        Delete = 2
    }
}

namespace TMI.Web.Models
{
    public class LuneceModel
    {
        [Display(Name = "表名", Description = "", Order = 1)]
        public string TableName { get; set; }

        [Display(Name = "表主键", Description = "", Order = 2)]
        public string TableKey { get; set; }

        //[Display(Name = "标题", Description = "", Order = 3)]
        //public string Title { get; set; }

        //[Display(Name = "副标题", Description = "", Order = 4)]
        //public string Title { get; set; }

        [Display(Name = "检索内容", Description = "", Order = 5)]
        public string Content { get; set; }

        [Display(Name = "操作点", Description = "", Order = 6)]
        public int OperatingPoint { get; set; }

        [Display(Name = "创建人", Description = "", Order = 7)]
        [StringLength(20)]
        public string ADDWHO { get; set; }

        [Display(Name = "创建时间", Description = "", Order = 8)]
        public DateTime? ADDTS { get; set; }

        [Display(Name = "修改人", Description = "", Order = 9)]
        [StringLength(20)]
        public string EDITWHO { get; set; }

        [Display(Name = "修改日期", Description = "", Order = 10)]
        public DateTime? EDITTS { get; set; }

        [Display(Name = "操作类型", Description = "", Order = 10)]
        public LuneceIndexType IndexType { get; set; }
    }

}