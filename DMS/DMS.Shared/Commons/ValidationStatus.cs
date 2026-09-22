namespace DMS.Shared.Commons
{
    public class ValidationStatus<T>
    {
        public int ValidItemsCount
        {
            get { return ValidItems.Count; }
        }

        public List<ErrorDescription> ErrorList { get; }

        public bool IsValid
        {
            get { return ErrorList.Count == 0; }
        }
        public List<T> ValidItems { get; set; }

        public ValidationStatus()
        {
            ErrorList = new List<ErrorDescription>();
            ValidItems = new List<T>();
        }

        public void AddError(ErrorDescription error)
        {
            ErrorList.Add(error);
        }
    }
}
