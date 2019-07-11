using Microsoft.Kinect;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Exposes the kinect frame buffer directly
    public class PlayerImage : IImage
    {
        public readonly BodyIndexFrame frame;

        public PlayerImage(BodyIndexFrame frame)
        {
            this.frame = frame;
            // TODO: Not sure what the format is exactly
            Info = frame.FrameDescription.ToImageInfo(PixelFormat.R8);
        }

        public ImageInfo Info { get; }

        public bool IsVolatile => true;

        public IImageData GetData()
        {
            return new KinectBufferImageData(frame.FrameDescription, frame.LockImageBuffer());
        }
    }
}
