using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons
{
    /// <summary>
    /// Class that is used to send information between client and server
    /// </summary>
    public class Request
    {
        string name;
        byte[] bitmapInArray;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public byte[] BitmapInArray
        {
            get
            {
                return bitmapInArray;
            }

            set
            {
                bitmapInArray = value;
            }
        }
    }
}
