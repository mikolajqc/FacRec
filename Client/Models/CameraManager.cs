using System.Drawing;
using Accord.Imaging;
using Accord.Imaging.Filters;
using AForge.Video.DirectShow;

namespace Client.Models
{
    class CameraManager
    {
        private readonly VideoCaptureDevice _videoSource;
        private readonly FilterInfoCollection _videoDevices;
        private Bitmap _currentBitmapPreview;
        private Bitmap _currentBitmap;

        public CameraManager()
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _videoSource.NewFrame += (s, eventArgs)
                =>
            {
                lock (this)
                {
                    _currentBitmap = new Bitmap(eventArgs.Frame);
                    var resize = new ResizeNearestNeighbor(480, 320);
                    var im = UnmanagedImage.FromManagedImage(eventArgs.Frame);
                    var downsample = resize.Apply(im);
                    _currentBitmapPreview = downsample.ToManagedImage();
                }
            };
        }

        public void Start()
        {
            _videoSource.Start();
        }

        public Bitmap GetFramePreview()
        {
            return _currentBitmapPreview;
        }

        public Bitmap GetFrame()
        {
            return _currentBitmap;
        }

        public void Stop()
        {
            _videoSource.Stop();
        }
    }
}
