using System.Drawing;

namespace Commons.Inferfaces.Services
{
    public interface IAddNewFaceService
    {
        void AddNewFace(Bitmap bitmapWithFace, string name, string directPathToLearningSet);
    }
}
