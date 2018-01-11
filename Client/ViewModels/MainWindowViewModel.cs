using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Client.Models;
using Client.Utilities;
using Commons.Consts;

namespace Client
{
    //todo: Zrobic architekture MVVM!!!!
    //todo: Simple IoC Container dla Caliburn.Micro
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
        private bool _isLdaSet;

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
            get { return _imageWebcam; }
            set
            {
                _imageWebcam = value;
                NotifyOfPropertyChange(() => ImageWebcam);
            }
        }

        public BitmapImage ImageSnapshot
        {
            get { return _imageSnapshot; }

            set
            {
                _imageSnapshot = value;
                NotifyOfPropertyChange(() => ImageSnapshot);
            }
        }

        public string NameOfUser
        {
            get { return _nameOfUser; }
            set
            {
                _nameOfUser = value;
                NotifyOfPropertyChange(() => NameOfUser);
            }
        }

        public bool IsLdaSet
        {
            get { return _isLdaSet; }
            set
            {
                _isLdaSet = value; 
                NotifyOfPropertyChange(()=>IsLdaSet);
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
                        var bitmap = _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFrame()).Item2;
                        if (bitmap == null)
                        {
                            MessageBox.Show("Face is not detected corectly. Try again.");
                        }
                        else
                        {
                            _imagesToAdd.Add(Tools.BitmapToImageSource(bitmap));
                            NotifyOfPropertyChange(() => ImagesToAdd);
                        }
                    }));
        }

        public void Snapshot()
        {
            Application.Current.Dispatcher.BeginInvoke(
            new System.Action(
                () =>
                {
                    var bitmap = _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFrame()).Item2;
                    if (bitmap == null)
                    {
                        MessageBox.Show("Face is not detected corectly. Try again.");
                    }
                    else
                    {
                        _imageSnapshot = Tools.BitmapToImageSource(bitmap);
                        NotifyOfPropertyChange(() => ImageSnapshot);
                    }
                }));
        }

        public async void Recognize()
        {
            if (ImageSnapshot == null)
            {
                MessageBox.Show("You need to take a photo first!");
            }
            else
            {
                ResultOfRecognition = string.Empty;
                try
                {
                    NotifyOfPropertyChange(() => IsLdaSet);
                    ResultOfRecognition = await _faceRecognitionManager.Recognize(ImageSnapshot, IsLdaSet);
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show("Error with connection to server address: " + CommonConsts.Client.ServerAddress + " or server error. Check Internet connection.\nDetails: " + e.Message);
                }
            }
        }

        public async void AddFace()
        {
            if (ImagesToAdd.Length == 0)
            {
                MessageBox.Show("You need to take a photo of new face first!");
            }
            else if(string.IsNullOrEmpty(NameOfUser))
            {
                MessageBox.Show("You need to enter username!");
            }
            else
            {
                try
                {
                    await _faceRecognitionManager.AddFace(_imagesToAdd, _nameOfUser);
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show("Error with connection to server address: " + CommonConsts.Client.ServerAddress + " or server error. Check Internet connection.\nDetails: " + e.Message);
                }

                MessageBox.Show("Face Added!");
            }
        }

        #endregion

        #region ovverriddenmethods

        protected override void OnActivate()
        {
            _cameraManager = new CameraManager();
            _faceDetector = new FaceDetector();
            _faceRecognitionManager = new FaceRecognitionManager();

            _timer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = 50
            };
            _timer.Elapsed += (sender, e) => { UpdateImage(); };
            _timer.Start();

            if (_cameraManager.IsCameraAvailable())
            {
                _cameraManager.Init();
                _cameraManager.Start();
            }
            else
            {
                MessageBox.Show("The camera is not available!");
            }
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
            if(_cameraManager.GetFrame() != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new System.Action(
                        () =>
                        {
                            var bitmapWithMarkedFace =
                                _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFrame()).Item1;
                            bitmapWithMarkedFace.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            _imageWebcam = Tools.BitmapToImageSource(
                                bitmapWithMarkedFace
                                );
                            NotifyOfPropertyChange(() => ImageWebcam);
                        }));

            }
        }

        #endregion

    }
}
