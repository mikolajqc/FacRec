﻿namespace Commons.BussinessClasses
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