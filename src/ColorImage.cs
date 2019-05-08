using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Exposes the frame buffer directly
    class ColorImage : IImage
    {
        readonly ColorFrame frame;

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
