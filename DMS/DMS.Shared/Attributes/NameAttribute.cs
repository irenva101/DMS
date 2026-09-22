namespace DMS.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NameAttribute : Attribute
    {
        public string[] Names { get; }

        public NameAttribute(params string[] names)
        {
            Names = names;
        }
    }
}
