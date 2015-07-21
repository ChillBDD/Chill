using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chill.Http
{
    public class AuthenticateAction : IUserAction
    {
        private readonly HttpClientBuilder _httpClientBuilder;
        private readonly TestUser testUser;

        public AuthenticateAction(TestUser testUser, HttpClientBuilder httpClientBuilder)
        {
            this.testUser = testUser;
            _httpClientBuilder = httpClientBuilder;
        }

        public string Message
        {
            get { return $"User {testUser} logs in"; }
        }

        public IEnumerable<ResponseAction> ResultActions
        {
            get { return Enumerable.Empty<ResponseAction>(); }
        }

        public Task Execute()
        {
            return _httpClientBuilder.Authenticate(testUser);
        }
    }
}