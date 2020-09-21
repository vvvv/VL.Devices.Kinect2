using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using VL.Lib.Collections;
using Stride.Core.Mathematics;

namespace VL.Devices.Kinect2
{
    public class FaceHD
    {
        public ulong ID { get; }

        public Quaternion FaceOrientation { get; }

        public Vector3 HeadPivotPoint { get; }

        public FaceAlignmentQuality FaceAlignmentQuality { get;  }

        public CameraSpacePoint[] NativeCameraSpacePoints { get; }

        public Spread<Vector3> WorldSpacePoints
        {
            get
            {
                return TypeConverter.ToVector3(NativeCameraSpacePoints.ToArray());
            }
        }

        internal FaceHD(IReadOnlyList<CameraSpacePoint> cameraSpacePoints, FaceAlignment faceAlignment, ulong ID)
        {
            NativeCameraSpacePoints = cameraSpacePoints.ToArray();
            FaceOrientation = TypeConverter.ToQuaternion(faceAlignment.FaceOrientation);
            HeadPivotPoint = TypeConverter.ToVector3(faceAlignment.HeadPivotPoint);
            FaceAlignmentQuality = faceAlignment.Quality;
            this.ID = ID;
        }
    }
    public class FaceHDTracker : IDisposable
    {
        private FaceHD defaultValue = new FaceHD(new CameraSpacePoint[0], new FaceAlignment(), 0);
        private KinectSensor _sensor = null;

        private BodyFrameSource _bodySource = null;

        private BodyFrameReader _bodyReader = null;

        private HighDefinitionFaceFrameSource _faceSource = null;

        private HighDefinitionFaceFrameReader _faceReader = null;

        private FaceAlignment _faceAlignment = null;

        private FaceModel _faceModel = null;

        private Subject<FaceHD> faceHDData;

        private FaceHD _faceHDData;

        public IObservable<FaceHD> FaceHD { get { return faceHDData; } }

        public FaceHDTracker(KinectSensor sensor)
        {
            _sensor = sensor;
            faceHDData = new Subject<FaceHD>();

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

                    //TODO: Use a parametrized Body.TrackingID to choose/set the TrackingId
                    Body body = bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (!_faceSource.IsTrackingIdValid)
                    {
                        if (body != null)
                        {
                            _faceSource.TrackingId = body.TrackingId;
                        }
                        faceHDData.OnNext(defaultValue);
                    }
                    else
                    {
                        UpdateFaceData();
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
                    //UpdateFaceData();
                }
            }
        }

        private void UpdateFaceData()
        {
            if (_faceModel == null || !_faceSource.IsOnline) return;
            
            var vertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);
            _faceHDData = new FaceHD(vertices, _faceAlignment, _faceSource.TrackingId);
            faceHDData.OnNext(_faceHDData);
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
