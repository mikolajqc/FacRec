using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Client.Models;
using Client.Utilities;

namespace Client
{
    //todo: Zrobic architekture MVVM!!!!
    //todo: ResizeNearestNeighbor do skalowania? z bilbioteki AForge
    //todo: lista dostepnych urzadzen video
    //todo: Simple IoC Container dla Caliburn.Micro
    //todo: zamiast robic kilka requestow na dodanie twarzy, ogarnac jeden
    //todo: lustrzane odbicie
    public class MainWindowViewModel : Screen
    {
        #region fields
        ///Sprawdzic czy tutaj musi byc BitmapImage czy moze byc Bitmap
        private BitmapImage _imageWebcam;
        private BitmapImage _imageSnapshot;
        private string _nameOfUser;
        private string _resultOfRecognition;
        private System.Timers.Timer _timer;
        private List<BitmapImage> _imagesToAdd = new List<BitmapImage>();

        private CameraManager _cameraManager;
        private FaceDetector _faceDetector;
        private FaceRecognitionManager _faceRecognitionManager;
        #endregion

        #region properties

        public string ResultOfRecognition
        {
            get { return _resultOfRecognition; }
            set
            {
                _resultOfRecognition = value;
                NotifyOfPropertyChange(() => ResultOfRecognition);
            }
        }

        public BitmapImage[] ImagesToAdd
        {
            get { return _imagesToAdd.ToArray(); }
            set
            {
                _imagesToAdd = new List<BitmapImage>(value);
                NotifyOfPropertyChange(() => ImagesToAdd);
            }
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
                NotifyOfPropertyChange(() => ImageWebcam);
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
                NotifyOfPropertyChange(() => ImageSnapshot);
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
                NotifyOfPropertyChange(() => NameOfUser);
            }
        }

        #endregion

        #region publicmethods

        public void Clear()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new System.Action(
                    () =>
                    {
                        _imagesToAdd.Clear();
                        NotifyOfPropertyChange(() => ImagesToAdd);
                    }));
        }

        public void PhotoOfNewFace()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new System.Action(
                    () =>
                    {
                        var bitmap = _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFramePreview()).Item2;
                        if (bitmap != null) _imagesToAdd.Add(Tools.BitmapToImageSource(bitmap));
                        NotifyOfPropertyChange(() => ImagesToAdd);
                    }));
        }

        public void Snapshot()
        {
            Application.Current.Dispatcher.BeginInvoke(
            new System.Action(
                () =>
                {
                    var bitmap = _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFramePreview()).Item2;
                    if (bitmap != null) _imageSnapshot = Tools.BitmapToImageSource(bitmap);
                    NotifyOfPropertyChange(() => ImageSnapshot);
                }));
        }

        public async void Recognize()
        {
            if (ImageSnapshot != null)
            {
                ResultOfRecognition = string.Empty;
                ResultOfRecognition = await _faceRecognitionManager.Recognize(ImageSnapshot);
            }
            else
            {
                MessageBox.Show("You need to take a photo first!");
            }
        }

        public async void AddFace()
        {
            await _faceRecognitionManager.AddFace(_imagesToAdd, _nameOfUser);
            MessageBox.Show("Face Added!");
        }

        #endregion

        #region ovverriddenmethods

        protected override void OnActivate()
        {
            _cameraManager = new CameraManager();
            _faceDetector = new FaceDetector();
            _faceRecognitionManager = new FaceRecognitionManager();;

        _timer = new System.Timers.Timer
                {
                    AutoReset = true,
                    Interval = 50
                };
            // ogarnac to inaczej
            _timer.Elapsed += (sender, e) =>
                {
                    UpdateImage();
                };

            _cameraManager.Start();
            _timer.Start();
        }

        protected override void OnDeactivate(bool close)
        {
            _timer.Stop();
            _cameraManager.Stop();
            base.OnDeactivate(close);
        }

        #endregion
        

        #region privatemethods

        private void UpdateImage()
        {
            if(_cameraManager.GetFramePreview() != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new System.Action(
                        () => {
                                _imageWebcam = Tools.BitmapToImageSource(
                                    _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFramePreview()).Item1
                                );
                            NotifyOfPropertyChange(() => ImageWebcam);
                        }));

            }
        }

        #endregion

    }
}
