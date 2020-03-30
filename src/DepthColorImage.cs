using Microsoft.Kinect;
using System;
using System.Buffers;
using System.Runtime.InteropServices;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Converts the frame to target format when consumer reads the data
    // Uses system provided memory pool
    // TODO: Test code snippet below using memory pool
    public class DepthColorImage : IImage
    {
        class ImageData : MemoryManager<byte>, IImageData
        {
            public ImageData(DepthColorImage image)
            {
                int depthSize = 512 * 424 * 2;
                depthPointer = Marshal.AllocHGlobal(depthSize);

                ScanSize = 1920 * 8;
                Size = 1920 * 1080 * 8;
                Pointer = Marshal.AllocHGlobal(Size);
                
                image.frame.CopyFrameDataToIntPtr(depthPointer, 512 * 424 * 2);
                image.frame.DepthFrameSource.KinectSensor.CoordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(depthPointer, (uint)depthSize, Pointer, (uint)Size);
                
            }
            private IntPtr depthPointer;
            public IntPtr Pointer { get; }

            public int Size { get; }

            public int ScanSize { get; }

            public ReadOnlyMemory<byte> Bytes => Memory;

            public override unsafe Span<byte> GetSpan()
            {
                return new Span<byte>(Pointer.ToPointer(), Size);
            }

            public override unsafe MemoryHandle Pin(int elementIndex = 0)
            {
                return new MemoryHandle(Pointer.ToPointer(), pinnable: this);
            }

            public override void Unpin()
            {
            }

            protected override void Dispose(bool disposing)
            {
                Marshal.FreeHGlobal(Pointer);
                Marshal.FreeHGlobal(depthPointer);
            }
        }

        // Following code should work but couldn't test it yet
        /*
        unsafe class ImageData : MemoryManager<byte>, IImageData
        {
            private readonly IMemoryOwner<DepthSpacePoint> depthSpacePointsOwner;
            private readonly Memory<DepthSpacePoint> depthSpacePoints;

            public ImageData(DepthColorImage image)
            {
                var depthPixelCount = image.frame.FrameDescription.Width * image.frame.FrameDescription.Height;
                var colorPixelCount = image.Info.Width * image.Info.Height;

                depthSpacePointsOwner = MemoryPool<DepthSpacePoint>.Shared.Rent(colorPixelCount);
                depthSpacePoints = depthSpacePointsOwner.Memory.Slice(0, colorPixelCount);
                ScanSize = image.Info.ScanSize;

                using (var depthOwner = MemoryPool<ushort>.Shared.Rent(depthPixelCount))
                using (var depthHandle = depthOwner.Memory.Pin())
                using (var depthSpacePointsHandle = depthSpacePointsOwner.Memory.Pin())
                {
                    var depthPointer = new IntPtr(depthHandle.Pointer);
                    var depthSize = depthPixelCount * sizeof(ushort);
                    var depthSpacePointsPointer = new IntPtr(depthSpacePointsHandle.Pointer);
                    var depthSpacePointsSize = colorPixelCount * sizeof(DepthSpacePoint);
                    image.frame.CopyFrameDataToIntPtr(depthPointer, (uint)depthSize);
                    image.frame.DepthFrameSource.KinectSensor.CoordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(depthPointer, (uint)depthSize, depthSpacePointsPointer, (uint)depthSpacePointsSize);
                }
            }

            public ReadOnlyMemory<byte> Bytes => Memory;

            public int ScanSize { get; }

            public void Dispose()
            {
                depthSpacePointsOwner.Dispose();
            }

            public override Span<byte> GetSpan()
            {
                return MemoryMarshal.AsBytes(depthSpacePoints.Span);
            }

            public override MemoryHandle Pin(int elementIndex = 0)
            {
                return depthSpacePoints.Pin();
            }

            public override void Unpin()
            {
            }

            protected override void Dispose(bool disposing)
            {
                depthSpacePointsOwner.Dispose();
            }
        }
        */

        public readonly DepthFrame frame;

        public DepthColorImage(DepthFrame frame)
        {
            this.frame = frame;
            Info = new ImageInfo(1920, 1080, PixelFormat.R32G32F);//Format needs to be R32G32F, awaiting support in VL.Lib.Basics.Imaging
        }

        public ImageInfo Info { get; }

        public bool IsVolatile => true;

        public IImageData GetData() => new ImageData(this);
    }
}
