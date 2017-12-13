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
        string _name;
        byte[] _bitmapInArray;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public byte[] BitmapInArray
        {
            get
            {
                return _bitmapInArray;
            }

            set
            {
                _bitmapInArray = value;
            }
        }
    }
}
