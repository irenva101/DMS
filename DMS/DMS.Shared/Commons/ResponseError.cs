namespace DMS.Shared.Commons
{
    public abstract class ResponseErrorBase
    {
        public ResponseErrorCode ErrorCode { get; set; }

        protected ResponseErrorBase() { }

        protected ResponseErrorBase(ResponseErrorCode code)
        {
            ErrorCode = code;
        }
    }

    public class ResponseError : ResponseErrorBase
    {
        public string Message { get; set; }

        public ResponseError() : base() { }

        public ResponseError(ResponseErrorCode code, string msg) : base(code)
        {
            Message = msg;
        }

        // Without this, wrapped errors print the type name instead of what went wrong.
        public override string ToString() => Message;
    }

    public class ResponseError<T> : ResponseErrorBase
    {
        public IList<T>? Message { get; set; }

        public ResponseError() : base() { }

        public ResponseError(ResponseErrorCode code, IList<T>? data) : base(code)
        {
            Message = data;
        }
    }

    public enum ResponseErrorCode
    {
        //exceptions 1-100
        Exception = 1,

        //database codes 1001-2000
        Database_NotFound = 1000,
        Database_BadData = 1001,
        Database_Unsupported = 1002,

        //other error 4001-5000
        ParsingProblem = 4000,
        SwitchProblem = 4001,
        DeserializeProblem = 4002,
        BadRequest = 4003,
        Unauthorized = 4004,
        Forbidden = 4005,
        ExternalServiceError = 4006,
        Token_Matching = 4007
    }
    public class DefaultErrorResponses
    {
        public static readonly ResponseError ExceptionError = new ResponseError(
            ResponseErrorCode.Exception,
            "An error occurred."
        );
    }

}
