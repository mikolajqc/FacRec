using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Tracking;

namespace Client.Utilities
{
    //TODO: ogarnac tak zeby sama detekcja robila sie na niskiej rozdzielczosci zdjeciu. A wycinka szla z odpowiednio wyskalowanego zdjecia wysokiej jakosci, cos na zasadzie podgladu
    class FaceDetector
    {
        private HaarObjectDetector _detector;
        private RectanglesMarker _marker;
        private Camshift _tracker;

        public FaceDetector()
        {
            HaarCascade cascade = new FaceHaarCascade();
            _detector = new HaarObjectDetector(cascade,
                25, ObjectDetectorSearchMode.Single, 1.2f,
                ObjectDetectorScalingMode.GreaterToSmaller);
            _tracker = new Camshift();
        }

        public Bitmap GetBitmapWithDetectedFace(Bitmap sourceBitmap)
        {
            UnmanagedImage im = UnmanagedImage.FromManagedImage(sourceBitmap);
            Rectangle[] regions = _detector.ProcessFrame(im);

            float xscale = im.Width / 640f;
            float yscale = im.Height / 480f;

            //ResizeNearestNeighbor resize = new ResizeNearestNeighbor(640, 480);
            //UnmanagedImage im1 = resize.Apply(im);

            if (regions.Length > 0)
            {
                Console.WriteLine("Face");
                // Will track the first face found
                Rectangle face = regions[0];

                // Reduce the face size to avoid tracking background
                
                Rectangle window = new Rectangle(
                    (int)((regions[0].X + regions[0].Width / 2f) * xscale),
                    (int)((regions[0].Y + regions[0].Height / 2f) * yscale),
                    1, 1);
                

               // window.Inflate(
                //    (int)(0.2f * regions[0].Width * xscale),
                //    (int)(0.4f * regions[0].Height * yscale));

                // Initialize tracker
            //    tracker.SearchWindow = window;
              //  tracker.ProcessFrame(im);

                _tracker.SearchWindow = window;
                _tracker.ProcessFrame(im);

                _marker = new RectanglesMarker(regions);
                //_marker.FillColor = Color.Red;
                _marker.ApplyInPlace(im);

            }

            return im.ToManagedImage();
        }
    }
}
