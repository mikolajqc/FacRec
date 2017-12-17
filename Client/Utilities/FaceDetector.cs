using System;
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

        private Rectangle _intenderRectangle;

        private bool _isTracking;
        private bool _isDetecting;

        private const int DetectFaceBreakInFrames = 100;
        private int _currentNumberOfCalling = 0;

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

        /// <summary>
        /// Method return pair of Bitmaps that contains source bitmap with marked face and bitmap with croped face
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        public Tuple<Bitmap, Bitmap> GetBitmapWithDetectedFace(Bitmap sourceBitmap)
        {
            
            if (++_currentNumberOfCalling > DetectFaceBreakInFrames)
            {
                _isDetecting = true;
                _currentNumberOfCalling = 0;
            }
            

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
                        (int)(0.2f * regions[0].Width),
                        (int)(0.4f * regions[0].Height));

                    _tracker.SearchWindow = window[0];
                    _intenderRectangle = window[0];
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
                UnmanagedImage unmanagedImage = UnmanagedImage.FromManagedImage(sourceBitmap);

                _tracker.ProcessFrame(unmanagedImage);

                var trackingObjectPosition = _tracker.TrackingObject;
                _intenderRectangle = trackingObjectPosition.Rectangle;

                var currentSearchWindow = _tracker.SearchWindow;

                _marker = new RectanglesMarker(currentSearchWindow, trackingObjectPosition.Rectangle);
                _marker.ApplyInPlace(unmanagedImage);

                result = unmanagedImage.ToManagedImage();
            }

            return new Tuple<Bitmap, Bitmap>(result, CropBitmap(sourceBitmap, _intenderRectangle));
        }

        /// <summary>
        /// Method returns croped image (bitmap) under the rectangle. It returns null if rectangle has 0 size
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Bitmap CropBitmap(Bitmap source, Rectangle rectangle)
        {
            if (rectangle.Width == 0 || rectangle.Height == 0) return null;

            Bitmap result = new Bitmap(rectangle.Width, rectangle.Height);
            Graphics graphics = Graphics.FromImage(result);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            graphics.DrawImage(source, 0, 0, rectangle, GraphicsUnit.Pixel);

            return result;
        }
        
    }
}
