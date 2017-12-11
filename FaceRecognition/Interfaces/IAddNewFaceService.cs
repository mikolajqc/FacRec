using System.Drawing;

namespace FaceRecognition.Interfaces
{
    public interface IAddNewFaceService
    {
        void AddNewFace(Bitmap bitmapWithFace, string name);
    }
}
