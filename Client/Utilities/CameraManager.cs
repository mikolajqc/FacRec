using System.Drawing;
using Accord.Imaging;
using Accord.Imaging.Filters;
using AForge.Video.DirectShow;

namespace Client.Utilities
{
    class CameraManager
    {
        private readonly VideoCaptureDevice _videoSource;
        private readonly FilterInfoCollection _videoDevices;
        private Bitmap _currentBitmap;

        public CameraManager()
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(
            (s, eventArgs)
            =>
            {
                lock (this)
                {
                    ResizeNearestNeighbor resize = new ResizeNearestNeighbor(320,240);
                    UnmanagedImage im = UnmanagedImage.FromManagedImage(eventArgs.Frame);
                    UnmanagedImage downsample = resize.Apply(im);
                    _currentBitmap = downsample.ToManagedImage();
                }

            });
            
        }

        public void Start()
        {
            _videoSource.Start();
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
