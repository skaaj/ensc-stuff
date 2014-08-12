namespace ensc_gurps.model.character
{
    public class Class
    {
        public string Name { get; set; }
        public string Brief { get; set; }

        public Class() {}

        public Class(string name, string brief)
        {
            Name = name;
            Brief = brief;
        }
    }
}
