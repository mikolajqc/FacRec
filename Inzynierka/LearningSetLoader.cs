using Accord.Imaging.Filters;
using Accord.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    public static class LearningSetLoader
    {
        public static byte[] GetAverageFaceVector(byte[,] vectors)
        {
            byte[] sumVector = new byte[10304];

            for(uint j = 0; j < 10304; ++j)
            {
                int sumOfPixelOnOnePosition = 0;

                for (int i = 0; i < 400; ++i)
                {
                    sumOfPixelOnOnePosition += (int)vectors[i, j];
                }

                sumVector[j] = (byte)(sumOfPixelOnOnePosition / 400);
            }

            return sumVector;
        }

        public static byte[,] GetImagesAsVectorsFromDirectory(string directory)
        {
            byte[,] vectors = new byte[400,10304];
            int i = 0;

            foreach(string dir in Directory.GetDirectories(directory))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file) == ".pgm")
                    {
                        Console.WriteLine(file);
                        var tempVector = GetImageVector(file);

                        for (int k = 0; k < 10304; ++k)
                        {
                            vectors[i,k] = tempVector[k];
                        }

                    }
                    ++i;
                }
            }
            Console.WriteLine("asdasda: "+ vectors[390,5000]);
            return vectors;
        }

        public static byte[] GetImageVector (string pathToImage)
        {
            Bitmap bitmap = ImageDecoder.DecodeFromFile(pathToImage);
            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bitmap = histogramEqualization.Apply(bitmap);


            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;

            byte[] imageInBytes = new byte[width * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color color = bitmap.GetPixel(x, y);
                    
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    int grayscale = (r + g + b) / 3;

                    imageInBytes[y * width + x] = (byte)grayscale;
                    
                }
            }

            return imageInBytes;
        }


        public static byte[,] GetDifferenceVectors(byte[] averageVector, byte[,] vectors)
        {
            byte[,] differenceVectors = new byte[400, 10304];


            for (int numberOfVector = 0; numberOfVector < 400; ++numberOfVector)
            { 

                for (int numberOfPixel = 0; numberOfPixel < 10304; ++numberOfPixel)
                {
                    differenceVectors[numberOfVector, numberOfPixel] = (byte)(vectors[numberOfVector, numberOfPixel] - averageVector[numberOfPixel]);
                }

            }

            return differenceVectors;

        }


    }

}
