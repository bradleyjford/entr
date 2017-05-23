namespace Entr.Data
{
    public class SortDescriptor
    {
        public SortDescriptor(string propertyName)
        {
            PropertyName = propertyName;
            Direction = SortDirection.Ascending;
        }

        public SortDescriptor(string propertyName, SortDirection direction)
        {
            PropertyName = propertyName;
            Direction = direction;
        }

        public string PropertyName { get; private set; }
        public SortDirection Direction { get; private set; }
    }
}
