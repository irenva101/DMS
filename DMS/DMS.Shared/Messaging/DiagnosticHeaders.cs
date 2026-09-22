namespace DMS.Shared.Messaging
{
    public static class DiagnosticHeaders
    {
        public const string ActivityName = "DMS.Messaging";

        public const string ActivityId = "activity.id";
        public const string TraceParent = "traceparent";
        public const string TraceState = "tracestate";
        public const string MessageId = "message.id";
        public const string CorrelationId = "correlation.id";

        public const string MessagingSystem = "messaging.system";
        public const string MessagingDestination = "messaging.destination.name";
        public const string MessagingOperation = "messaging.operation";
    }
}
