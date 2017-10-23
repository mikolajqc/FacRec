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
        public static Bitmap CreateBitMapFromBytes (byte[] data, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width,height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    byte intensityOfColor = data[y * width + x];
                    bitmap.SetPixel(x, y, Color.FromArgb(intensityOfColor,intensityOfColor,intensityOfColor));

                }
            }

            return bitmap;
        }
    }
}
