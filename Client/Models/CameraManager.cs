using System.Drawing;
using Accord.Imaging;
using Accord.Imaging.Filters;
using AForge.Video.DirectShow;

namespace Client.Models
{
    class CameraManager
    {
        private VideoCaptureDevice _videoSource;
        private readonly FilterInfoCollection _videoDevices;
        private Bitmap _currentBitmap;
        private bool _isStarted;

        public CameraManager()
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public void Init()
        {
            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _videoSource.NewFrame += (s, eventArgs)
                =>
            {
                lock (this)
                {
                    var resize = new ResizeNearestNeighbor(480, 320);
                    var im = UnmanagedImage.FromManagedImage(eventArgs.Frame);
                    var downsample = resize.Apply(im);
                    _currentBitmap = downsample.ToManagedImage();
                }
            };
        }

        public void Start()
        {
            _videoSource.Start();
            _isStarted = true;
        }

        public Bitmap GetFrame()
        {
            return _currentBitmap;
        }

        public bool IsCameraAvailable()
        {
            if (_videoDevices.Count > 0) return true;
            return false;
        }

        public void Stop()
        {
            if(_isStarted) _videoSource.Stop();
            _isStarted = false;
        }
    }
}
