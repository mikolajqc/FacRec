using System.Drawing;

namespace FaceRecognition.Interfaces
{
    public interface IRecognitonService
    {
        string Recognize(Bitmap bitmapWithFace);
    }
}
