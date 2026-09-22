namespace DMS.Shared.Commons
{
    public enum SourceError
    {
        File = 1,
        DB = 2,
        GSMApi = 3,
        DBTransactionError = 4
    }

    public class ErrorDescription
    {
        public string Identificator { get; set; }
        public string Description { get; set; }
        public SourceError SourceError { get; set; }

        public ErrorDescription() { }

        public ErrorDescription(string id, string desc)
        {
            Identificator = id;
            Description = desc;
        }

        public ErrorDescription(string desc)
        {
            Description = desc;
        }
    }
}
