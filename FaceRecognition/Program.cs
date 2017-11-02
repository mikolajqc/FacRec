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
            FaceRecognition fR = new FaceRecognition(@"C:\Users\mikolaj.ciesielski\Desktop\Studia\Inżynierka\Databases\AT&T");
            fR.Learn();

            Bitmap testBitMap = ImageDecoder.DecodeFromFile(@"C:\Users\mikolaj.ciesielski\Desktop\Studia\Inżynierka\Databases\nonface.jpg");
            Console.WriteLine("String: " + fR.Recognize(testBitMap));
            Console.WriteLine("END");
            Console.ReadKey();
        }

    }

}
