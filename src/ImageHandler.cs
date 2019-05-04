using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using VL.Lib.Basics.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace VL.Devices.Kinect2
{
    public static class ImageHandler
    {
        static object m_lock = new object();

        public static BitmapImage GetColorImage(ColorFrame frame)
        {
            BitmapImage result = null;
            if (frame != null)
            {
                using (frame)
                {
                    result = ImageExtensions.ToImage(ImageToBitmap(frame), false);
                }
            }
            return result;
        }

        private static Bitmap ImageToBitmap(ColorFrame img)
        {
            FrameDescription description = img.CreateFrameDescription(ColorImageFormat.Bgra);
            byte[] pixeldata = new byte[1920*1080*4];
            img.CopyConvertedFrameDataToArray(pixeldata , ColorImageFormat.Bgra);
            Bitmap bmap = new Bitmap(description.Width, description.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            BitmapData bmapdata = bmap.LockBits(
                new Rectangle(0, 0, description.Width, description.Height),
                ImageLockMode.WriteOnly,
                bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(pixeldata, 0, ptr, 1920 * 1080 * 4);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }
    }
}
