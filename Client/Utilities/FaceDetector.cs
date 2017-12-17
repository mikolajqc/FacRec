using System.Drawing;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Tracking;

namespace Client.Utilities
{
    //TODO: ogarnac tak zeby sama detekcja robila sie na niskiej rozdzielczosci zdjeciu. A wycinka szla z odpowiednio wyskalowanego zdjecia wysokiej jakosci, cos na zasadzie podgladu
    //TODO: Napisz polaczenie detekcji Harra i trackingu - co np 5s detekcja harra a potem tracking tej twarzy!!! 
    //TODO: przenies wszystkie stale do configuracji
    class FaceDetector
    {
        private HaarObjectDetector _detector;
        private RectanglesMarker _marker;
        private Camshift _tracker;

        private bool _isTracking;
        private bool _isDetecting;

        public FaceDetector()
        {
            HaarCascade cascade = new FaceHaarCascade();
            _detector = new HaarObjectDetector(cascade,
                25, ObjectDetectorSearchMode.Single, 1.2f,
                ObjectDetectorScalingMode.GreaterToSmaller);
            _tracker = new Camshift();
            _isTracking = false;
            _isDetecting = true;
        }

        public Bitmap GetBitmapWithDetectedFace(Bitmap sourceBitmap)
        {
            Bitmap result = null;

            if (_isDetecting)
            {
                UnmanagedImage im = UnmanagedImage.FromManagedImage(sourceBitmap);
                Rectangle[] regions = _detector.ProcessFrame(im);

                if (regions.Length > 0)
                {
                    _isDetecting = false;
                    _tracker.Reset();

                    // Reduce the face size to avoid tracking background
                    Rectangle[] window = {new Rectangle(
                        (int)((regions[0].X + regions[0].Width / 2f) ),
                        (int)((regions[0].Y + regions[0].Height / 2f)),
                        1, 1)};

                     window[0].Inflate(
                        (int)(0.25f * regions[0].Width),
                        (int)(0.3f * regions[0].Height));

                    _tracker.SearchWindow = window[0];
                    _tracker.ProcessFrame(im);

                    _marker = new RectanglesMarker(window);
                    _marker.ApplyInPlace(im);

                    _isTracking = true;
                    
                }
                else
                {
                    _isDetecting = true;
                }

                result = im.ToManagedImage();

            } else if (_isTracking)
            {
                UnmanagedImage im = UnmanagedImage.FromManagedImage(sourceBitmap);

                _tracker.ProcessFrame(im);

                var trackingObjectPosition = _tracker.TrackingObject;
                var currentSearchWindow = _tracker.SearchWindow;

                _marker = new RectanglesMarker(currentSearchWindow, trackingObjectPosition.Rectangle);
                _marker.ApplyInPlace(im);

                result = im.ToManagedImage();
            }

            return result;
        }
    }
}
