namespace Commons.BussinessClasses
{
    public class EigenFace
    {
        private int _id;
        private string _value;

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
    }
}