using Microsoft.Kinect;
using System;
using VL.Lib.Basics.Imaging;
using VL.Lib.Collections;
using Stride.Core.Mathematics;
using System.Runtime.InteropServices;

namespace VL.Devices.Kinect2
{
    // Keep all implementing classes internal and just expose them publicly through interface
    public static class FrameExtensions
    {
        public static ColorImage ToColorImage(this ColorFrame frame) => new ColorImage(frame);

        public static ConvertedColorImage ToConvertedColorImage(this ColorFrame frame, ColorImageFormat colorImageFormat) => new ConvertedColorImage(frame, colorImageFormat);

        public static DepthImage ToDepthImage(this DepthFrame frame) => new DepthImage(frame);

        public static InfraredImage ToInfraredImage(this InfraredFrame frame) => new InfraredImage(frame);

        public static PlayerImage ToPlayerImage(this BodyIndexFrame frame) => new PlayerImage(frame);

        public static DepthColorImage ToDepthColorImage(this DepthFrame frame) => new DepthColorImage(frame);

        static CameraSpacePoint[] cameraSpacePoints = new CameraSpacePoint[512 * 424];
        static Stride.Core.Mathematics.Vector4[] worldWrite = new Stride.Core.Mathematics.Vector4[512 * 424];
        public static ArrayImage<Stride.Core.Mathematics.Vector4> ToWorldImage(this DepthFrame frame)
        {
            //frame.CopyFrameDataToIntPtr(depthPointer, 512 * 424 * 2);
            using var x = frame.LockImageBuffer();
            frame.DepthFrameSource.KinectSensor.CoordinateMapper.MapDepthFrameToCameraSpaceUsingIntPtr(x.UnderlyingBuffer, x.Size, cameraSpacePoints);

            for (int i = 0; i < cameraSpacePoints.Length; i++)
            {
                worldWrite[i].X = cameraSpacePoints[i].X;
                worldWrite[i].Y = cameraSpacePoints[i].Y;
                worldWrite[i].Z = cameraSpacePoints[i].Z;
                worldWrite[i].W = 1;
            }

            return new ArrayImage<Stride.Core.Mathematics.Vector4>(worldWrite, new ImageInfo(512, 424, PixelFormat.R32G32B32A32F), true);
         }

        public static ColorDepthImage ToColorDepthImage(this DepthFrame frame, bool isRaw, bool relativeLookup) => new ColorDepthImage(frame, isRaw, relativeLookup);

        public static Spread<int> ToDepthPipetValues(this DepthImage image, Spread<Vector2> pixels) => DepthPipet.GetValues(image, pixels);

        public static Spread<int> ToInfraredPipetValues(this InfraredImage image, Spread<Vector2> pixels) => InfraredPipet.GetValues(image, pixels);

        public static ImageInfo ToImageInfo(this FrameDescription frameDescription, ColorImageFormat colorImageFormat)
        {
            return new ImageInfo(
                width: frameDescription.Width,
                height: frameDescription.Height,
                format: colorImageFormat.ToPixelFormat(),
                originalFormat: colorImageFormat.ToString());
        }

        public static ImageInfo ToImageInfo(this FrameDescription frameDescription, PixelFormat pixelFormat)
        {
            return new ImageInfo(
                width: frameDescription.Width,
                height: frameDescription.Height,
                format: pixelFormat,
                originalFormat: pixelFormat.ToString());
        }

        public static PixelFormat ToPixelFormat(this ColorImageFormat format)
        {
            switch (format)
            {
                case ColorImageFormat.None:
                    return PixelFormat.Unknown;
                case ColorImageFormat.Rgba:
                    return PixelFormat.R8G8B8A8;
                case ColorImageFormat.Bgra:
                    return PixelFormat.B8G8R8A8;
                case ColorImageFormat.Yuv:
                case ColorImageFormat.Bayer:
                case ColorImageFormat.Yuy2:
                    throw new NotSupportedException(); // TODO: Make me nicer
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
