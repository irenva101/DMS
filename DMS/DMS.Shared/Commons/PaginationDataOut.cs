namespace DMS.Shared.Commons
{
    public class PaginationDataOut<DataOut>
    {
        public int Count { get; set; }

        public List<DataOut> Data { get; set; } = [];

        public PaginationDataOut()
        {
        }
    }
}
