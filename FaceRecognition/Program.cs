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
            FacesMatrix allImages = new FacesMatrix();
            allImages.LoadFromDirectory(@"C:\Users\mikolaj.ciesielski\Desktop\Studia\Inżynierka\Databases\AT&T");
        }

    }
}
