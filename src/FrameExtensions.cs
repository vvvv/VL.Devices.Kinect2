using Microsoft.Kinect;
using System;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Keep all implementing classes internal and just expose them publicly through interface
    public static class FrameExtensions
    {
        public static IImage ToColorImage(this ColorFrame frame) => new ColorImage(frame);

        public static IImage ToConvertedColorImage(this ColorFrame frame, ColorImageFormat colorImageFormat) => new ConvertedColorImage(frame, colorImageFormat);

        public static IImage ToDepthImage(this DepthFrame frame) => new DepthImage(frame);

        public static IImage ToInfraredImage(this InfraredFrame frame) => new InfraredImage(frame);

        public static IImage ToPlayerImage(this BodyIndexFrame frame) => new PlayerImage(frame);

        public static IImage ToDepthColorImage(this DepthFrame frame) => new DepthColorImage(frame);

        public static IImage ToColorDepthImage(this DepthFrame frame, bool isRaw, bool relativeLookup) => new ColorDepthImage(frame, isRaw, relativeLookup);

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
