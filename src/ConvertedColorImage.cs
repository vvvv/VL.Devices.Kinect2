using Microsoft.Kinect;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Converts the frame to target format when consumer reads the data
    // Uses system provided memory pool
    class ConvertedColorImage : IImage
    {
        class ImageData : IImageData
        {
            public ImageData(ConvertedColorImage image)
            {
                var info = image.Info;
                ScanSize = info.ScanSize;
                Size = info.ImageSize;
                Pointer = Marshal.AllocHGlobal(Size);
                image.frame.CopyConvertedFrameDataToIntPtr(Pointer, (uint)Size, image.colorImageFormat);
            }

            public IntPtr Pointer { get; }

            public int Size { get; }

            public int ScanSize { get; }

            public void Dispose()
            {
                Marshal.FreeHGlobal(Pointer);
            }
        }

        readonly ColorFrame frame;
        readonly ColorImageFormat colorImageFormat;

        public ConvertedColorImage(ColorFrame frame, ColorImageFormat colorImageFormat)
        {
            this.frame = frame;
            this.colorImageFormat = colorImageFormat;
            Info = frame.FrameDescription.ToImageInfo(colorImageFormat);
        }

        public ImageInfo Info { get; }

        public bool IsVolatile => true;

        public IImageData GetData() => new ImageData(this);
    }
}
