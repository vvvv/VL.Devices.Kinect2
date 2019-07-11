using Microsoft.Kinect;
using SharpDX;
using System;
using VL.Lib.Collections;

namespace VL.Devices.Kinect2
{
    public static class PointCloud
    {
        public static unsafe Spread<Vector3>GetPoints(DepthImage image, float scale, float zScale, int minZ, int maxZ)
        {
            DepthFrame frame = image.frame;
            var width = image.Info.Width;
            var height = image.Info.Height;
            SpreadBuilder<Vector3> sb = new SpreadBuilder<Vector3>((int) (width * height * scale));
            using (var buffer = frame.LockImageBuffer())
            {
                ushort* ptr = (ushort*)buffer.UnderlyingBuffer.ToPointer();
                int step = (int)(1 / scale);
                for (int row = 0; row < image.Info.Height; row += step)
                {
                    for (int col = 0; col < image.Info.Width; col += step)
                    {
                        ushort z = *ptr;
                        if(z > minZ && z< maxZ)
                            sb.Add(new Vector3(col * scale, row * scale, z * zScale));
                        ptr += step;
                    }
                }
                return sb.ToSpread();
            }
        }
    }
}
