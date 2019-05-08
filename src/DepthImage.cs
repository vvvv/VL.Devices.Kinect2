using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Exposes the kinect frame buffer directly
    class DepthImage : IImage
    {
        readonly DepthFrame frame;

        public DepthImage(DepthFrame frame)
        {
            this.frame = frame;
            // TODO: Not sure what the format is exactly
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
