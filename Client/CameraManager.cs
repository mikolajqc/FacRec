using AForge.Video.DirectShow;
using System.Drawing;

namespace Client
{
    class CameraManager
    {
        private VideoCaptureDevice _videoSource;
        private FilterInfoCollection _videoDevices;
        private Bitmap _currentBitmap;

        public CameraManager()
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(
            (s, eventArgs)
            =>
            {
                _currentBitmap = new Bitmap(eventArgs.Frame);
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
