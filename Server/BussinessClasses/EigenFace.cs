using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.BussinessClasses
{
    public class EigenFace
    {
        private int id;
        private string value;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
    }
}