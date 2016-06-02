using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Trakkr.YouTrack.Universal
{
    public class Connection : IConnection
    {
        private HttpClient client;
        private readonly string baseAddress;

        private readonly HttpClientHandler clientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            CookieContainer = new CookieContainer()
        };

        public Connection(string host, int port = 80, bool useSsl = false, string path = null)
        {
            var protocol = "http";

            if (useSsl)
            {
                protocol = "https";
            }

            baseAddress = $"{protocol}://{host}:{port}{path}/rest/";
        }

        public void Authenticate(NetworkCredential credentials)
        {
            Authenticate(credentials.UserName, credentials.Password);
        }

        public async void Authenticate(string username, string password)
        {
            Logout();

            var credentials = new JsonObject
            {
                ["username"] = JsonValue.CreateStringValue(username),
                ["password"] = JsonValue.CreateStringValue(password),
            };

            var response = await Post("user/login", credentials);

            if (string.Compare(response.GetObject()["login"].GetString(), "ok", StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                throw new HttpRequestException("Authentication Failed.");
            }

            IsAuthenticated = true;
        }

        public void Logout()
        {
            IsAuthenticated = false;
            clientHandler.CookieContainer = new CookieContainer();
        }

        public bool IsAuthenticated { get; private set; }

        public async Task<string> GetCurrentAuthenticatedUser()
        {
            var userData = await Get("user/current");
            
            // Username FullName Email
            var username = userData.GetObject()["Username"].GetString();

            return username;
        }

        private async Task<IJsonValue> Get(string command)
            => await AsJson(await Client.GetAsync(command));

        private async Task<IJsonValue> Post(string command, IJsonValue data)
            => await Post(command, data.Stringify());

        private async Task<IJsonValue> Post(string command, string data)
            => await AsJson(await Client.PostAsync(command, new StringContent(data)));

        private async Task<JsonValue> AsJson(HttpResponseMessage response)
        {
            JsonValue result;
            if (!JsonValue.TryParse(await CheckResponse(response).ReadAsStringAsync(), out result))
            {
                throw new InvalidDataException("Response of request was invalid Json");
            }

            return result;
        }

        private HttpClient Client
        {
            get
            {
                if (client != null)
                {
                    return client;
                }

                var httpClient = new HttpClient(clientHandler);

                httpClient.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.BaseAddress = new Uri(baseAddress);

                client = httpClient;

                return client;
            }
        }

        private HttpContent CheckResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new HttpRequestException("Insufficient Rights.");
            }

            throw new HttpRequestException($"An error occured; StatusCode: {response.StatusCode}.");
        }
    }
}