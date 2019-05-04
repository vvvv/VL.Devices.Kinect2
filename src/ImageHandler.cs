using Microsoft.Kinect;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using VL.Lib.Basics.Imaging;

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
                    byte[] pixelData = new byte[1920 * 1080 * 4];
                    frame.CopyConvertedFrameDataToArray(pixelData, ColorImageFormat.Bgra);
                    Bitmap bitmap = new Bitmap(frame.FrameDescription.Width, frame.FrameDescription.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    BitmapData bmapdata = bitmap.LockBits(
                        new Rectangle(0, 0, frame.FrameDescription.Width, frame.FrameDescription.Height),
                        ImageLockMode.WriteOnly,
                        bitmap.PixelFormat);
                    IntPtr ptr = bmapdata.Scan0;
                    Marshal.Copy(pixelData, 0, ptr, 1920 * 1080 * 4);
                    bitmap.UnlockBits(bmapdata);
                    result = ImageExtensions.ToImage(bitmap, false);
                }
            }
            return result;
        }

        public static BitmapImage GetInfraredImage(InfraredFrame frame)
        {
            BitmapImage result = null;
            if (frame != null)
            {
                using (frame)
                {
                    int width = frame.FrameDescription.Width;
                    int height = frame.FrameDescription.Height;
                    System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;

                    ushort[] infraredData = new ushort[frame.FrameDescription.LengthInPixels];
                    byte[] pixelData = new byte[frame.FrameDescription.LengthInPixels * 4];

                    frame.CopyFrameDataToArray(infraredData);

                    for (int infraredIndex = 0; infraredIndex < infraredData.Length; infraredIndex++)
                    {
                        ushort ir = infraredData[infraredIndex];
                        byte intensity = (byte)(ir >> 8);

                        pixelData[infraredIndex * 4] = intensity; // Blue
                        pixelData[infraredIndex * 4 + 1] = intensity; // Green   
                        pixelData[infraredIndex * 4 + 2] = intensity; // Red
                        pixelData[infraredIndex * 4 + 3] = 255;
                    }

                    var bitmap = new Bitmap(width, height, format);
                    var bitmapdata = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.WriteOnly,
                        bitmap.PixelFormat
                    );
                    IntPtr ptr = bitmapdata.Scan0;

                    Marshal.Copy(pixelData, 0, ptr, pixelData.Length);
                    bitmap.UnlockBits(bitmapdata);
                    result = ImageExtensions.ToImage(bitmap, false);
                }
            }
            return result;
        }
    }
}
