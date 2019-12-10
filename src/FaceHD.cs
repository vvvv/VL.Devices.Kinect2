using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using VL.Lib.Collections;
using Xenko.Core.Mathematics;

namespace VL.Devices.Kinect2
{
    public class FaceHDData
    {
        private ColorSpacePoint[] _colorSpacePoints;

        private DepthSpacePoint[] _depthSpacePoints;

        public int TrackingId { get; }

        public Quaternion FaceOrientation { get; }

        public Vector3 HeadPivotPoint { get; }

        public FaceAlignmentQuality FaceAlignmentQuality { get;  }

        public IReadOnlyList<CameraSpacePoint> NativeCameraSpacePoints { get; }

        public Spread<Vector2> RGBSpacePoints { 
            get {
                if (Sensor != null)
                {
                    if (_colorSpacePoints == null)
                        _colorSpacePoints = new ColorSpacePoint[NativeCameraSpacePoints.Count];
                    Sensor.CoordinateMapper.MapCameraPointsToColorSpace(NativeCameraSpacePoints.ToArray(), _colorSpacePoints);
                }
                return TypeConverter.ToVector2(_colorSpacePoints);
            }  
        }

        public Spread<Vector2> DepthSpacePoints
        {
            get
            {
                if (Sensor != null)
                {
                    if (_depthSpacePoints == null)
                        _depthSpacePoints = new DepthSpacePoint[NativeCameraSpacePoints.Count];
                    Sensor.CoordinateMapper.MapCameraPointsToDepthSpace(NativeCameraSpacePoints.ToArray(), _depthSpacePoints);
                }
                return TypeConverter.ToVector2(_depthSpacePoints);
            }
        }

        public Spread<Vector3> CameraSpacePoints
        {
            get
            {
                return TypeConverter.ToVector3(NativeCameraSpacePoints.ToArray());
            }
        }

        public KinectSensor Sensor { get; }

        public FaceHDData(IReadOnlyList<CameraSpacePoint> cameraSpacePoints, Microsoft.Kinect.Vector4 faceOrientation, CameraSpacePoint headPivotPoint, FaceAlignmentQuality quality, KinectSensor sensor)
        {
            NativeCameraSpacePoints = cameraSpacePoints;
            FaceOrientation = TypeConverter.ToQuaternion(faceOrientation);
            HeadPivotPoint = TypeConverter.ToVector3(headPivotPoint);
            FaceAlignmentQuality = quality;
            Sensor = sensor;
        }
    }
    public class FaceHD : IDisposable
    {
        private KinectSensor _sensor = null;

        private BodyFrameSource _bodySource = null;

        private BodyFrameReader _bodyReader = null;

        private HighDefinitionFaceFrameSource _faceSource = null;

        private HighDefinitionFaceFrameReader _faceReader = null;

        private FaceAlignment _faceAlignment = null;

        private FaceModel _faceModel = null;

        private Subject<FaceHDData> faceHDData;

        private FaceHDData _faceHDData;

        public IObservable<FaceHDData> FaceHDData { get { return faceHDData; } }

        public FaceHD(KinectSensor sensor)
        {
            _sensor = sensor;
            faceHDData = new Subject<FaceHDData>();

            if (_sensor != null)
            {
                _bodySource = _sensor.BodyFrameSource;
                _bodyReader = _bodySource.OpenReader();
                _bodyReader.FrameArrived += BodyReader_FrameArrived;

                _faceSource = new HighDefinitionFaceFrameSource(_sensor);

                _faceReader = _faceSource.OpenReader();
                _faceReader.FrameArrived += FaceReader_FrameArrived;

                _faceModel = new FaceModel();
                _faceAlignment = new FaceAlignment();
            }
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Body[] bodies = new Body[frame.BodyCount];
                    frame.GetAndRefreshBodyData(bodies);

                    Body body = bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (!_faceSource.IsTrackingIdValid)
                    {
                        if (body != null)
                        {
                            _faceSource.TrackingId = body.TrackingId;
                        }
                    }
                }
            }
        }

        private void FaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null && frame.IsFaceTracked)
                {
                    frame.GetAndRefreshFaceAlignmentResult(_faceAlignment);
                    UpdateFaceData();
                }
            }
        }

        private void UpdateFaceData()
        {
            if (_faceModel == null) return;
            
            var vertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);
            if (vertices.Count > 0)
            {
                _faceHDData = new FaceHDData(vertices, _faceAlignment.FaceOrientation, _faceAlignment.HeadPivotPoint, _faceAlignment.Quality, _sensor);
                faceHDData.OnNext(_faceHDData);
            }
        }

        public void Dispose()
        {
            if (_faceModel != null)
            {
                _faceModel.Dispose();
                _faceModel = null;
            }
        }
    }


}
