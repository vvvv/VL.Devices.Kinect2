using System;
using Microsoft.Kinect;
using Xenko.Core.Mathematics;
using VL.Lib.Collections;

namespace VL.Devices.Kinect2
{
    public static class PointCloud
    {
        public static unsafe Spread<Vector3>GetPoints(DepthImage image, int minZ, int maxZ, int decimation)
        {
            DepthFrame frame = image.frame;
            var step = Math.Max(1, decimation+1);
            var width = image.Info.Width / step;
            var wRemainder = image.Info.Width % step;
            var height = image.Info.Height / step;
            var count = width * height;
            SpreadBuilder<Vector3> sb = new SpreadBuilder<Vector3>(count);
            using (var buffer = frame.LockImageBuffer())
            {
                ushort* ptr = (ushort*)buffer.UnderlyingBuffer.ToPointer();
                for (int row = 0; row < height; row++)
                {
                    var rowStart = ptr;
                    for (int col = 0; col < width; col++)
                    {
                        ushort z = *ptr;
                        if(z > minZ && z< maxZ)
                            sb.Add(new Vector3(col * step, row * step, z));
                        ptr += step;
                    }
                    ptr += wRemainder;
                    ptr += image.Info.Width * (step-1);
                }
                return sb.ToSpread();
            }
        }
    }
}
