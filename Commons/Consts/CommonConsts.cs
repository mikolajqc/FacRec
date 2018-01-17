
namespace Commons.Consts
{
    public static class CommonConsts
    {
        public static class Server
        {
            public const int DefaultWidthOfPicturesOfFace = 104;
            public const int DefaultHeightOfPictureOfFace = 174;
            public const int ErrorToleranceForEigenFaces = int.MaxValue;
            public const int RequiredNumberOfImagesPerPersonForLearning = 10;
            public const int NumberOfKeyEigenFaces = 100;
            public const string PathToLearningSet = @"D:\Studia\Inzynierka\FaceBase\";
        }

        public static class Client
        {
            public const string PathToConfigOfCascadeForFaces = @"HaarCascadeFiles/haarcascade_frontalface_alt2.xml";
            public const string PathToConfigOfCascadeForEyes = @"HaarCascadeFiles/haarcascade_eye.xml";
            public const string ServerAddress = "http://localhost";
            public const string RecognitionActionPath = "/api/FacRec/Recognize";
            public const string AddFaceActionPath = "/api/FacRec/AddFace";
        }

    }
}
