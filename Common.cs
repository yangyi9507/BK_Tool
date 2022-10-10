using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BK_Tool
{
    public class Common
    {

        #region 将图片进行反色处理
        /// <summary>
        /// 将图片进行反色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        /// <returns>被反色后的图片</returns>
        /// 
        public Bitmap RePic(Bitmap mybm, int width, int height)
        {
            Bitmap bm = new Bitmap(width, height);//初始化一个记录处理后的图片的对象
            int x, y, resultR, resultG, resultB;
            Color pixel;

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    resultR = 255 - pixel.R;//反红
                    resultG = 255 - pixel.G;//反绿
                    resultB = 255 - pixel.B;//反蓝
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));//绘图
                }
            }

            return bm;//返回经过反色处理后的图片
        }
        #endregion

        #region 输入字符串获取图像
        /// <summary>
        /// 输入字符串获取图像
        /// </summary>
        /// <param name="base64string"></param>
        public Bitmap GetImgjpg(string base64string)
        {
            byte[] bt = Convert.FromBase64String(base64string);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bt);
            Bitmap bitmap = new Bitmap(stream);

            bitmap = RePic(bitmap, bitmap.Width, bitmap.Height);
            return bitmap;
        }
        #endregion

        #region 重新获取Base64
        public string ImgToBase64String(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion        

        #region 时间非空判断
        public DateTime IsNullCheck(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dt;
                try
                {
                    dt = Convert.ToDateTime(str);
                }
                catch (Exception)
                {

                    dt = Convert.ToDateTime(str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2) + " " + str.Substring(8, 2) + ":" + str.Substring(10, 2) + ":" + str.Substring(12, 2));
                }

                return dt;
            }
            else
            {
                return DateTime.Now;
            }
        }

        #endregion

        #region 获取年龄
        public string GetAge(DateTime dtBirthday, DateTime dtNow)
        {
            string strAge = string.Empty; // 年龄的字符串表示
            int intYear = 0; // 岁
            int intMonth = 0; // 月
            int intDay = 0; // 天

            // 计算天数
            intDay = dtNow.Day - dtBirthday.Day;
            if (intDay < 0)
            {
                dtNow = dtNow.AddMonths(-1);
                intDay += DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            }

            // 计算月数
            intMonth = dtNow.Month - dtBirthday.Month;
            if (intMonth < 0)
            {
                intMonth += 12;
                dtNow = dtNow.AddYears(-1);
            }

            // 计算年数
            intYear = dtNow.Year - dtBirthday.Year;

            // 格式化年龄输出
            if (intYear >= 1) // 年份输出
            {
                strAge = intYear.ToString() + "岁";
            }

            if (intMonth > 0 && intYear <= 5) // 五岁以下可以输出月数
            {
                strAge += intMonth.ToString() + "月";
            }

            if (intDay >= 0 && intYear < 1) // 一岁以下可以输出天数
            {
                if (strAge.Length == 0 || intDay > 0)
                {
                    strAge += intDay.ToString() + "日";
                }
            }

            return strAge;
        }
        #endregion
    }
}
