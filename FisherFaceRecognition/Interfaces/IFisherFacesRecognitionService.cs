using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisherFaceRecognition.Interfaces
{///todo: wywalic takie osobne interfejsy i wrzucic to do jednego wspolnego czegos z interfejsami
    public interface IFisherFacesRecognitionService
    {
        string Recognize(Bitmap bitmapWithFace);
    }
}
