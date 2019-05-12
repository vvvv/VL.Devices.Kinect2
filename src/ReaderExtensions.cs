using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using VL.Lib.Basics.Imaging;

namespace VL.Devices.Kinect2
{
    // Not really sure about those would need more insight on how the final API (nodes) should look like
    public class ReaderExtensions
    {
        public static IEnumerable<IImage> GetImages(ColorFrameReader reader, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using (var frame = reader.AcquireLatestFrame())
                {
                    // Give control to consumer
                    yield return frame.ToColorImage();
                    // Consumer read the image, we can now dispose the frame given by kinect
                }
            }
        }

        public static IObservable<IImage> GetImageStream(ColorFrameReader reader)
        {
            // Could work but also not sure, would need to test
            return Observable.FromEventPattern<ColorFrameArrivedEventArgs>(reader, nameof(ColorFrameReader.FrameArrived))
                .SelectMany(e => Observable.Using(e.EventArgs.FrameReference.AcquireFrame, f => new BehaviorSubject<IImage>(f.ToColorImage())));
        }
    }
}
