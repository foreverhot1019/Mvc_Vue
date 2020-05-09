using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
//using Microsoft.Office.Interop.OneNote;
using System.Xml;

namespace TMI.Web
{
    public class OCRReadText
    {
        public OCRReadText()
        {
            //构造函数
        }

        /// <summary>
        /// 识别图片中的文字（不支持 FrameWork4.5）
        /// framework2.0 32位服务器
        /// </summary>
        /// <param name="imgfileName">图片路径</param>
        /// <returns></returns>
        public static string OCRReadTextByMODI(string imgfileName)
        {
            //报错 不支持 FrameWork4.5
            var langs = MODI.MiLANGUAGES.miLANG_CHINESE_SIMPLIFIED; //中文含英文
            // MODI.MiLANGUAGES.miLANG_ENGLISH;
            // MODI.MiLANGUAGES.miLANG_JAPANESE; //日文含英文
            var doc = new MODI.Document();
            var image = default(MODI.Image);
            var layout = default(MODI.Layout);

            try
            {
                doc.Create(imgfileName);
                doc.OCR(langs, true, true);
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < doc.Images.Count; i++)
                {
                    image = (MODI.Image)doc.Images[i];
                    layout = image.Layout;
                    sb.AppendLine(string.Format("{0}, {1}", i, layout.Text));
                }
                doc.Close(false);
                return sb.ToString();
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
            finally
            {
                layout = null;
                image = null;
                doc = null;
            }
        }

        /// <summary>
        ///  图片转换成 Base64
        /// </summary>
        /// <param name="im"></param>
        /// <param name="ImgExtension"></param>
        /// <returns></returns>
        private static string GetBase64ByImage(Image im, ref string ImgExtension)
        {
            string _Base64 = "";
            using (MemoryStream ms = new MemoryStream())
            {
                #region  图片转换成 Base64

                if (im.RawFormat.Guid == ImageFormat.Jpeg.Guid)
                {
                    im.Save(ms, ImageFormat.Jpeg);
                    ImgExtension = "jpg";
                }
                else if (im.RawFormat.Guid == ImageFormat.Gif.Guid)
                {
                    im.Save(ms, ImageFormat.Jpeg);
                    ImgExtension = "jpg";
                }
                else if (im.RawFormat.Guid == ImageFormat.Bmp.Guid)
                {
                    im.Save(ms, ImageFormat.Bmp);
                    ImgExtension = "bmp";
                }
                else if (im.RawFormat.Guid == ImageFormat.Tiff.Guid)
                {
                    im.Save(ms, ImageFormat.Tiff);
                    ImgExtension = "tiff";
                }
                else if (im.RawFormat.Guid == ImageFormat.Png.Guid)
                {
                    im.Save(ms, ImageFormat.Png);
                    ImgExtension = "png";
                }
                else if (im.RawFormat.Guid == ImageFormat.Emf.Guid)
                {
                    im.Save(ms, ImageFormat.Emf);
                    ImgExtension = "emf";
                }
                else
                {
                    return "";
                }

                #endregion

                byte[] buffer = ms.GetBuffer();
                _Base64 = Convert.ToBase64String(buffer);
            }

            return _Base64;
        }

        //private static string m_OcrPageID = "";

        /// <summary>
        /// 识别图片中的文字
        /// https://msdn.microsoft.com/zh-cn/ff966472  error code
        /// https://msdn.microsoft.com/zh-cn/library/office/ff796230.aspx
        /// OneNote必须至少有一个空笔记
        /// 引用COM中的OneNote 15.0 Object Library
        /// </summary>
        /// <param name="s_ImgPath">图片本地路径</param>
        /// <param name="i_DelaySecond">延迟时间，保证ocr处理完成</param>
        /// <param name="s_Message"></param>
        /// <returns></returns>
        public static bool OCRReadTextByOneNote(string s_ImgPath, int i_DelaySecond, out string s_Message)
        {
            bool ret = false;
            i_DelaySecond = i_DelaySecond < 1000 ? 1000 : i_DelaySecond;
            s_Message = "";
            try
            {
                using (Image im = Image.FromFile(s_ImgPath))
                {
                    //ImageType只支持这些类型：auto|png|emf|jpg
                    string ImgExtension = "";
                    //图片转Base64的string
                    string _Base64 = GetBase64ByImage(im, ref ImgExtension);
                    //创建OneNote应用
                   // var onenoteApp = new Microsoft.Office.Interop.OneNote.Application();
                    //储存OneNote的noteBook XML
                    //string notebookXml;
                    //先检查 是否已 建立过 page
                    //if (m_OcrPageID == "")
                    //{
                    //hsSections
                  //  onenoteApp.GetHierarchy(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, out notebookXml, Microsoft.Office.Interop.OneNote.XMLSchema.xsCurrent);
                    //var doc = XDocument.Parse(notebookXml);
                   // var ns = doc.Root.Name.Namespace;
                   // var sectionNode = doc.Descendants(ns + "Section").Where(x => x.FirstAttribute.Value != "删除的页面").FirstOrDefault();
                    //var sectionID = sectionNode.Attribute("ID").Value;
                   // onenoteApp.CreateNewPage(sectionID, out m_OcrPageID, NewPageStyle.npsBlankPageNoTitle);
                    //}
                    //hsPages
                  //  onenoteApp.GetHierarchy(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, out notebookXml, Microsoft.Office.Interop.OneNote.XMLSchema.xsCurrent);
                    //向OneNote插入新的XML格式的包含图片的Page
                   // var _pdoc = XDocument.Parse(notebookXml);
                    //var _pns = _pdoc.Root.Name.Namespace;
                    //var _page = new XDocument(new XElement(_pns + "Page", new XAttribute("ID", m_OcrPageID),
                    //                new XElement(_pns + "Outline",
                    //                    new XElement(_pns + "OEChildren",
                    //                        new XElement(_pns + "OE",
                    //                            new XElement(_pns + "Image", new XAttribute("format", ImgExtension), new XAttribute("originalPageNumber", "0"),
                    //                                new XElement(_pns + "Position", new XAttribute("x", "0"), new XAttribute("y", "0"), new XAttribute("z", "0")),
                    //                                new XElement(_pns + "Size", new XAttribute("width", im.Width.ToString()), new XAttribute("height", im.Height.ToString())),
                    //                                new XElement(_pns + "Data", _Base64)))))));
                    //onenoteApp.UpdatePageContent(_page.ToString(), DateTime.MinValue);
                    //线程休眠时间，单位毫秒，若图片很大，则延长休眠时间，保证Onenote OCR完毕
                    System.Threading.Thread.Sleep(i_DelaySecond);
                    //获取新的OneNote Page的XML内容
                    //string pageXml;
                    //onenoteApp.GetPageContent(m_OcrPageID, out pageXml, Microsoft.Office.Interop.OneNote.PageInfo.piBasic);
                    //抓取识别文字
                  //  XDocument _OCRText = XDocument.Parse(pageXml);
                   // IEnumerable<XElement> i_OCRText = _OCRText.Descendants(_pns + "OCRText");
                    //if (i_OCRText.Count() > 0)
                    //{
                    //    s_Message = i_OCRText.FirstOrDefault().Value;
                    //}

                    //onenoteApp.DeleteHierarchy(m_OcrPageID);
                }
                ret = true;
            }
            catch (Exception ex)
            {
                s_Message = TMI.Web.Extensions.Common.GetExceptionMsg(ex);
                ret = false;
            }
            return ret;
        }
    }
}