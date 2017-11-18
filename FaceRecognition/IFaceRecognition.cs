﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    public interface IFaceRecognition
    {
        string Recognize(Bitmap bitmapWithFace);
        void AddNewFace(Bitmap bitmapWithFace, string name);
        void Learn();

    }
}
