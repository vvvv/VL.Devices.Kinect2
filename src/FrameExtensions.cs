using Microsoft.Kinect;
using System;
using VL.Lib.Basics.Imaging;
using VL.Lib.Collections;
using Xenko.Core.Mathematics;

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

        public static ColorDepthImage ToColorDepthImage(this DepthFrame frame, bool isRaw, bool relativeLookup) => new ColorDepthImage(frame, isRaw, relativeLookup);

        public static Spread<Vector3> ToPointCloud(this DepthImage image, float minZ, float maxZ, int decimation) => PointCloud.GetPoints(image, minZ, maxZ, decimation);

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
