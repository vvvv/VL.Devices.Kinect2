using System;
using Microsoft.Kinect;
using Stride.Core.Mathematics;
using VL.Lib.Collections;
using System.Buffers;

namespace VL.Devices.Kinect2
{
    public static class PointCloud
    {
        public static unsafe SpreadBuilder<Vector3> CollectPoints(
            SpreadBuilder<Vector3> builder,
            DepthImage image,
            float minZ,
            float maxZ,
            int decimation = 2)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            if (image is null)
                throw new ArgumentNullException(nameof(image));

            var frame = image.frame;
            var pixelCount = frame.FrameDescription.LengthInPixels;

            using (var depthBuffer = frame.LockImageBuffer())
            using (var cameraSpacePoints = MemoryPool<CameraSpacePoint>.Shared.Rent((int)pixelCount))
            {
                using (var cameraSpacePointsHandle = cameraSpacePoints.Memory.Pin())
                {
                    var cameraSpacePointsPtr = new IntPtr(cameraSpacePointsHandle.Pointer);

                    frame.DepthFrameSource.KinectSensor.CoordinateMapper.MapDepthFrameToCameraSpaceUsingIntPtr(
                        depthBuffer.UnderlyingBuffer, depthBuffer.Size,
                        cameraSpacePointsPtr, (uint)(pixelCount * sizeof(CameraSpacePoint)));
                }

                var step = Math.Max(1, decimation);
                var width = image.Info.Width;
                var height = image.Info.Height;

                builder.Clear();

                // The memory we got from the pool might be bigger than what we need. So let's slice it down to our expected size so we get index out of bounds checks.
                var csps = cameraSpacePoints.Memory.Span.Slice(0, (int)pixelCount);
                for (int y = 0; y < height; y += step)
                {
                    var r = y * width;
                    for (int x = 0; x < width; x += step)
                    {
                        var csp = csps[r + x];
                        if (csp.Z > minZ && csp.Z < maxZ)
                            builder.Add(new Vector3(csp.X, csp.Y, csp.Z));
                    }
                }
                return builder;
            }
        }
    }
}
