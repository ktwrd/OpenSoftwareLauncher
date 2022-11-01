using System;

namespace OSLCommon
{
    public class HttpException
    {
        public HttpException()
            : this(0, "", null)
            { }
        public HttpException(int code, string message)
            : this(code, message, null)
            { }
        public HttpException(int code, string message, Exception exception = null)
        {
            Code = code;
            Message = message;
            this.Exception = exception?.ToString() ?? "";
        }
        public int Code { get; private set; }
        public string Message { get; private set; }
        public readonly bool Error = true;
        public string Exception { get; private set; }
    }
}
