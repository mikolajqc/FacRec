using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Client.Utilities
{
    public class FaceRecognitionManager
    {
        private readonly RequestManager _requestManager;

        public FaceRecognitionManager()
        {
            _requestManager = new RequestManager();
        }

        public Task<string> Recognize(Bitmap bitmapWithCroppedFace, bool isLdaSet)
        {
            return _requestManager.Recognize(bitmapWithCroppedFace, isLdaSet);
        }

        public async Task<int> AddFace(List<Bitmap> bitmapWithCroppedFacesToAdd, string nameOfUser)
        {
            foreach (var bitmap in bitmapWithCroppedFacesToAdd)
            {
                await _requestManager.AddFace(bitmap, nameOfUser);
            }

            return 0;
        }
    }
}
