namespace ensc_gurps
{
    public class Status
    {
        public string Name { get; set; }
        public float Value { get; set; }

        public Status() {}

        public Status(string name, float value)
        {
            Name = name;
            Value = value;
        }
    }
}
