using System.Drawing;

namespace Commons.Inferfaces.Services
{
    public interface IRecognitonService
    {
        string Recognize(Bitmap bitmapWithFace);
    }
}
