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

        // Tuple builder version - compatible with Observable pattern and HoldLatestCopy
        public static unsafe SpreadBuilder<(Vector3 position, Vector2 uvColor, Vector2 uvDepth)> CollectPointsWithUV(
            SpreadBuilder<(Vector3 position, Vector2 uvColor, Vector2 uvDepth)> builder,
            DepthImage depth,
            float minZ,
            float maxZ,
            int decimation = 2)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));
            if (depth is null)
                throw new ArgumentNullException(nameof(depth));

            var frame = depth.frame;
            var pixelCount = frame.FrameDescription.LengthInPixels;

            // Get color frame dimensions dynamically
            var sensor = frame.DepthFrameSource.KinectSensor;
            var colorFrameDesc = sensor.ColorFrameSource.FrameDescription;
            int colorW = colorFrameDesc.Width;
            int colorH = colorFrameDesc.Height;
            var coordinateMapper = sensor.CoordinateMapper;
            float invColorW = 1f / colorW;
            float invColorH = 1f / colorH;

            using (var depthBuffer = frame.LockImageBuffer())
            using (var cameraSpacePoints = MemoryPool<CameraSpacePoint>.Shared.Rent((int)pixelCount))
            using (var colorSpacePoints = MemoryPool<ColorSpacePoint>.Shared.Rent((int)pixelCount))
            {
                // Map to camera space
                using (var cameraSpacePointsHandle = cameraSpacePoints.Memory.Pin())
                {
                    var cameraSpacePointsPtr = new IntPtr(cameraSpacePointsHandle.Pointer);

                    coordinateMapper.MapDepthFrameToCameraSpaceUsingIntPtr(
                        depthBuffer.UnderlyingBuffer, depthBuffer.Size,
                        cameraSpacePointsPtr, (uint)(pixelCount * sizeof(CameraSpacePoint)));
                }

                // Map to color space
                using (var colorSpacePointsHandle = colorSpacePoints.Memory.Pin())
                {
                    var colorSpacePointsPtr = new IntPtr(colorSpacePointsHandle.Pointer);

                    coordinateMapper.MapDepthFrameToColorSpaceUsingIntPtr(
                        depthBuffer.UnderlyingBuffer, depthBuffer.Size,
                        colorSpacePointsPtr, (uint)(pixelCount * sizeof(ColorSpacePoint)));
                }

                var step = Math.Max(1, decimation);
                var width = depth.Info.Width;
                var height = depth.Info.Height;
                float invDepthW = 1f / width;
                float invDepthH = 1f / height;

                builder.Clear();

                // The memory we got from the pool might be bigger than what we need. So let's slice it down to our expected size so we get index out of bounds checks.
                var camPoints = cameraSpacePoints.Memory.Span.Slice(0, (int)pixelCount);
                var colPoints = colorSpacePoints.Memory.Span.Slice(0, (int)pixelCount);

                for (int y = 0; y < height; y += step)
                {
                    var r = y * width;
                    for (int x = 0; x < width; x += step)
                    {
                        var i = r + x;
                        var cam = camPoints[i];
                        
                        // Filter by Z range
                        if (cam.Z <= minZ || cam.Z >= maxZ)
                            continue;

                        var col = colPoints[i];
                        var colX = col.X;
                        var colY = col.Y;

                        // Validate ColorSpacePoint; this also rejects NaN/Infinity
                        if (!(colX >= 0f && colX < colorW && colY >= 0f && colY < colorH))
                        {
                            // Drop point entirely if UV is invalid (maintain synchronization)
                            continue;
                        }

                        // Add position and UVs as tuple (normalized UV coordinates 0..1)
                        var position = new Vector3(cam.X, cam.Y, cam.Z);
                        var uvColor = new Vector2(colX * invColorW, colY * invColorH);
                        var uvDepth = new Vector2(x * invDepthW, y * invDepthH);
                        builder.Add((position, uvColor, uvDepth));
                    }
                }

                return builder;
            }
        }

    }
}
