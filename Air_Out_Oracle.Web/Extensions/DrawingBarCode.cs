using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Design;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace AirOut.Web.BarCode
{
    /// <summary>
    ///绘制条形码
    /// </summary>
    public class DrawingBarCode
    {
        public DrawingBarCode()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        #region 根据字体产生条形码

        /// <summary>
        /// 根据条形码绘制图片
        /// </summary>
        /// <param name="strNumber">条形码</param>
        public Image DrawingBarCodeImage(string strNumber)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            //39码
            strNumber = "*" + strNumber + "*";
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("/BarCodeFonts/FREE3OF9/FREE3OF9X.ttf"));
            FontFamily ff = new FontFamily("Free 3 of 9 Extended", fonts);
            Font font = new Font(ff, 12);
            //设置图片大小
            Image img = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(img);
            SizeF fontSize = g.MeasureString(strNumber, font);
            int intWidth = Convert.ToInt32(fontSize.Width);
            int intHeight = Convert.ToInt32(fontSize.Height);
            g.Dispose();
            img.Dispose();
            img = new Bitmap(intWidth, intHeight);
            g = Graphics.FromImage(img);
            g.Clear(Color.White);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawString(strNumber, font, Brushes.Black, 0, 0);
            g.Save();
            g.Dispose();
            return img;
        }

        /// <summary>
        /// 根据条形码绘制图片,并Response 输出
        /// </summary>
        /// <param name="strNumber">条形码</param>
        public void DrawingBarCodeResponse(string strNumber)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            //39码
            strNumber = "*" + strNumber + "*";
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("/BarCodeFonts/FREE3OF9/FREE3OF9X.TTF"));
            FontFamily ff = new FontFamily("Free 3 of 9 Extended", fonts);
            Font font = new Font(ff, 12);
            //设置图片大小
            Image img = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(img);
            SizeF fontSize = g.MeasureString(strNumber, font);
            int intWidth = Convert.ToInt32(fontSize.Width);
            int intHeight = Convert.ToInt32(fontSize.Height);
            g.Dispose();
            img.Dispose();
            img = new Bitmap(intWidth, intHeight);
            g = Graphics.FromImage(img);
            g.Clear(Color.White);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawString(strNumber, font, Brushes.Black, 0, 0);
            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/Jpeg";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            g.Dispose();
            img.Dispose();
        }

        /// <summary>
        /// 根据条形码绘制图片
        /// </summary>
        /// <param name="strNumber">条形码</param>
        /// <param name="intFontSize">字体大小</param>
        public Image DrawingBarCodeImage(string strNumber, int intFontSize)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            //39码
            strNumber = "*" + strNumber + "*";
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("/BarCodeFonts/FREE3OF9/FREE3OF9X.TTF"));
            FontFamily ff = new FontFamily("Free 3 of 9 Extended", fonts);
            Font font = new Font(ff, intFontSize);
            //设置图片大小
            Image img = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(img);
            SizeF fontSize = g.MeasureString(strNumber, font);
            int intWidth = Convert.ToInt32(fontSize.Width);
            int intHeight = Convert.ToInt32(fontSize.Height);
            g.Dispose();
            img.Dispose();
            img = new Bitmap(intWidth + 20, intHeight + 20);
            g = Graphics.FromImage(img);
            g.Clear(Color.White);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawString(strNumber, font, Brushes.Black, 10, 10);
            g.Save();
            g.Dispose();
            return img;
        }

        /// <summary>
        /// 根据条形码绘制图片,加上文字显示
        /// </summary>
        /// <param name="strNumber">条形码</param>
        /// <param name="intFontSize">字体大小</param>
        public Image DrawingBarCodeImage(string strNumber, int intFontSize,bool showtext)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            //39码
            strNumber = "*" + strNumber + "*";
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("/BarCodeFonts/FREE3OF9/FREE3OF9X.TTF"));
            FontFamily ff = new FontFamily("Free 3 of 9 Extended", fonts);
            Font font = new Font(ff, intFontSize);
            //设置图片大小
            Image img = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(img);
            SizeF fontSize = g.MeasureString(strNumber, font);
            int intWidth = Convert.ToInt32(fontSize.Width);
            int intHeight = Convert.ToInt32(fontSize.Height);
            g.Dispose();
            img.Dispose();
            img = new Bitmap(intWidth + 20, intHeight + 25);
            g = Graphics.FromImage(img);
            g.Clear(Color.White);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawString(strNumber, font, Brushes.Black, 10, 10);
            if (showtext)
            {
                g.DrawString(strNumber, new Font("宋体", 15,FontStyle.Regular), Brushes.Black, 10, 30);
            }

            g.Save();
            g.Dispose();
            return img;
        }

        /// <summary>
        /// 根据条形码绘制图片,并Response 输出
        /// </summary>
        /// <param name="strNumber">条形码</param>
        /// <param name="intFontSize">字体大小</param>
        public void DrawingBarCodeResponse(string strNumber, int intFontSize)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            //39码
            strNumber = "*" + strNumber + "*";
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("/BarCodeFonts/FREE3OF9/FREE3OF9X.TTF"));
            FontFamily ff = new FontFamily("Free 3 of 9 Extended", fonts);
            Font font = new Font(ff, intFontSize);
            //设置图片大小
            Image img = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(img);
            SizeF fontSize = g.MeasureString(strNumber, font);
            int intWidth = Convert.ToInt32(fontSize.Width);
            int intHeight = Convert.ToInt32(fontSize.Height);
            g.Dispose();
            img.Dispose();
            img = new Bitmap(intWidth, intHeight);
            g = Graphics.FromImage(img);
            g.Clear(Color.White);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawString(strNumber, font, Brushes.Black, 0, 0);
            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/Jpeg";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            g.Dispose();
            img.Dispose();
        }

        #endregion

        #region 绘制Code39码

        /// <summary>
        /// 根据条形码绘制图片，并Response输出
        /// </summary>
        /// <param name="strNumber">条形码</param>
        public void DrawingBarCode39(string strNumber, int intFontSize, bool WithStart)
        {
            ViewFont = new Font("宋体", intFontSize);
            Image img = GetCodeImage(strNumber, Code39Model.Code39Normal, WithStart);
            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.BufferOutput = true;
            HttpContext.Current.Response.ContentType = "image/Jpeg";
            HttpContext.Current.Response.BinaryWrite(stream.GetBuffer());
            HttpContext.Current.Response.Flush();
        }

        private Hashtable m_Code39 = new Hashtable();

        private byte m_Magnify = 0;
        /// <summary> 
        /// 放大倍数 
        /// </summary> 
        public byte Magnify { get { return m_Magnify; } set { m_Magnify = value; } }

        private int m_Height = 40;
        /// <summary> 
        /// 图形高 
        /// </summary> 
        public int Height { get { return m_Height; } set { m_Height = value; } }

        private Font m_ViewFont = new Font("宋体",15);
        /// <summary> 
        /// 字体大小 
        /// </summary> 
        public Font ViewFont { get { return m_ViewFont; } set { m_ViewFont = value; } }

        /*
        Code39码的编码规则是： 
        1、 每五条线表示一个字符； 
        2、 粗线表示1，细线表示0； 
        3、 线条间的间隙宽的表示1，窄的表示0； 
        4、 五条线加上它们之间的四条间隙就是九位二进制编码，而且这九位中必定有三位是1，所以称为39码； 
        5、 条形码的首尾各一个＊标识开始和结束
         */
        public void Add39BarCode(Boolean IsCode39)
        {
            m_Code39.Clear();
            m_Code39.Add("A", "1101010010110");
            m_Code39.Add("B", "1011010010110");
            m_Code39.Add("C", "1101101001010");
            m_Code39.Add("D", "1010110010110");
            m_Code39.Add("E", "1101011001010");
            m_Code39.Add("F", "1011011001010");
            m_Code39.Add("G", "1010100110110");
            m_Code39.Add("H", "1101010011010");
            m_Code39.Add("I", "1011010011010");
            m_Code39.Add("J", "1010110011010");
            m_Code39.Add("K", "1101010100110");
            m_Code39.Add("L", "1011010100110");
            m_Code39.Add("M", "1101101010010");
            m_Code39.Add("N", "1010110100110");
            m_Code39.Add("O", "1101011010010");
            m_Code39.Add("P", "1011011010010");
            m_Code39.Add("Q", "1010101100110");
            m_Code39.Add("R", "1101010110010");
            m_Code39.Add("S", "1011010110010");
            m_Code39.Add("T", "1010110110010");
            m_Code39.Add("U", "1100101010110");
            m_Code39.Add("V", "1001101010110");
            m_Code39.Add("W", "1100110101010");
            m_Code39.Add("X", "1001011010110");
            m_Code39.Add("Y", "1100101101010");
            m_Code39.Add("Z", "1001101101010");
            m_Code39.Add("0", "1010011011010");
            m_Code39.Add("1", "1101001010110");
            m_Code39.Add("2", "1011001010110");
            m_Code39.Add("3", "1101100101010");
            m_Code39.Add("4", "1010011010110");
            m_Code39.Add("5", "1101001101010");
            m_Code39.Add("6", "1011001101010");
            m_Code39.Add("7", "1010010110110");
            m_Code39.Add("8", "1101001011010");
            m_Code39.Add("9", "1011001011010");
            m_Code39.Add("+", "1001010010010");
            m_Code39.Add("-", "1001010110110");
            m_Code39.Add("*", "1001011011010");
            m_Code39.Add("/", "1001001010010");
            m_Code39.Add("%", "1010010010010");
            m_Code39.Add("$", "1001001001010");
            m_Code39.Add(".", "1100101011010");
            m_Code39.Add(" ", "1001101011010");
        }

        public enum Code39Model
        {
            /// <summary> 
            /// 基本类别 1234567890ABC 
            /// </summary> 
            Code39Normal,
            /// <summary> 
            /// 全ASCII方式 +A+B 来表示小写 
            /// </summary> 
            Code39FullAscII
        }

        /// <summary> 
        /// 获得条码图形 
        /// </summary> 
        /// <param name="p_Text">文字信息</param> 
        /// <param name="p_Model">类别</param> 
        /// <param name="p_StarChar">是否增加前后*号</param> 
        /// <returns>图形</returns> 
        public Bitmap GetCodeImage(string p_Text, Code39Model p_Model = Code39Model.Code39Normal, bool p_StarChar = true)
        {
            if((m_Code39==null ||m_Code39.Count<=0))
                Add39BarCode(true);

            string _ValueText = "";
            string _CodeText = "";
            char[] _ValueChar = null;
            switch (p_Model)
            {
                case Code39Model.Code39Normal:
                    _ValueText = p_Text.ToUpper();
                    break;
                default:
                    _ValueChar = p_Text.ToCharArray();
                    for (int i = 0; i != _ValueChar.Length; i++)
                    {
                        if ((int)_ValueChar[i] >= 97 && (int)_ValueChar[i] <= 122)
                        {
                            _ValueText += "+" + _ValueChar[i].ToString().ToUpper();
                        }
                        else
                        {
                            _ValueText += _ValueChar[i].ToString();
                        }
                    }
                    break;
            }
            _ValueChar = _ValueText.ToCharArray();
            if (p_StarChar == true) _CodeText += m_Code39["*"];
            for (int i = 0; i != _ValueChar.Length; i++)
            {
                if (p_StarChar == true && _ValueChar[i] == '*')
                    throw new Exception("带有起始符号不能出现*");
                string key = _ValueChar[i].ToString();
                object _CharCode = m_Code39[key];
                if (_CharCode == null)
                    throw new Exception("不可用的字符" + _ValueChar[i].ToString());
                _CodeText += _CharCode.ToString();
            }
            if (p_StarChar == true) 
                _CodeText += m_Code39["*"];
            Bitmap _CodeBmp = GetImage(_CodeText);
            GetViewImage(_CodeBmp, p_Text);

            return _CodeBmp;
           
        }

        /// <summary> 
        /// 绘制编码图形 
        /// </summary> 
        /// <param name="p_Text">编码</param> 
        /// <returns>图形</returns> 
        private Bitmap GetImage(string p_Text)
        {
            char[] _Value = p_Text.ToCharArray();
            //宽 == 需要绘制的数量*放大倍数 + 两个字的宽    
            Bitmap _CodeImage = new Bitmap(_Value.Length * ((int)m_Magnify + 1) + 20, (int)m_Height + 20);
            Graphics _Garphics = Graphics.FromImage(_CodeImage);
            //Graphics _Garphics = new Graphics();
            //_Garphics.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
            _Garphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            _Garphics.CompositingQuality = CompositingQuality.HighQuality;
            
            _Garphics.FillRectangle(Brushes.White, new Rectangle(0, 0, _CodeImage.Width, _CodeImage.Height));
            
            int _LenEx = 10;
            for (int i = 0; i != _Value.Length; i++)
            {
                int _DrawWidth = m_Magnify + 1;
                if (_Value[i] == '1')
                {
                    _Garphics.FillRectangle(Brushes.Black, new Rectangle(_LenEx, 10, _DrawWidth, m_Height));
                }
                else
                {
                    _Garphics.FillRectangle(Brushes.White, new Rectangle(_LenEx, 10, _DrawWidth, m_Height));
                }
                _LenEx += _DrawWidth;
            }
            _Garphics.Save();
            _Garphics.Dispose();
            return _CodeImage;
        }

        /// <summary> 
        /// 绘制文字,文字宽度大于图片宽度将不显示 
        /// </summary> 
        /// <param name="p_CodeImage">图形</param> 
        /// <param name="p_Text">文字</param> 
        private void GetViewImage(Bitmap p_CodeImage, string p_Text)
        {
            if (m_ViewFont == null) return;
            Graphics _Graphics = Graphics.FromImage(p_CodeImage);
            //_Graphics.SmoothingMode = SmoothingMode.HighQuality;  //使绘图质量最高，即消除锯齿
            _Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            _Graphics.CompositingQuality = CompositingQuality.HighQuality;
            SizeF _FontSize = _Graphics.MeasureString(p_Text, m_ViewFont);
            if (_FontSize.Width > p_CodeImage.Width || _FontSize.Height > p_CodeImage.Height - 20)
            {
                _Graphics.Dispose();
                return;
            }
            int _StarHeight = p_CodeImage.Height - (int)_FontSize.Height;
            _Graphics.FillRectangle(Brushes.White, new Rectangle(0, _StarHeight, p_CodeImage.Width, (int)_FontSize.Height));
            int _StarWidth = (p_CodeImage.Width - (int)_FontSize.Width) / 2;
            p_Text = "*" + p_Text + "*";//添加星号
            _Graphics.DrawString(p_Text, m_ViewFont, Brushes.Black, _StarWidth, _StarHeight);
            _Graphics.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// 39码条形码 转换成 HTML
    /// EAN13条形码 转换成 HTML
    /// </summary>
    public class BarCodeToHTML
    {
        /// <summary>
        /// 将字符串 转换成 39码12位条形码
        /// </summary>
        /// <param name="s">条形码 字符串</param>
        /// <param name="width">生成的条形码 线条宽度</param>
        /// <param name="height">生成的条形码 线条高度</param>
        /// <returns></returns>
        public static string Get39BarCodeHtml(string s, int width=1, int height=23)
        {
            Hashtable ht = new Hashtable();

            #region 39码 12位

            ht.Add('A', "110101001011");
            ht.Add('B', "101101001011");
            ht.Add('C', "110110100101");
            ht.Add('D', "101011001011");
            ht.Add('E', "110101100101");
            ht.Add('F', "101101100101");
            ht.Add('G', "101010011011");
            ht.Add('H', "110101001101");
            ht.Add('I', "101101001101");
            ht.Add('J', "101011001101");
            ht.Add('K', "110101010011");
            ht.Add('L', "101101010011");
            ht.Add('M', "110110101001");
            ht.Add('N', "101011010011");
            ht.Add('O', "110101101001");
            ht.Add('P', "101101101001");
            ht.Add('Q', "101010110011");
            ht.Add('R', "110101011001");
            ht.Add('S', "101101011001");
            ht.Add('T', "101011011001");
            ht.Add('U', "110010101011");
            ht.Add('V', "100110101011");
            ht.Add('W', "110011010101");
            ht.Add('X', "100101101011");
            ht.Add('Y', "110010110101");
            ht.Add('Z', "100110110101");
            ht.Add('0', "101001101101");
            ht.Add('1', "110100101011");
            ht.Add('2', "101100101011");
            ht.Add('3', "110110010101");
            ht.Add('4', "101001101011");
            ht.Add('5', "110100110101");
            ht.Add('6', "101100110101");
            ht.Add('7', "101001011011");
            ht.Add('8', "110100101101");
            ht.Add('9', "101100101101");
            ht.Add('+', "100101001001");
            ht.Add('-', "100101011011");
            ht.Add('*', "100101101101");
            ht.Add('/', "100100101001");
            ht.Add('%', "101001001001");
            ht.Add('$', "100100100101");
            ht.Add('.', "110010101101");
            ht.Add(' ', "100110101101");

            #endregion

            s = "*" + s.ToUpper() + "*";

            string result_bin = "";//二进制串

            try
            {
                foreach (char ch in s)
                {
                    result_bin += ht[ch].ToString();
                    result_bin += "0";//间隔，与一个单位的线条宽度相等
                }
            }
            catch { return "存在不允许的字符！"; }

            string result_html = "";//HTML代码
            string color = "";//颜色
            foreach (char c in result_bin)
            {
                color = c == '0' ? "#FFFFFF" : "#000000";
                result_html += "<div style=\"width:" + width + "px;height:" + height + "px;float:left;background:" + color + ";\"></div>";
            }
            result_html += "<div style=\"clear:both\"></div>";

            int len = ht['*'].ToString().Length;
            foreach (char c in s)
            {
                result_html += "<div style=\"width:" + (width * (len + 1)) + "px;float:left;color:#000000;text-align:center;\">" + c + "</div>";
            }
            result_html += "<div style=\"clear:both\"></div>";

            return "<div style=\"background:#FFFFFF;padding:5px;font-size:" + (width * 10) + "px;font-family:'楷体';\">" + result_html + "</div>";
        }

        /// <summary>
        /// 将字符串 转换成 EAN13条形码
        /// </summary>
        /// <param name="s">条形码 字符串</param>
        /// <param name="width">生成的条形码 线条宽度</param>
        /// <param name="height">生成的条形码 线条高度</param>
        /// <returns></returns>
        public static string GetEAN13BarCodeHtml(string s, int width=1, int height=23)
        {
            int checkcode_input = -1;//输入的校验码
            if (!Regex.IsMatch(s, @"^\d{12}$"))
            {
                if (!Regex.IsMatch(s, @"^\d{13}$"))
                {
                    return "存在不允许的字符！";
                }
                else
                {
                    checkcode_input = int.Parse(s[12].ToString());
                    s = s.Substring(0, 12);
                }
            }

            int sum_even = 0;//偶数位之和
            int sum_odd = 0;//奇数位之和

            for (int i = 0; i < 12; i++)
            {
                if (i % 2 == 0)
                {
                    sum_odd += int.Parse(s[i].ToString());
                }
                else
                {
                    sum_even += int.Parse(s[i].ToString());
                }
            }

            int checkcode = (10 - (sum_even * 3 + sum_odd) % 10) % 10;//校验码

            if (checkcode_input > 0 && checkcode_input != checkcode)
            {
                return "输入的校验码错误！";
            }

            s += checkcode;//变成13位

            // 000000000101左侧42个01010右侧35个校验7个101000000000
            // 6        101左侧6位 01010右侧5位 校验1位101000000000

            string result_bin = "";//二进制串
            result_bin += "000000000101";

            string type = ean13type(s[0]);
            for (int i = 1; i < 7; i++)
            {
                result_bin += ean13(s[i], type[i - 1]);
            }
            result_bin += "01010";
            for (int i = 7; i < 13; i++)
            {
                result_bin += ean13(s[i], 'C');
            }
            result_bin += "101000000000";

            string result_html = "";//HTML代码
            string color = "";//颜色
            int height_bottom = width * 5;
            foreach (char c in result_bin)
            {
                color = c == '0' ? "#FFFFFF" : "#000000";
                result_html += "<div style=\"width:" + width + "px;height:" + height + "px;float:left;background:" + color + ";\"></div>";
            }
            result_html += "<div style=\"clear:both\"></div>";

            result_html += "<div style=\"float:left;color:#000000;width:" + (width * 9) + "px;text-align:center;\">" + s[0] + "</div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";
            for (int i = 1; i < 7; i++)
            {
                result_html += "<div style=\"float:left;width:" + (width * 7) + "px;color:#000000;text-align:center;\">" + s[i] + "</div>";
            }
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";
            for (int i = 7; i < 13; i++)
            {
                result_html += "<div style=\"float:left;width:" + (width * 7) + "px;color:#000000;text-align:center;\">" + s[i] + "</div>";
            }
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";
            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";
            result_html += "<div style=\"float:left;color:#000000;width:" + (width * 9) + "px;\"></div>";
            result_html += "<div style=\"clear:both\"></div>";

            return "<div style=\"background:#FFFFFF;padding:0px;font-size:" + (width * 10) + "px;font-family:'楷体';\">" + result_html + "</div>";
        }

        private static string ean13(char c, char type)
        {
            switch (type)
            {
                case 'A':
                    {
                        switch (c)
                        {
                            case '0': return "0001101";
                            case '1': return "0011001";
                            case '2': return "0010011";
                            case '3': return "0111101";//011101
                            case '4': return "0100011";
                            case '5': return "0110001";
                            case '6': return "0101111";
                            case '7': return "0111011";
                            case '8': return "0110111";
                            case '9': return "0001011";
                            default: return "Error!";
                        }
                    }
                case 'B':
                    {
                        switch (c)
                        {
                            case '0': return "0100111";
                            case '1': return "0110011";
                            case '2': return "0011011";
                            case '3': return "0100001";
                            case '4': return "0011101";
                            case '5': return "0111001";
                            case '6': return "0000101";//000101
                            case '7': return "0010001";
                            case '8': return "0001001";
                            case '9': return "0010111";
                            default: return "Error!";
                        }
                    }
                case 'C':
                    {
                        switch (c)
                        {
                            case '0': return "1110010";
                            case '1': return "1100110";
                            case '2': return "1101100";
                            case '3': return "1000010";
                            case '4': return "1011100";
                            case '5': return "1001110";
                            case '6': return "1010000";
                            case '7': return "1000100";
                            case '8': return "1001000";
                            case '9': return "1110100";
                            default: return "Error!";
                        }
                    }
                default: return "Error!";
            }
        }

        private static string ean13type(char c)
        {
            switch (c)
            {
                case '0': return "AAAAAA";
                case '1': return "AABABB";
                case '2': return "AABBAB";
                case '3': return "AABBBA";
                case '4': return "ABAABB";
                case '5': return "ABBAAB";
                case '6': return "ABBBAA";//中国
                case '7': return "ABABAB";
                case '8': return "ABABBA";
                case '9': return "ABBABA";
                default: return "Error!";
            }
        }
    }

}