using System;
using Microsoft.Kinect;
using Xenko.Core.Mathematics;
using VL.Lib.Collections;

namespace VL.Devices.Kinect2
{
    public static class PointCloud
    {
        //static CoordinateMapper FCoordinateMapper = new CoordinateMapper();

        public static unsafe Spread<Vector3>GetPoints(DepthImage image, float minZ, float maxZ, int decimation)
        {
            DepthFrame frame = image.frame;
            var depthSize = frame.FrameDescription.LengthInPixels;
            var depthData = new ushort[depthSize];
            var cameraSpacePoints = new CameraSpacePoint[depthSize];
            frame.CopyFrameDataToArray(depthData);
            frame.DepthFrameSource.KinectSensor.CoordinateMapper.MapDepthFrameToCameraSpace(depthData, cameraSpacePoints);


            var step = Math.Max(1, decimation+1);
            var width = image.Info.Width / step;
            var wRemainder = image.Info.Width % step;
            var height = image.Info.Height / step;
            var count = width * height;
            SpreadBuilder<Vector3> sb = new SpreadBuilder<Vector3>(count);
            //using (var buffer = frame.LockImageBuffer())
            {
                //ushort* ptr = (ushort*)buffer.UnderlyingBuffer.ToPointer();
                for (int row = 0; row < height; row++)
                {
                    //var rowStart = ptr;
                    var r = row * step * image.Info.Width;
                    for (int col = 0; col < width; col++)
                    {
                        var csp = cameraSpacePoints[r + col*step];
                        if(csp.Z > minZ && csp.Z < maxZ)
                            sb.Add(new Vector3(csp.X, csp.Y, csp.Z));
                        //ptr += step;
                    }
                    //ptr += wRemainder;
                    //ptr += image.Info.Width * (step-1);
                }
                return sb.ToSpread();
            }
        }
    }
}
