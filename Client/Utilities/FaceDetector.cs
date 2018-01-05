using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Client.Utilities
{
    //TODO: ogarnac tak zeby sama detekcja robila sie na niskiej rozdzielczosci zdjeciu. A wycinka szla z odpowiednio wyskalowanego zdjecia wysokiej jakosci, cos na zasadzie podgladu
    //TODO: przenies wszystkie stale do configuracji
    class FaceDetector
    {
        private readonly CascadeClassifier _classifier;

        public FaceDetector()
        {
            _classifier = new CascadeClassifier(@"haarcascade_frontalface_alt2.xml");
        }

        /// <summary>
        /// Method return pair of Bitmaps that contains source bitmap with marked face and bitmap with croped face
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        public Tuple<Bitmap, Bitmap> GetBitmapWithDetectedFace(Bitmap sourceBitmap)
        {
            Image<Bgr, byte> currentFrame = new Image<Bgr, byte>(sourceBitmap);

            var grayFrame = currentFrame.Convert<Gray, Byte>();
            var faces = _classifier.DetectMultiScale(grayFrame, 1.1, 10, System.Drawing.Size.Empty);

            foreach (var face in faces)
                currentFrame.Draw(face, new Bgr(Color.BurlyWood));

            Bitmap bitmapWithMarkedFace = new Bitmap(currentFrame.ToBitmap());
            Bitmap bitmapWithCroppedFace = null;

            if (faces.Length > 0)
            {
                currentFrame.ROI = faces[0];
                currentFrame = currentFrame.Copy();
                bitmapWithCroppedFace = new Bitmap(currentFrame.ToBitmap());
            }

            return new Tuple<Bitmap, Bitmap>(bitmapWithMarkedFace, bitmapWithCroppedFace);
        }
        
    }
}
