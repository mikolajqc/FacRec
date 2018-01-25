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
    public class MainWindowViewModel : Screen
    {
        #region fields

        private System.Timers.Timer _timer;
        private readonly MainModel _mainModel = new MainModel();

        #endregion

        #region properties

        public string ResultOfRecognition
        {
            get { return _mainModel.ResultOfRecognition; }
            set
            {
                _mainModel.ResultOfRecognition = value;
                NotifyOfPropertyChange(() => ResultOfRecognition);
            }
        }

        public BitmapImage[] ImagesToAdd
        {
            get
            {
                List<BitmapImage> result = new List<BitmapImage>();
                foreach (var bitmap in _mainModel.ImagesToAdd)
                {
                    result.Add(Tools.BitmapToImageSource(bitmap));
                }

                return result.ToArray();
            }
            set
            {
                List<Bitmap> result = new List<Bitmap>();
                foreach (var bitmapImage in value)
                {
                    result.Add(Tools.BitmapImage2Bitmap(bitmapImage));
                }

                _mainModel.ImagesToAdd = result;
                NotifyOfPropertyChange(() => ImagesToAdd);
            }
        }

        public BitmapImage ImageWebcam
        {
            get { return Tools.BitmapToImageSource(_mainModel.ImageWebcam); }
            set
            {
                _mainModel.ImageWebcam = Tools.BitmapImage2Bitmap(value);
                NotifyOfPropertyChange(() => ImageWebcam);
            }
        }

        public BitmapImage ImageSnapshot
        {
            get { return Tools.BitmapToImageSource(_mainModel.ImageSnapshot); }

            set
            {
                _mainModel.ImageSnapshot = Tools.BitmapImage2Bitmap(value);
                NotifyOfPropertyChange(() => ImageSnapshot);
            }
        }

        public string NameOfUser
        {
            get { return _mainModel.NameOfUser; }
            set
            {
                _mainModel.NameOfUser = value;
                NotifyOfPropertyChange(() => NameOfUser);
            }
        }

        public bool IsLdaSet
        {
            get { return _mainModel.IsLdaSet; }
            set
            {
                _mainModel.IsLdaSet = value;
                NotifyOfPropertyChange(() => IsLdaSet);
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
                        _mainModel.ClearImagesToAdd();
                        NotifyOfPropertyChange(() => ImagesToAdd);
                    }));
        }

        public void PhotoOfNewFace()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new System.Action(
                    () =>
                    {
                        if (_mainModel.AddFaceToAddSet() == 0)
                        {
                            MessageBox.Show("Face is not detected corectly. Try again.");
                        }
                        else
                        {
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
                        if (_mainModel.UpdateBitmapWithDetectedFace() == 0)
                        {
                            MessageBox.Show("Face is not detected corectly. Try again.");
                        }
                        else
                        {
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
                    await _mainModel.Recognize();
                    NotifyOfPropertyChange(() => ResultOfRecognition);
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show("Error with connection to server address: " + CommonConsts.Client.ServerAddress +
                                    " or server error. Check Internet connection.\nDetails: " + e.Message);
                }
            }
        }

        public async void AddFace()
        {
            if (ImagesToAdd.Length == 0)
            {
                MessageBox.Show("You need to take a photo of new face first!");
            }
            else if (string.IsNullOrEmpty(NameOfUser))
            {
                MessageBox.Show("You need to enter username!");
            }
            else
            {
                try
                {
                    await _mainModel.AddPhotosOfFaces();
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show("Error with connection to server address: " + CommonConsts.Client.ServerAddress +
                                    " or server error. Check Internet connection.\nDetails: " + e.Message);
                }

                MessageBox.Show("Face Added!");
            }
        }

        #endregion

        #region ovverriddenmethods

        protected override void OnActivate()
        {
            _timer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = 50
            };
            _timer.Elapsed += (sender, e) => { UpdateImage(); };
            _timer.Start();

            if (_mainModel.IsCameraAvailable)
            {
                _mainModel.ActivateCamera();
            }
            else
            {
                MessageBox.Show("The camera is not available!");
            }
        }

        protected override void OnDeactivate(bool close)
        {
            _timer.Stop();
            _mainModel.Dispose();
            base.OnDeactivate(close);
        }

        #endregion


        #region privatemethods

        private void UpdateImage()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new System.Action(
                    () =>
                    {
                        _mainModel.UpdateBitmapWithMarkedFace();
                        NotifyOfPropertyChange(() => ImageWebcam);
                    }));
        }

        #endregion
    }
}
