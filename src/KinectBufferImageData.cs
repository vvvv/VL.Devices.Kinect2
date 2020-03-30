using Microsoft.Kinect;
using System;
using System.Buffers;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    unsafe class KinectBufferImageData : MemoryManager<byte>, IImageData
    {
        readonly KinectBuffer buffer;

        public KinectBufferImageData(FrameDescription frameDescription, KinectBuffer buffer)
        {
            this.buffer = buffer;
            ScanSize = (int)frameDescription.BytesPerPixel * frameDescription.Width;
        }

        public ReadOnlyMemory<byte> Bytes => Memory;

        public int ScanSize { get; }

        public override Span<byte> GetSpan()
        {
            return new Span<byte>(buffer.UnderlyingBuffer.ToPointer(), (int)buffer.Size);
        }

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            // Already pinned
            return new MemoryHandle(buffer.UnderlyingBuffer.ToPointer(), pinnable: this);
        }

        public override void Unpin()
        {
        }

        protected override void Dispose(bool disposing)
        {
            buffer.Dispose();
        }
    }
}
