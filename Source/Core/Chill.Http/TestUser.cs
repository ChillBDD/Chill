namespace Chill.Http
{
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
        System.Threading.Tasks.Task
        >;


    public class TestUser
    {
        //private HttpClientBuilder _builder;

        public TestUser(string name, string password)
        {
            Password = password;
            Name = name;
        }

        public string Password { get; set; }


        public HttpClient Client { get; private set; }
        public HttpTestScenario Scenario { get; set; }

        public string Name { get; }

        //public AuthenticateAction LogsIn()
        //{
        //    return new AuthenticateAction(this, _builder);
        //}

        public override string ToString()
        {
            return $"User '{Name}'";
        }

        public void Initialize(HttpClientBuilder builder)
        {
            //_builder = builder;
            Client = builder.Build(this);
        }

        // Usage:
        public HttpBasedUserAction Posts(string url, IRequestMessage data, bool checkStatusCodeIsSuccess = true)
        {
            var actionName = data.GetType().Name;
            var message = $"User {Name} posts {actionName}:{data}";
            return new HttpBasedUserAction(message, this, data.Build(url, HttpMethod.Post), checkStatusCodeIsSuccess);
        }

        public HttpBasedUserAction Gets(string url, bool checkStatusCodeIsSuccess = true)
        {
            var message = $"User {Name} gets {url}";
            return new HttpBasedUserAction(message, this, new HttpRequestMessage(HttpMethod.Get, url),
                checkStatusCodeIsSuccess);
        }

        public HttpBasedUserAction<TResponse> Gets<TResponse>(string url, bool checkStatusCodeIsSuccess = true)
        {
            var message = $"User {Name} gets {url}";
            return new HttpBasedUserAction<TResponse>(message, this, new HttpRequestMessage(HttpMethod.Get, url),
                checkStatusCodeIsSuccess);
        }

    }

    public interface IRequestMessage
    {
        HttpRequestMessage Build(string message, HttpMethod method);
    }

    public class FileToUpload : IRequestMessage
    {
        public FileToUpload(byte[] content, string contentType, string fileName, string fileId = null)
        {
            Content = content;
            ContentType = contentType;
            FileName = fileName;
            FileId = fileId;
        }

        public string FileName { get; set; }
        public string FileId { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }

        public HttpRequestMessage Build(string url, HttpMethod method)
        {
            var message = new HttpRequestMessage(method, url)
            {
                Content = new ByteArrayContent(Content)
            };

            message.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            return message;
        }

        public override string ToString()
        {
            return string.Format("{0} {2} ({1} bytes)", FileName, Content.Length, FileId);
        }
    }

    public class MultiPartFileUpload : IRequestMessage
    {
        private readonly FileToUpload[] _filesToUpload;

        public MultiPartFileUpload(params FileToUpload[] filesToUpload)
        {
            _filesToUpload = filesToUpload;
        }

        public HttpRequestMessage Build(string url, HttpMethod method)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, url);
            var content = new MultipartFormDataContent();
            foreach(var file in _filesToUpload)
            {
                var byteArrayContent = new ByteArrayContent(file.Content);
                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(byteArrayContent, file.FileId ?? file.FileName, file.FileName);
            }
            message.Content = content;
            return message;
        }

        public override string ToString()
        {
            return string.Format("MultiPartFormPost: {0}", string.Join(",", _filesToUpload.Select(x => x.ToString())));
        }
    }
}