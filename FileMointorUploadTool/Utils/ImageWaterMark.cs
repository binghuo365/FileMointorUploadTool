using System;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace FileMointorUploadTool.Utils
{
   public class ImageWaterMark
    {
        /// <summary>    
        /// Creating a Watermarked Photograph with GDI+ for .NET    
        /// </summary>    
        /// <param name="rSrcImgPath">原始图片的物理路径</param>    
        /// <param name="rMarkImgPath">水印图片的物理路径</param>    
        /// <param name="rMarkText">水印文字（不显示水印文字设为空串）</param>    
        /// <param name="rDstImgPath">输出合成后的图片的物理路径</param>    
        public static void  BuildWatermark(string rSrcImgPath, string rMarkImgPath, string rMarkText, string rDstImgPath)
        {
            Image imgPhoto = Image.FromFile(rSrcImgPath);
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            Image imgWatermark = new Bitmap(rMarkImgPath);
            int wmWidth = imgWatermark.Width;
            int wmHeight = imgWatermark.Height;
            //这个代码以100%它的原始大小绘制imgPhoto 到Graphics 对象的（x=0,y=0）位置。    
            //以后所有的绘图都将发生在原来照片的顶部。    
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.DrawImage(
                 imgPhoto,
                 new Rectangle(0, 0, phWidth, phHeight),
                 0,
                 0,
                 phWidth,
                 phHeight,
                 GraphicsUnit.Pixel);
            //为了最大化版权信息的大小，我们将测试7种不同的字体大小来决定我们能为我们的照片宽度使用的可能的最大大小。    
            //为了有效地完成这个，我们将定义一个整型数组，接着遍历这些整型值测量不同大小的版权字符串。    
            //一旦我们决定了可能的最大大小，我们就退出循环，绘制文本    
            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
            Font crFont = null;
            SizeF crSize = new SizeF();
            for (int i = 0; i < 7; i++)
            {
                crFont = new Font("arial", sizes[i],
                      FontStyle.Bold);
                crSize = grPhoto.MeasureString(rMarkText,
                      crFont);
                if ((ushort)crSize.Width < (ushort)phWidth)
                    break;
            }
            int yPixlesFromBottom = (int)(phHeight * .05);
            float yPosFromBottom = ((phHeight -
                 yPixlesFromBottom) - (crSize.Height / 2));
            float xCenterOfImg = (phWidth / 2);
            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;
            SolidBrush semiTransBrush2 =
                 new SolidBrush(Color.FromArgb(153, 0, 0, 0));
            grPhoto.DrawString(rMarkText,
                 crFont,
                 semiTransBrush2,
                 new PointF(xCenterOfImg + 1, yPosFromBottom + 1),
                 StrFormat);
            SolidBrush semiTransBrush = new SolidBrush(
                 Color.FromArgb(153, 255, 255, 255));
            grPhoto.DrawString(rMarkText,
                 crFont,
                 semiTransBrush,
                 new PointF(xCenterOfImg, yPosFromBottom),
                 StrFormat);
            Bitmap bmWatermark = new Bitmap(bmPhoto);
            bmWatermark.SetResolution(
                 imgPhoto.HorizontalResolution,
                 imgPhoto.VerticalResolution);
            Graphics grWatermark =
                 Graphics.FromImage(bmWatermark);
            ImageAttributes imageAttributes =
                 new ImageAttributes();
            ColorMap colorMap = new ColorMap();
            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };
            imageAttributes.SetRemapTable(remapTable,
                 ColorAdjustType.Bitmap);
            float[][] colorMatrixElements = {     
                                             new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},    
                                             new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},    
                                             new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},    
                                             new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},    
                                             new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}    
                                        };
            ColorMatrix wmColorMatrix = new
                 ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(wmColorMatrix,
                 ColorMatrixFlag.Default,
                 ColorAdjustType.Bitmap);
            int markWidth;
            int markHeight;
            //mark比原来的图宽    
            if (phWidth <= wmWidth)
            {
                markWidth = phWidth - 10;
                markHeight = (markWidth * wmHeight) / wmWidth;
            }
            else if (phHeight <= wmHeight)
            {
                markHeight = phHeight - 10;
                markWidth = (markHeight * wmWidth) / wmHeight;
            }
            else
            {
                markWidth = wmWidth;
                markHeight = wmHeight;
            }
            int xPosOfWm = ((phWidth - markWidth) - 10);
            int yPosOfWm = 10;
            grWatermark.DrawImage(imgWatermark,
                 new Rectangle(xPosOfWm, yPosOfWm, markWidth,
                 markHeight),
                 0,
                 0,
                 wmWidth,
                 wmHeight,
                 GraphicsUnit.Pixel,
                 imageAttributes);
            //最后的步骤将是使用新的Bitmap取代原来的Image。 销毁两个Graphic对象，然后把Image 保存到文件系统。    
            imgPhoto = bmWatermark;
            grPhoto.Dispose();
            grWatermark.Dispose();
            imgPhoto.Save(rDstImgPath);
            imgPhoto.Dispose();
            imgWatermark.Dispose();
        }



        public static void WaterImage(string filePath, string markPath, string text, string destPath)
        {
            Bitmap origialGif = new Bitmap(filePath);
            //用于存放桢 
            List<Gif.Components.GifDecoder.GifFrame> frames = new
            List<Gif.Components.GifDecoder.GifFrame>();
            //如果不是gif文件,直接返回原图像 
            if (origialGif.RawFormat.Guid != System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
                BuildWatermark(filePath, markPath, text, destPath) ;
                return;
            }
            //如果该图像是gif文件 
            foreach (Guid guid in origialGif.FrameDimensionsList)
            {
                System.Drawing.Imaging.FrameDimension
                frameDimension = new System.Drawing.Imaging.FrameDimension(guid);
                int frameCount = origialGif.GetFrameCount(frameDimension);
                for (int i = 0; i < frameCount; i++)
                {
                    if (origialGif.SelectActiveFrame(frameDimension,i) == 0)
                    {
                        int delay =
                        Convert.ToInt32(origialGif.GetPropertyItem(20736).Value.GetValue(i));
                        Image img = Image.FromHbitmap(origialGif.GetHbitmap());
                        Font font = new Font(new
                        FontFamily("arial"), 10.0f, FontStyle.Regular);
                        Graphics g =
                        Graphics.FromImage(img);
                        g.DrawString(text, font, Brushes.BlanchedAlmond,
                        new PointF(origialGif.Width - 100.0f, origialGif.Height - 20.0f));
                        Gif.Components.GifDecoder.GifFrame frame = new Gif.Components.GifDecoder.GifFrame(img, delay);

                        frames.Add(frame);
                    }
                }
                Gif.Components.AnimatedGifEncoder gif =
                new Gif.Components.AnimatedGifEncoder();
                gif.Start(destPath);

                gif.SetDelay(100);
                gif.SetRepeat(0);
                for (int i = 0; i < frames.Count; i++)
                {
                    gif.AddFrame(frames[i].image);
                }
                gif.Finish();
            }
            return;
        } 

    }
}
