using Microsoft.Kinect;
using System;
using System.Runtime.InteropServices;
using VL.Lib.Basics.Imaging;
using VL.Core;

namespace VL.Devices.Kinect2
{
    // Converts the frame to target format when consumer reads the data
    // Uses system provided memory pool
    class ColorDepthImage : IImage
    {
        class ImageData : IImageData
        {
            public unsafe ImageData(ColorDepthImage image)
            {
                int depthSize = 512 * 424 * 2;
                depthPointer = Marshal.AllocHGlobal(depthSize);

                int colorSize = 512 * 424 * 8;
                colorPointer = Marshal.AllocHGlobal(colorSize);

                ScanSize = 512 * 8;
                Size = 512 * 424 * 8;
                convertedColorPointer = Marshal.AllocHGlobal(Size);

                //image.frame.CopyFrameDataToIntPtr(depthPointer, 512 * 424 * 2);
                //image.frame.DepthFrameSource.KinectSensor.CoordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(depthPointer, (uint)depthSize, Pointer, (uint)Size);


                image.frame.CopyFrameDataToIntPtr(depthPointer, 512 * 424 * 2);
                image.frame.DepthFrameSource.KinectSensor.CoordinateMapper.MapDepthFrameToColorSpaceUsingIntPtr(depthPointer, 512 * 424 * 2, colorPointer, 512 * 424 * 8);

                if (!image.isRaw)
                {
                    float* col = (float*)colorPointer;
                    float* conv = (float*)convertedColorPointer;
                    if (image.relativeLookup)
                    {
                        for (int i = 0; i < 512 * 424; i++)
                        {
                            conv[i * 2] = (float)VLMath.Map(col[i * 2] - i % 1920, 0, 1920, 0, 1, MapMode.Float);
                            conv[i * 2 + 1] = (float)VLMath.Map(col[i * 2 + 1] - VLMath.Abs(i / 1920), 0, 1080, 0, 1, MapMode.Float);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 512 * 424; i++)
                        {
                            conv[i * 2] = (float)VLMath.Map(col[i * 2], 0, 1920, 0, 1, MapMode.Clamp);
                            conv[i * 2 + 1] = (float)VLMath.Map(col[i * 2 + 1], 0, 1080, 0, 1, MapMode.Clamp);
                        }
                    }
                    Pointer = convertedColorPointer;
                }
                else
                {
                    Pointer = colorPointer;
                }
            }
            private IntPtr depthPointer;
            private IntPtr colorPointer;
            private IntPtr convertedColorPointer;

            public IntPtr Pointer { get; }

            public int Size { get; }

            public int ScanSize { get; }

            public void Dispose()
            {
                Marshal.FreeHGlobal(depthPointer);
                Marshal.FreeHGlobal(colorPointer);
                Marshal.FreeHGlobal(convertedColorPointer);
                //Marshal.FreeHGlobal(Pointer);
            }
        }

        readonly DepthFrame frame;
        readonly bool isRaw, relativeLookup;

        public ColorDepthImage(DepthFrame frame, bool isRaw, bool relativeLookup)
        {
            this.frame = frame;
            this.isRaw = isRaw;
            this.relativeLookup = relativeLookup;
            Info = new ImageInfo(512, 424, PixelFormat.R32F);//Format needs to be R32G32F, awaiting support in VL.Lib.Basics.Imaging
        }

        public ImageInfo Info { get; }

        public bool IsVolatile => true;

        public IImageData GetData() => new ImageData(this);
    }
}
