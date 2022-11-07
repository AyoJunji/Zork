namespace Zork.Common
{
    public class Item
    {
        public string Name { get; }

        public string Description { get; }

        public int Value { get; }

        public Item(string name, string description, int value)
        {
            Name = name;
            Description = description;
            Value = value; 
        }
    }
}