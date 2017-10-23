using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    public static class Tools
    {
        public static Bitmap CreateBitMapFromBytes (double[] data, int width, int height)
        {
            data = Normalize(data, 0, 255);

            Bitmap bitmap = new Bitmap(width,height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    byte intensityOfColor = (byte)data[y * width + x];
                    bitmap.SetPixel(x, y, Color.FromArgb(intensityOfColor,intensityOfColor,intensityOfColor));

                }
            }

            return bitmap;
        }

        public static double[] GetVectorFromTable(double[,] vectors, int numberOfVector)
        {
            double[] result = new double[10304];
            
            for(int i = 0; i < 10304; ++i)
            {
                result[i] = vectors[numberOfVector, i];
            }

            return result;
        }

        public static double[] Normalize(double[] collection, float newMin, float newMax)
        {
            double[] newCollection = new double[collection.Length];

            double oldMax = GetMax(collection);
            double oldMin = GetMin(collection);
            double oldDifference = oldMax - oldMin;
            double newDifference = newMax - newMin;

            for (int i = 0; i < collection.Length; ++i)
            {
                double k = newDifference / oldDifference;
                newCollection[i] = collection[i] * k + (newMin - oldMin*k);
            }

            return newCollection;
        }

        public static double GetMax(double[] collection)
        {
            double max = double.MinValue;

            foreach(double value in collection)
            {
                if (max < value) max = value;
            }

            return max;
        }

        public static double GetMin(double[] collection)
        {
            double min = double.MaxValue;

            foreach (double value in collection)
            {
                if (min > value) min = value;
            }

            return min;
        }
    }
}
