using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Client.Models
{
    //TODO: ogarnac tak zeby sama detekcja robila sie na niskiej rozdzielczosci zdjeciu. A wycinka szla z odpowiednio wyskalowanego zdjecia wysokiej jakosci, cos na zasadzie podgladu
    //TODO: przenies wszystkie stale do configuracji
    class FaceDetector
    {
        private const int Width = 104;
        private const int Height = 174;

        private readonly CascadeClassifier _faceClassifier;
        private readonly CascadeClassifier _eyesClassifier;

        public FaceDetector()
        {
            _faceClassifier = new CascadeClassifier(@"HaarCascadeFiles/haarcascade_frontalface_alt2.xml");
            _eyesClassifier = new CascadeClassifier(@"HaarCascadeFiles/haarcascade_eye.xml");
        }

        /// <summary>
        /// Method return pair of Bitmaps that contains source bitmap with marked face and bitmap with croped face
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        public Tuple<Bitmap, Bitmap> GetBitmapWithDetectedFace(Bitmap sourceBitmap)
        {
            bool isFaceCorrect = true;

            Image<Bgr, byte> imageWithCroppedFace = new Image<Bgr, byte>(sourceBitmap);
            Image<Bgr, byte> imageWithMarkedFace = new Image<Bgr, byte>(sourceBitmap);
            Image<Bgr, byte> imageOriginal = new Image<Bgr, byte>(sourceBitmap);

            var grayFrame = imageWithCroppedFace.Convert<Gray, Byte>();
            var faces = _faceClassifier.DetectMultiScale(grayFrame, 1.1, 10, Size.Empty);

            foreach (var face in faces)
                imageWithMarkedFace.Draw(face, new Bgr(Color.BurlyWood));

            if (faces.Length < 1)
            {
                return new Tuple<Bitmap, Bitmap>(new Bitmap(imageWithMarkedFace.ToBitmap()), null);
            }

            imageWithCroppedFace.ROI = faces[0];
            imageWithCroppedFace = imageWithCroppedFace.Copy();
            // imageWithCroppedFace = imageWithCroppedFace.Resize(2*imageWithCroppedFace.Width, 2 * imageWithCroppedFace.Height, Emgu.CV.CvEnum.Inter.Linear);

            //eyes detection:
            var gray = imageWithCroppedFace.Convert<Gray, Byte>();
            var eyes = _eyesClassifier.DetectMultiScale(gray, 1.1, 10, Size.Empty);

            //  foreach (var eye in eyes)
            //       imageWithCroppedFace.Draw(eye, new Bgr(Color.BlueViolet), 2);

            Rectangle rectangleToCroppFace;

            //utworzenie rectangla do wyciecia twarzy
            if (eyes.Length < 2)
            {
                return new Tuple<Bitmap, Bitmap>(new Bitmap(imageWithMarkedFace.ToBitmap()), null);
            }


            if (eyes[0].Left < eyes[1].Left)
            {

                var deltaY = (eyes[1].Y + eyes[1].Height / 2) - (eyes[0].Y + eyes[0].Height / 2);
                var deltaX = (eyes[1].X + eyes[1].Width / 2) - (eyes[0].X + eyes[0].Width / 2);
                double degrees = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
                if (Math.Abs(degrees) < 35)
                {
                    imageWithCroppedFace = imageWithCroppedFace.Rotate(-degrees, new Bgr(), true);
                    imageOriginal = imageOriginal.Rotate(-degrees,
                        new PointF(faces[0].X + faces[0].Width / 2, faces[0].Y + faces[0].Height / 2),
                        Emgu.CV.CvEnum.Inter.Linear, new Bgr(), true);

                    imageOriginal.ROI = faces[0];
                    imageOriginal = imageOriginal.Copy();
                }

                rectangleToCroppFace = new Rectangle(eyes[0].Left, 0, eyes[1].Right - eyes[0].Left, imageWithCroppedFace.Height);
            }
            else
            {


                var deltaY = (eyes[0].Y + eyes[0].Height / 2) - (eyes[1].Y + eyes[1].Height / 2);
                var deltaX = (eyes[0].X + eyes[0].Width / 2) - (eyes[1].X + eyes[1].Width / 2);
                double degrees = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
                if (Math.Abs(degrees) < 35)
                {
                    //   degrees = 10;
                    imageWithCroppedFace = imageWithCroppedFace.Rotate(-degrees, new Bgr(), true);
                    imageOriginal = imageOriginal.Rotate(-degrees,
                        new PointF(faces[0].X + faces[0].Width / 2, faces[0].Y + faces[0].Height / 2),
                        Emgu.CV.CvEnum.Inter.Linear, new Bgr(), true);

                    imageOriginal.ROI = faces[0];
                    imageOriginal = imageOriginal.Copy();
                }
                rectangleToCroppFace = new Rectangle(eyes[1].Left, 0, eyes[0].Right - eyes[1].Left, imageWithCroppedFace.Height);
            }

            imageWithCroppedFace.ROI = rectangleToCroppFace;
            imageWithCroppedFace = imageWithCroppedFace.Copy();


            imageOriginal.ROI = rectangleToCroppFace;
            imageOriginal = imageOriginal.Copy();
            var croppedBitmap = new Bitmap(imageOriginal.ToBitmap());

            return new Tuple<Bitmap, Bitmap>(new Bitmap(imageWithMarkedFace.ToBitmap()), croppedBitmap);
        }
        
    }
}
