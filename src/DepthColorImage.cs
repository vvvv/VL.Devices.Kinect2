using Microsoft.Kinect;
using System;
using System.Runtime.InteropServices;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Converts the frame to target format when consumer reads the data
    // Uses system provided memory pool
    class DepthColorImage : IImage
    {
        class ImageData : IImageData
        {
            public ImageData(DepthColorImage image)
            {
                int depthSize = 512 * 424 * 2;
                depthPointer = Marshal.AllocHGlobal(depthSize);

                ScanSize = 1920 * 8;
                Size = 1920 * 1080 * 8;
                Pointer = Marshal.AllocHGlobal(Size);
                
                image.frame.CopyFrameDataToIntPtr(depthPointer, 512 * 424 * 2);
                image.frame.DepthFrameSource.KinectSensor.CoordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(depthPointer, (uint)depthSize, Pointer, (uint)Size);
                
            }
            private IntPtr depthPointer;
            public IntPtr Pointer { get; }

            public int Size { get; }

            public int ScanSize { get; }

            public void Dispose()
            {
                Marshal.FreeHGlobal(Pointer);
                Marshal.FreeHGlobal(depthPointer);
            }
        }

        readonly DepthFrame frame;

        public DepthColorImage(DepthFrame frame)
        {
            this.frame = frame;
            Info = new ImageInfo(1920, 1080, PixelFormat.R32G32F);//Format needs to be R32G32F, awaiting support in VL.Lib.Basics.Imaging
        }

        public ImageInfo Info { get; }

        public bool IsVolatile => true;

        public IImageData GetData() => new ImageData(this);
    }
}
