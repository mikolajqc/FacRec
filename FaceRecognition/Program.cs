using Accord.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{
    class Program
    {
        static void Main(string[] args)
        {
            FaceRecognition fR = new FaceRecognition(@"../../../LearningSet_AT&T");
            fR.Learn();

            Bitmap testBitMap = ImageDecoder.DecodeFromFile(@"../../../LearningSet_AT&T/s1/1.pgm");
            Console.WriteLine("String: " + fR.Recognize(testBitMap));
            Console.WriteLine("Adding new face:");
            fR.AddNewFace(ImageDecoder.DecodeFromFile(@"C:\Users\Mikolaj\Desktop\Studia\putin.jpg"), "putin");
            Console.WriteLine("Recognized: " + fR.Recognize(ImageDecoder.DecodeFromFile(@"C:\Users\Mikolaj\Desktop\Studia\putin.jpg")));
            Console.ReadKey();
        }

    }

}
