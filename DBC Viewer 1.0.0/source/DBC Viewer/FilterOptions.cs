using System.Globalization;

namespace DBCViewer
{
    class FilterOptions
    {
        public string Column { get; private set; }
        public string Value { get; set; }
        public ComparisonType CompType { get; private set; }

        public FilterOptions(string col, ComparisonType ct, string val)
        {
            Column = col;
            Value = val;
            CompType = ct;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Column, CompType, Value);
        }
    }
}
