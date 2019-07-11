using Microsoft.Kinect;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Exposes the kinect frame buffer directly
    public class DepthImage : IImage
    {
        public readonly DepthFrame frame;

        public DepthImage(DepthFrame frame)
        {
            this.frame = frame;
            Info = frame.FrameDescription.ToImageInfo(PixelFormat.R16);
        }

        public ImageInfo Info { get; }

        public bool IsVolatile => true;

        public IImageData GetData()
        {
            return new KinectBufferImageData(frame.FrameDescription, frame.LockImageBuffer());
        }
    }
}
