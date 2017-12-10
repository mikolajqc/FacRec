using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.Interfaces
{
    public interface IAddNewFaceService
    {
        void AddNewFace(Bitmap bitmapWithFace, string name);
    }
}
