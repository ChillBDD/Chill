namespace Chill.Http
{
    using System;

    public class ResponseAction
    {
        public ResponseAction(string message, Action assert)
        {
            Message = message;
            Assert = assert;
        }

        public string Message { get; set; }
        public Exception Exception { get; set; }
        public Action Assert { get; set; }
    }
}