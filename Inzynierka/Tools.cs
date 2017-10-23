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
        public static Bitmap CreateBitMapFromBytes (float[] data, int width, int height)
        {
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

        public static float[] GetVectorFromTable(float[,] vectors, int numberOfVector)
        {
            float[] result = new float[10304];
            
            for(int i = 0; i < 10304; ++i)
            {
                result[i] = vectors[numberOfVector, i];
            }

            return result;
        }
    }
}
