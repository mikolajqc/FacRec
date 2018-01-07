﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Client.Utilities;

namespace Client.Models
{
    public class FaceRecognitionManager
    {
        private readonly RequestManager _requestManager;

        public FaceRecognitionManager()
        {
            _requestManager = new RequestManager();
        }

        public Task<string> Recognize(BitmapImage bitmapWithCroppedFace)
        {
            return _requestManager.Recognize(Tools.BitmapImage2Bitmap(bitmapWithCroppedFace));
        }

        public async Task<int> AddFace(List<BitmapImage> bitmapWithCroppedFacesToAdd, string nameOfUser)
        {
            foreach (var bitmap in bitmapWithCroppedFacesToAdd)
            {
                await _requestManager.AddFace(Tools.BitmapImage2Bitmap(bitmap), nameOfUser);
            }

            return 0;
        }

    }
}