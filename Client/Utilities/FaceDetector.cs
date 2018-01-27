using System;
using System.Drawing;
using Commons.Consts;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Client.Utilities
{
    internal class FaceDetector
    {
        private readonly CascadeClassifier _faceClassifier;
        private readonly CascadeClassifier _eyesClassifier;

        public FaceDetector()
        {
            _faceClassifier = new CascadeClassifier(CommonConsts.Client.PathToConfigOfCascadeForFaces);
            _eyesClassifier = new CascadeClassifier(CommonConsts.Client.PathToConfigOfCascadeForEyes);
        }

        /// <summary>
        /// Method return pair of Bitmaps that contains source bitmap with marked face and bitmap with croped face
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        public Tuple<Bitmap, Bitmap> GetBitmapWithDetectedFace(Bitmap sourceBitmap)
        {
            Image<Bgr, byte> imageWithCroppedFace = new Image<Bgr, byte>(sourceBitmap);
            Image<Bgr, byte> imageWithMarkedFace = new Image<Bgr, byte>(sourceBitmap);
            Image<Bgr, byte> imageOriginal = new Image<Bgr, byte>(sourceBitmap);

            var grayFrame = imageWithCroppedFace.Convert<Gray, byte>();
            var faces = _faceClassifier.DetectMultiScale(grayFrame, 1.1, 10, Size.Empty);
            grayFrame.Dispose();

            foreach (var face in faces)
                imageWithMarkedFace.Draw(face, new Bgr(Color.BurlyWood));

            // w przypadku, gdy nie znaleziono twarzy
            if (faces.Length < 1)
            {
                return new Tuple<Bitmap, Bitmap>(new Bitmap(imageWithMarkedFace.ToBitmap()), null);
            }

            imageWithCroppedFace.ROI = faces[0];
            imageWithCroppedFace = imageWithCroppedFace.Copy();

            //eyes detection:
            var gray = imageWithCroppedFace.Convert<Gray, byte>();
            var eyes = _eyesClassifier.DetectMultiScale(gray, 1.1, 10, Size.Empty);
            gray.Dispose();

            Rectangle rectangleToCroppFace;

            // w przypadku, gdy nie znaleziono oczu
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

                rectangleToCroppFace = new Rectangle(eyes[0].Left, 0, eyes[1].Right - eyes[0].Left,
                    imageWithCroppedFace.Height);
            }
            else
            {
                var deltaY = (eyes[0].Y + eyes[0].Height / 2) - (eyes[1].Y + eyes[1].Height / 2);
                var deltaX = (eyes[0].X + eyes[0].Width / 2) - (eyes[1].X + eyes[1].Width / 2);
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

                rectangleToCroppFace = new Rectangle(eyes[1].Left, 0, eyes[0].Right - eyes[1].Left,
                    imageWithCroppedFace.Height);
            }

            imageWithCroppedFace.ROI = rectangleToCroppFace;

            imageOriginal.ROI = rectangleToCroppFace;
            imageOriginal = imageOriginal.Copy();
            var croppedBitmap = new Bitmap(imageOriginal.ToBitmap());

            return new Tuple<Bitmap, Bitmap>(new Bitmap(imageWithMarkedFace.ToBitmap()), croppedBitmap);
        }
    }
}
