using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Accord.Imaging.Filters;
using Caliburn.Micro;
using Client.Utilities;

namespace Client
{
    //todo: ResizeNearestNeighbor do skalowania? z bilbioteki AForge
    //todo: lista dostepnych urzadzen video
    //todo: Simple IoC Container dla Caliburn.Micro
    //todo: zamiast robic kilka requestow na dodanie twarzy, ogarnac jeden
    public class MainWindowViewModel : Screen
    {
        ///TODO: requests manager
        ///TODO: lustrzane odbicie
        #region fields
        ///Sprawdzic czy tutaj musi byc BitmapImage czy moze byc Bitmap
        private BitmapImage _imageWebcam;
        private BitmapImage _imageSnapshot;
        private CameraManager _cameraManager;
        private FaceDetector _faceDetector;
        private RequestManager _requestManager;

        private string _nameOfUser;

        private System.Timers.Timer _timer;
        
        //temp
        private List<BitmapImage> _imagesToAdd = new List<BitmapImage>();
        #endregion

        #region properties

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

        public void PhotoOfNewFace()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new System.Action(
                    () =>
                    {
                        var bitmap = _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFramePreview()).Item2;
                        if (bitmap != null) _imagesToAdd.Add(BitmapToImageSource(bitmap));
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
                    if (bitmap != null) _imageSnapshot = BitmapToImageSource(bitmap);
                    NotifyOfPropertyChange(() => ImageSnapshot);
                }));
        }

        public async void Recognize()
        {
            string result = await _requestManager.Recognize(BitmapImage2Bitmap(ImageSnapshot));
            MessageBox.Show(result);
        }

        public async void AddFace()
        {
            foreach (BitmapImage bitmapImage in _imagesToAdd)
            {
                await _requestManager.AddFace(BitmapImage2Bitmap(bitmapImage), _nameOfUser);
            }
            MessageBox.Show("Face Added!");
        }

        #endregion

        #region ovverriddenmethods

        protected override void OnActivate()
        {
            _cameraManager = new CameraManager();
            _faceDetector = new FaceDetector();
            _requestManager = new RequestManager();

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
                                _imageWebcam = BitmapToImageSource(
                                    _faceDetector.GetBitmapWithDetectedFace(_cameraManager.GetFramePreview()).Item1
                                );
                            NotifyOfPropertyChange(() => ImageWebcam);
                        }));

            }
        }


        ///todo: Funkcje externalowe przerobic na swoj kod i moze przenisc do jakichs tools? jako static

        /// <summary>
        /// External code!!!
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        
        /// <summary>
        /// External
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                var bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        #endregion

    }
}
