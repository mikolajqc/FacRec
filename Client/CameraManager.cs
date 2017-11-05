using AForge.Video.DirectShow;
using System.Drawing;

namespace Client
{
    class CameraManager
    {
        private VideoCaptureDevice videoSource = null;
        private FilterInfoCollection videoDevices = null;
        private Bitmap currentBitmap = null;

        public CameraManager()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(
            (s, eventArgs)
            =>
            {
                currentBitmap = new Bitmap(eventArgs.Frame);
            });
            
        }

        public void Start()
        {
            videoSource.Start();
        }

        public Bitmap GetFrame()
        {
            return currentBitmap;
        }

        public void Stop()
        {
            videoSource.Stop();
        }
    }
}
