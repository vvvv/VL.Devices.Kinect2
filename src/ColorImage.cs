using Microsoft.Kinect;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Exposes the frame buffer directly
    public class ColorImage : IImage
    {
        public readonly ColorFrame frame;

        public ColorImage(ColorFrame frame)
        {
            this.frame = frame;
            Info = frame.FrameDescription.ToImageInfo(frame.RawColorImageFormat);
        }

        public ImageInfo Info { get; }

        public bool IsVolatile => true;

        public IImageData GetData()
        {
            return new KinectBufferImageData(frame.FrameDescription, frame.LockRawImageBuffer());
        }
    }
}
