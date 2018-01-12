using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Client.Utilities;

namespace Client.Models
{
    public class MainModel : IDisposable
    {
        //todo: ogarnac te toolsy zeby nie bylo nadmiarowych castowan
        //todo: ogarnac czy wszedzie potrzebne sa asyncs
        #region fields

        private readonly FaceDetector _faceDetector;
        private readonly CameraManager _cameraManager;
        private readonly FaceRecognitionManager _faceRecognitionManager;

        #endregion

        public MainModel()
        {
            _faceRecognitionManager = new FaceRecognitionManager();
            _faceDetector = new FaceDetector();
            _cameraManager = new CameraManager();
            ImagesToAdd = new List<BitmapImage>();
        }

        public BitmapImage ImageWebcam { get; set; }

        public BitmapImage ImageSnapshot { get; set; }

        public string NameOfUser { get; set; }

        public string ResultOfRecognition { get; set; }

        public List<BitmapImage> ImagesToAdd { get; set; }

        public bool IsLdaSet { get; set; }

        public bool IsCameraAvailable => _cameraManager.IsCameraAvailable();

        public void ClearImagesToAdd()
        {
            ImagesToAdd.Clear();
        }

        public int AddFaceToAddSet()
        {
            var bitmapWithDetectedFace = _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFrame()).Item2;
            if (bitmapWithDetectedFace == null) return 0;
            ImagesToAdd.Add(Tools.BitmapToImageSource(bitmapWithDetectedFace));
            return 1;
        }

        public async Task<int> AddPhotosOfFaces()
        {
            await _faceRecognitionManager.AddFace(ImagesToAdd, NameOfUser);
            return 0;
        }

        public async Task<int> Recognize()
        {
            ResultOfRecognition = await _faceRecognitionManager.Recognize(ImageSnapshot, IsLdaSet);
            return 0;
        }

        public int UpdateBitmapWithDetectedFace()
        {
            var bitmapWithDetectedFace = _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFrame()).Item2;
            if (bitmapWithDetectedFace == null) return 0;
            ImageSnapshot = Tools.BitmapToImageSource(bitmapWithDetectedFace);
            return 1;
        }

        public void UpdateBitmapWithMarkedFace()
        {
            var frame = _cameraManager.GetFrame();
            if (frame != null)
            {
                var bitmapWithMarkedFace =
                    _faceDetector.GetBitmapWithDetectedFace(frame).Item1;
                bitmapWithMarkedFace.RotateFlip(RotateFlipType.RotateNoneFlipX);
                ImageWebcam = Tools.BitmapToImageSource(
                    bitmapWithMarkedFace
                );
            }

        }

        public void ActivateCamera()
        {
            _cameraManager.Init();
            _cameraManager.Start();
        }

        public void Dispose()
        {
            _cameraManager?.Dispose();
        }
    }
}

