using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Consts
{
    public static class CommonConsts
    {
        public const int DefaultWidthOfPicturesOfFace = 104;
        public const int DefaultHeightOfPictureOfFace = 174;
        public const int ErrorToleranceForEigenFaces = Int32.MaxValue; //70000000;
        public const int RequiredNumberOfImagesPerPersonForLearning = 10;
        public const int NumberOfKeyEigenFaces = 100;
        public const string PathToLearningSet = @"D:\Studia\Inzynierka\FaceBase\";
    }
}
