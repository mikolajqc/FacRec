using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.Imaging;
using OpenTK.Graphics.ES30;

namespace Client.Models
{
    public class MainModel
    {
        #region fields
        ///Sprawdzic czy tutaj musi byc BitmapImage czy moze byc Bitmap
        private BitmapImage _imageWebcam;
        private BitmapImage _imageSnapshot;
        private string _nameOfUser;
        private string _resultOfRecognition;
        private System.Timers.Timer _timer;
        private List<BitmapImage> _imagesToAdd = new List<BitmapImage>();
        private bool _isLdaSet;

        private CameraManager _cameraManager;
        private FaceDetector _faceDetector;
        private FaceRecognitionManager _faceRecognitionManager;

        #endregion

        public MainModel()
        {
            _faceRecognitionManager = new FaceRecognitionManager();
            _faceDetector = new FaceDetector();
        }

        public BitmapImage ImageWebcam
        {
            get
            {
                return _imageWebcam;
            }

            set
            {
                _imageWebcam = value;
            }
        }

        public BitmapImage ImageSnapshot
        {
            get
            {
                return _imageSnapshot;
            }

            set
            {
                _imageSnapshot = value;
            }
        }

        public string NameOfUser
        {
            get
            {
                return _nameOfUser;
            }

            set
            {
                _nameOfUser = value;
            }
        }

        public string ResultOfRecognition
        {
            get
            {
                return _resultOfRecognition;
            }

            set
            {
                _resultOfRecognition = value;
            }
        }

        public List<BitmapImage> ImagesToAdd
        {
            get
            {
                return _imagesToAdd;
            }

            set
            {
                _imagesToAdd = value;
            }
        }

        public bool IsLdaSet
        {
            get
            {
                return _isLdaSet;
            }

            set
            {
                _isLdaSet = value;
            }
        }

        public void ClearImagesToAdd()
        {
            _imagesToAdd.Clear();
        }

        public void AddImageToAdd(BitmapImage image)
        {
            _imagesToAdd.Add(image);
        }

        public async Task<int> AddPhotosOfFaces()
        {
            await _faceRecognitionManager.AddFace(_imagesToAdd, _nameOfUser);

            return 0;
        }

        public async Task<string> Recognize()
        {
            return await _faceRecognitionManager.Recognize(_imageSnapshot, _isLdaSet);
        }

        public Bitmap GetBitmapWithDetectedFace(Bitmap source)
        {
            return _faceDetector.GetBitmapWithDetectedFace(source).Item2;
        }

        public Bitmap GetBitmapWithMarkedFace(Bitmap source)
        {
            return _faceDetector.GetBitmapWithDetectedFace(source).Item1;
        }
    }
}
