using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NUCForeground
{
    /// <summary>
    /// SRC: https://blog.csdn.net/zgke/article/details/3916679 
    /// BY: zgke
    /// </summary>
    public class NEMIconSearcher
    {
        /// <summary>  
        /// 判断图形里是否存在另外一个图形 并返回所在位置  
        /// </summary>  
        /// <param name="p_SourceBitmap">原始图形</param>  
        /// <param name="p_PartBitmap">小图形</param>  
        /// <param name="p_Float">溶差</param>  
        /// <returns>坐标</returns>  
        public static Point GetImageContains(Bitmap p_SourceBitmap, Bitmap p_PartBitmap, int p_Float)
        {
            int _SourceWidth = p_SourceBitmap.Width;
            int _SourceHeight = p_SourceBitmap.Height;

            int _PartWidth = p_PartBitmap.Width;
            int _PartHeight = p_PartBitmap.Height;

            Bitmap _SourceBitmap = new Bitmap(_SourceWidth, _SourceHeight);
            Graphics _Graphics = Graphics.FromImage(_SourceBitmap);
            _Graphics.DrawImage(p_SourceBitmap, new Rectangle(0, 0, _SourceWidth, _SourceHeight));
            _Graphics.Dispose();
            BitmapData _SourceData = _SourceBitmap.LockBits(new Rectangle(0, 0, _SourceWidth, _SourceHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] _SourceByte = new byte[_SourceData.Stride * _SourceHeight];
            Marshal.Copy(_SourceData.Scan0, _SourceByte, 0, _SourceByte.Length);  //复制出p_SourceBitmap的相素信息   

            Bitmap _PartBitmap = new Bitmap(_PartWidth, _PartHeight);
            _Graphics = Graphics.FromImage(_PartBitmap);
            _Graphics.DrawImage(p_PartBitmap, new Rectangle(0, 0, _PartWidth, _PartHeight));
            _Graphics.Dispose();
            BitmapData _PartData = _PartBitmap.LockBits(new Rectangle(0, 0, _PartWidth, _PartHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] _PartByte = new byte[_PartData.Stride * _PartHeight];
            Marshal.Copy(_PartData.Scan0, _PartByte, 0, _PartByte.Length);   //复制出p_PartBitmap的相素信息   
            _PartBitmap.UnlockBits(_PartData);
            _SourceBitmap.UnlockBits(_SourceData);

            for (int i = 0; i != _SourceHeight; i++)
            {
                if (_SourceHeight - i < _PartHeight) return new Point(-1, -1);  //如果 剩余的高 比需要比较的高 还要小 就直接返回               
                int _PointX = -1;    //临时存放坐标 需要包正找到的是在一个X点上  
                bool _SacnOver = true;   //是否都比配的上  
                for (int z = 0; z != _PartHeight - 1; z++)       //循环目标进行比较  
                {
                    int _TrueX = GetImageContains(_SourceByte, _PartByte, i * _SourceData.Stride, _SourceWidth, _PartWidth, p_Float);

                    if (_TrueX == -1)   //如果没找到   
                    {
                        _PointX = -1;    //设置坐标为没找到  
                        _SacnOver = false;   //设置不进行返回  
                        break;
                    }
                    else
                    {
                        if (z == 0) _PointX = _TrueX;
                        if (_PointX != _TrueX)   //如果找到了 也的保证坐标和上一行的坐标一样 否则也返回  
                        {
                            _PointX = -1;//设置坐标为没找到  
                            _SacnOver = false;  //设置不进行返回  
                            break;
                        }
                    }
                }

                _Graphics.Dispose();
                _PartBitmap.Dispose();
                _SourceBitmap.Dispose();

                if (_SacnOver) return new Point(_PointX, i);
            }

            return new Point(-1, -1);
        }
        /// <summary>  
        /// 判断图形里是否存在另外一个图形 所在行的索引  
        /// </summary>  
        /// <param name="p_Source">原始图形数据</param>  
        /// <param name="p_Part">小图形数据</param>  
        /// <param name="p_SourceIndex">开始位置</param>  
        /// <param name="p_SourceWidth">原始图形宽</param>  
        /// <param name="p_PartWidth">小图宽</param>  
        /// <param name="p_Float">溶差</param>  
        /// <returns>所在行的索引 如果找不到返回-1</returns>  
        public static int GetImageContains(byte[] p_Source, byte[] p_Part, int p_SourceIndex, int p_SourceWidth, int p_PartWidth, int p_Float)
        {
            int _PartIndex = 0;
            int _SourceIndex = p_SourceIndex;
            for (int i = 0; i < p_SourceWidth; i++)
            {
                if (p_SourceWidth - i < p_PartWidth) return -1;
                Color _CurrentlyColor = Color.FromArgb((int)p_Source[_SourceIndex + 3], (int)p_Source[_SourceIndex + 2], (int)p_Source[_SourceIndex + 1], (int)p_Source[_SourceIndex]);
                Color _CompareColor = Color.FromArgb((int)p_Part[3], (int)p_Part[2], (int)p_Part[1], (int)p_Part[0]);
                _SourceIndex += 4;

                bool _ScanColor = ScanColor(_CurrentlyColor, _CompareColor, p_Float);

                if (_ScanColor)
                {
                    _PartIndex += 4;
                    int _SourceRVA = _SourceIndex;
                    bool _Equals = true;
                    for (int z = 0; z != p_PartWidth - 1; z++)
                    {
                        _CurrentlyColor = Color.FromArgb((int)p_Source[_SourceRVA + 3], (int)p_Source[_SourceRVA + 2], (int)p_Source[_SourceRVA + 1], (int)p_Source[_SourceRVA]);
                        _CompareColor = Color.FromArgb((int)p_Part[_PartIndex + 3], (int)p_Part[_PartIndex + 2], (int)p_Part[_PartIndex + 1], (int)p_Part[_PartIndex]);

                        if (!ScanColor(_CurrentlyColor, _CompareColor, p_Float))
                        {
                            _PartIndex = 0;
                            _Equals = false;
                            break;
                        }
                        _PartIndex += 4;
                        _SourceRVA += 4;
                    }
                    if (_Equals) return i;
                }
                else
                {
                    _PartIndex = 0;
                }
            }
            return -1;
        }

        /// <summary>  
        /// 检查色彩(可以根据这个更改比较方式  
        /// </summary>  
        /// <param name="p_CurrentlyColor">当前色彩</param>  
        /// <param name="p_CompareColor">比较色彩</param>  
        /// <param name="p_Float">溶差</param>  
        /// <returns></returns>  
        public static bool ScanColor(Color p_CurrentlyColor, Color p_CompareColor, int p_Float)
        {
            int _R = p_CurrentlyColor.R;
            int _G = p_CurrentlyColor.G;
            int _B = p_CurrentlyColor.B;

            return (_R <= p_CompareColor.R + p_Float && _R >= p_CompareColor.R - p_Float) && (_G <= p_CompareColor.G + p_Float && _G >= p_CompareColor.G - p_Float) && (_B <= p_CompareColor.B + p_Float && _B >= p_CompareColor.B - p_Float);

        }

    }
}
