namespace Chill.Http
{
    using System.Net.Http;

    public class Response<TResult>
    {
        public Response(HttpResponseMessage responseMessage, TResult result)
        {
            ResponseMessage = responseMessage;
            Result = result;
        }

        public HttpResponseMessage ResponseMessage { get; set; }
        public TResult Result { get; set; }
    }
}