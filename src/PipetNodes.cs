
using Xenko.Core.Mathematics;
using VL.Lib.Collections;
using Microsoft.Kinect;

namespace VL.Devices.Kinect2
{
    internal static class PipetUtils
    {
        public static unsafe SpreadBuilder<int> GetValues(KinectBuffer buffer, SpreadBuilder<int> builder, int width, int height, Spread<Vector2> pixels)
        {
            builder.Clear();
            short* data = (short*)buffer.UnderlyingBuffer;

            int pixelX = 0;
            int pixelY = 0;

            foreach (var item in pixels)
            {
                pixelX = (int)item.X;
                pixelY = (int)item.Y;

                pixelX = pixelX < 0 ? 0 : pixelX;
                pixelY = pixelY < 0 ? 0 : pixelY;

                pixelX = pixelX > width - 1 ? width - 1 : pixelX;
                pixelY = pixelY > height - 1 ? height - 1 : pixelY;

                int pixel = data[pixelY * width + pixelX];
                builder.Add(pixel);
            }
            return builder;
        }

        public static unsafe SpreadBuilder<Color> GetValues(KinectBuffer buffer, SpreadBuilder<Color> builder, int width, int height, Spread<Vector2> pixels)
        {
            builder.Clear();
            short* data = (short*)buffer.UnderlyingBuffer;

            int pixelX = 0;
            int pixelY = 0;

            foreach (var item in pixels)
            {
                pixelX = (int)item.X;
                pixelY = (int)item.Y;

                pixelX = pixelX < 0 ? 0 : pixelX;
                pixelY = pixelY < 0 ? 0 : pixelY;

                pixelX = pixelX > width - 1 ? width - 1 : pixelX;
                pixelY = pixelY > height - 1 ? height - 1 : pixelY;

                int pixel = data[pixelY * width + pixelX];
                builder.Add(pixel);
            }
            return builder;
        }
    }

    public static class DepthPipet
    {
        private static readonly SpreadBuilder<int> builder = new SpreadBuilder<int>();
        public static unsafe Spread<int> GetValues(DepthImage image, Spread<Vector2> pixels)
        {

            using (var buffer = image.frame.LockImageBuffer())
            {
                return PipetUtils.GetValues(buffer, builder, image.Info.Width, image.Info.Height, pixels).ToSpread();
            }
        }
    }

    public static class InfraredPipet
    {
        private static readonly SpreadBuilder<int> builder = new SpreadBuilder<int>();
        public static unsafe Spread<int> GetValues(InfraredImage image, Spread<Vector2> pixels)
        {
            using (var buffer = image.frame.LockImageBuffer())
            {
                return PipetUtils.GetValues(buffer, builder, image.Info.Width, image.Info.Height, pixels).ToSpread();
            }
        }
    }

    public static class RGBPipet
    {
        private static readonly SpreadBuilder<Color> builder = new SpreadBuilder<Color>();
        public static unsafe Spread<Color> GetValues(ColorImage image, Spread<Vector2> pixels)
        {
            using (var buffer = image.frame.LockRawImageBuffer())
            {
                return PipetUtils.GetValues(buffer, builder, image.Info.Width, image.Info.Height, pixels).ToSpread();
            }
        }
    }
}
