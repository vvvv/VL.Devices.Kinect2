using Microsoft.Kinect;
using System.Linq;
using System.Runtime.CompilerServices;
using VL.Lib.Collections;
using Stride.Core.Mathematics;

namespace VL.Devices.Kinect2
{
    public static class TypeConverter
    {
        #region Unsafe Code
        private static void ToVector2(ref ColorSpacePoint input, out Vector2 output)
        {
            output = Unsafe.As<ColorSpacePoint, Vector2>(ref input);
        }

        private static void ToVector2(ref DepthSpacePoint input, out Vector2 output)
        {
            output = Unsafe.As<DepthSpacePoint, Vector2>(ref input);
        }

        private static void ToVector3(ref CameraSpacePoint input, out Vector3 output)
        {
            output = Unsafe.As<CameraSpacePoint, Vector3>(ref input);
        }

        private static void ToVector4(ref Microsoft.Kinect.Vector4 input, out Stride.Core.Mathematics.Vector4 output)
        {
            output = Unsafe.As<Microsoft.Kinect.Vector4, Stride.Core.Mathematics.Vector4>(ref input);
        }

        private static void ToQuaternion(ref Microsoft.Kinect.Vector4 input, out Quaternion output)
        {
            output = Unsafe.As<Microsoft.Kinect.Vector4, Quaternion>(ref input);
        }
        #endregion Unsafe Code


        private static readonly SpreadBuilder<Vector2> colorPointsBuilder = new SpreadBuilder<Vector2>();
        public static Spread<Vector2> ToVector2(ColorSpacePoint[] colorSpacePoints)
        {
            colorPointsBuilder.Clear();
            for (int i = 0; i < colorSpacePoints.Length; i++)
            {
                colorPointsBuilder.Add(ToVector2(colorSpacePoints[i]));
            }
            return colorPointsBuilder.ToSpread();
        }

        private static readonly SpreadBuilder<Vector2> depthPointsBuilder = new SpreadBuilder<Vector2>();
        public static Spread<Vector2> ToVector2(DepthSpacePoint[] depthSpacePoints)
        {
            depthPointsBuilder.Clear();
            for (int i = 0; i < depthSpacePoints.Length; i++)
            {
                depthPointsBuilder.Add(ToVector2(depthSpacePoints[i]));
            }
            return depthPointsBuilder.ToSpread();
        }

        private static readonly SpreadBuilder<Vector3> cameraPointsBuilder = new SpreadBuilder<Vector3>();
        public static Spread<Vector3> ToVector3(CameraSpacePoint[] cameraSpacePoints)
        {
            cameraPointsBuilder.Clear();
            for (int i = 0; i < cameraSpacePoints.Length; i++)
            {
                cameraPointsBuilder.Add(ToVector3(cameraSpacePoints[i]));
            }
            return cameraPointsBuilder.ToSpread();
        }

        /// <summary>
        /// Converts a Kinect DepthSpacePoint into a Xenko Vector2
        /// </summary>
        /// <param name="depthSpacePoint">Kinect DepthSpacePoint</param>
        /// <returns>Xenko Vector2</returns>
        public static Vector2 ToVector2(DepthSpacePoint depthSpacePoint)
        {
            Vector2 v2 = new Vector2();
            TypeConverter.ToVector2(ref depthSpacePoint, out v2);
            return v2;
        }

        /// <summary>
        /// Converts a Kinect ColorSpacePoint into a Xenko Vector2
        /// </summary>
        /// <param name="colorSpacePoint">Kinect ColorSpacePoint</param>
        /// <returns>Xenko Vector2</returns>
        public static Vector2 ToVector2(ColorSpacePoint colorSpacePoint)
        {
            Vector2 v2 = new Vector2();
            TypeConverter.ToVector2(ref colorSpacePoint, out v2);
            return v2;
        }

        /// <summary>
        /// Converts a Kinect CameraSpacePoint into a Xenko Vector3
        /// </summary>
        /// <param name="cameraSpacePoint">Kinect ColorSpacePoint</param>
        /// <returns>Xenko Vector3</returns>
        public static Vector3 ToVector3(CameraSpacePoint cameraSpacePoint)
        {
            Vector3 v3 = new Vector3();
            TypeConverter.ToVector3(ref cameraSpacePoint, out v3);
            return v3;
        }

        /// <summary>
        /// Converts a Kinect Vector4 into a Xenko Vector4
        /// </summary>
        /// <param name="vector4">Kinect Vector4</param>
        /// <returns>Xenko Vector4</returns>
        public static Stride.Core.Mathematics.Vector4 ToVector4(Microsoft.Kinect.Vector4 vector4)
        {
            Stride.Core.Mathematics.Vector4 v4 = new Stride.Core.Mathematics.Vector4();
            TypeConverter.ToVector4(ref vector4, out v4);
            return v4;
        }

        /// <summary>
        /// Converts a Kinect Vector4 into a Xenko Quaternion
        /// </summary>
        /// <param name="vector4">Kinect Vector4</param>
        /// <returns>Xenko Quaternion</returns>
        public static Quaternion ToQuaternion(Microsoft.Kinect.Vector4 vector4)
        {
            Quaternion v4 = new Quaternion();
            TypeConverter.ToQuaternion(ref vector4, out v4);
            return v4;
        }
    }
}
