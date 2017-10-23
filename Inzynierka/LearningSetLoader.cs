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
        public static float[] GetAverageFaceVector(float[,] vectors)
        {
            float[] sumVector = new float[10304];

            for(int j = 0; j < 10304; ++j)
            {
                float sumOfPixelOnOnePosition = 0;

                for (int i = 0; i < 400; ++i)
                {
                    sumOfPixelOnOnePosition += (float)vectors[i, j];
                }

                sumVector[j] = sumOfPixelOnOnePosition / 400f;
            }

            return sumVector;
        }

        public static float[,] GetImagesAsVectorsFromDirectory(string directory)
        {
            float[,] vectors = new float[400,10304];
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
            return vectors;
        }

        public static float[] GetImageVector (string pathToImage)
        {
            Bitmap bitmap = ImageDecoder.DecodeFromFile(pathToImage);
            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bitmap = histogramEqualization.Apply(bitmap);


            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;

            float[] resultVector = new float[width * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color color = bitmap.GetPixel(x, y);
                    
                    float r = color.R;
                    float g = color.G;
                    float b = color.B;
                    float grayscale = (r + g + b) / 3f;

                    resultVector[y * width + x] = grayscale;
                    
                }
            }

            return resultVector;
        }


        public static float[,] GetDifferenceVectors(float[] averageVector, float[,] vectors)
        {
            float[,] differenceVectors = new float[400, 10304];


            for (int numberOfVector = 0; numberOfVector < 400; ++numberOfVector)
            { 

                for (int numberOfPixel = 0; numberOfPixel < 10304; ++numberOfPixel)
                {
                    differenceVectors[numberOfVector, numberOfPixel] = (vectors[numberOfVector, numberOfPixel] - averageVector[numberOfPixel]);
                }

            }

            return differenceVectors;

        }


    }

}
