namespace DMS.Shared.Config.ConfigOptions
{
    public class Otlp
    {
        public bool Enabled { get; set; } = true;
        public string Protocol { get; set; } = "grpc"; // "grpc" or "http/protobuf"
        public string Endpoint { get; set; } = "http://localhost:4317";
        public string? Headers { get; set; }
        public int TimeoutMilliseconds { get; set; } = 10000;

    }
}
