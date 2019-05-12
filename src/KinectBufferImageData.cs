using Microsoft.Kinect;
using System;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    class KinectBufferImageData : IImageData
    {
        readonly KinectBuffer buffer;

        public KinectBufferImageData(FrameDescription frameDescription, KinectBuffer buffer)
        {
            this.buffer = buffer;
            ScanSize = (int)frameDescription.BytesPerPixel * frameDescription.Width;
        }

        public IntPtr Pointer => buffer.UnderlyingBuffer;

        public int Size => (int)buffer.Size;

        public int ScanSize { get; }

        public void Dispose() => buffer.Dispose();
    }
}
