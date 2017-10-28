using System;
using System.Collections.Generic;
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

            /*
            FacesMatrix allImages = new FacesMatrix();
            allImages.LoadFromDirectory(@"C:\Users\mikolaj.ciesielski\Desktop\Studia\Inżynierka\Databases\AT&T");

            double[] averageVector = allImages.GetAverageVector(1);

            foreach (double i in averageVector) Console.Write(i + " ");

            */
            Console.ReadKey();

        }

    }
}
