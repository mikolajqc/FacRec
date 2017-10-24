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
        public static Bitmap CreateBitMapFromBytes (double[] vector, int width, int height)
        {
            vector = NormalizeVector(vector, 0, 255);

            Bitmap bitmap = new Bitmap(width,height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    byte intensityOfColor = (byte)vector[y * width + x];
                    bitmap.SetPixel(x, y, Color.FromArgb(intensityOfColor,intensityOfColor,intensityOfColor));

                }
            }

            return bitmap;
        }
        
        public static T[,] GetVectorFromTableInTable<T> (T[,] vectors, int numberOfVector, int positionOfIndex)
        {
            T[,] result = new T[1,vectors.GetLength(positionOfIndex)];

            for (int i = 0; i < 10304; ++i)
            {
                if (positionOfIndex == 1) result[0, i] = vectors[numberOfVector, i];
                else result[0, i] = vectors[i, numberOfVector];

            }

            return result;
        }
        
        public static T[] GetVectorFromTable<T> (T[,] vectors, int numberOfVector)
        {

            T[] result = new T[10304];
            
            for(int i = 0; i < 10304; ++i)
            {
                result[i] = vectors[numberOfVector, i];
            }

            return result;
        }

        public static double[] NormalizeVector(double[] vector, float newMin, float newMax)
        {
            double[] newCollection = new double[vector.Length];

            double oldMax = GetMax(vector);
            double oldMin = GetMin(vector);
            double oldDifference = oldMax - oldMin;
            double newDifference = newMax - newMin;

            for (int i = 0; i < vector.Length; ++i)
            {
                double k = newDifference / oldDifference;
                newCollection[i] = vector[i] * k + (newMin - oldMin*k);
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
