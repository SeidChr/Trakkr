#region License

// Distributed under the BSD License
//   
// YouTrackSharp Copyright (c) 2010-2012, Hadi Hariri and Contributors
// All rights reserved.
//   
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//      * Redistributions of source code must retain the above copyright
//         notice, this list of conditions and the following disclaimer.
//      * Redistributions in binary form must reproduce the above copyright
//         notice, this list of conditions and the following disclaimer in the
//         documentation and/or other materials provided with the distribution.
//      * Neither the name of Hadi Hariri nor the
//         names of its contributors may be used to endorse or promote products
//         derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
//   TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
//   PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL 
//   <COPYRIGHTHOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//   SPECIAL,EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
//   LIMITED  TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
//   DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND  ON ANY
//   THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
//   THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//   

#endregion

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Trakkr.YouTrack.Universal.YouTrackSharp.Admin;
using Trakkr.YouTrack.Universal.YouTrackSharp.Infrastructure;

//using System.Security.Authentication;
//using EasyHttp.Http;
//using EasyHttp.Infrastructure;
//using YouTrackSharp.Admin;
//using YouTrackSharp.Infrastructure;
//using YouTrackSharp.Projects;

namespace Trakkr.YouTrack.Universal
{
    public class Connection : IConnection
    {
        private readonly IUriConstructor uriConstructor;
        private CookieCollection authenticationCookie;
        private string username;

        private HttpClient client;

        public Connection(string host, int port = 80, bool useSsl = false, string path = null)
        {
            var protocol = "http";

            if (useSsl)
            {
                protocol = "https";
            }

            uriConstructor = new DefaultUriConstructor(protocol, host, port, path);
        }

        public HttpStatusCode HttpStatusCode { get; private set; }

        public bool PostXml(string command, string xmlData)
        {
            //// This actually doesn't return Application/XML...Bug in YouTrack
            //var httpRequest = MakePostXmlRequest(command, xmlData, HttpContentTypes.ApplicationXml);

            //return httpRequest.Response;
        }

        HttpClient MakePostXmlRequest(string command, object data, string accept)
        {
            //var httpRequest = Client();

            //httpRequest.Request.Accept = accept;

            //httpRequest.Post(uriConstructor.ConstructBaseUri(command), data,
            //                 HttpContentTypes.ApplicationXml);

            //HttpStatusCode = httpRequest.Response.StatusCode;

            //return httpRequest;
        }

        public T Get<T>(string command)
        {
            //var httpRequest = Client();


            //try
            //{
            //    var staticBody = httpRequest.Get(uriConstructor.ConstructBaseUri(command)).StaticBody<T>();

            //    HttpStatusCode = httpRequest.Response.StatusCode;

            //    return staticBody;
            //}
            //catch (HttpException httpException)
            //{
            //    if (httpException.StatusCode == HttpStatusCode.Forbidden)
            //    {
            //        throw new InvalidRequestException("Not enogh Rights");
            //    }
            //    throw;
            //}
        }

        public IEnumerable<TInternal> Get<TWrapper, TInternal>(string command) where TWrapper : class, IDataWrapper<TInternal>
        {
            //var response = Get<TWrapper>(command);

            //if (response != null)
            //{
            //    return response.Data;
            //}
            //return new List<TInternal>();
        }


        public void PostFile(string command, string path)
        {
            //var httpRequest = Client();

            //httpRequest.Request.Accept = HttpContentTypes.ApplicationXml;


            //var contentType = GetFileContentType(path);

            //var files = new List<FileData>() { new FileData() { FieldName = "file", Filename = path, ContentTransferEncoding = "binary", ContentType = contentType } };


            //httpRequest.Post(uriConstructor.ConstructBaseUri(command), null, files);
            //HttpStatusCode = httpRequest.Response.StatusCode;
        }

        public void Head(string command)
        {
            //var httpRequest = Client();

            //httpRequest.Head(uriConstructor.ConstructBaseUri(command));
            //HttpStatusCode = httpRequest.Response.StatusCode;
        }

        public void Put(string command, object data)
        {
            //var httpRequest = Client();

            //httpRequest.Request.Accept = HttpContentTypes.ApplicationXml;

            //httpRequest.Put(uriConstructor.ConstructBaseUri(command), data,
            //                 HttpContentTypes.ApplicationXWwwFormUrlEncoded);

            //HttpStatusCode = httpRequest.Response.StatusCode;
        }

        public void Delete(string command)
        {
            //var httpRequest = Client();

            //httpRequest.Request.Accept = HttpContentTypes.ApplicationXml;

            //var constructBaseUri = uriConstructor.ConstructBaseUri(command);
            //httpRequest.Delete(constructBaseUri);

            //HttpStatusCode = httpRequest.Response.StatusCode;
        }

        public void Post(string command, object data)
        {
            // This actually doesn't return Application/XML...Bug in YouTrack
            //MakePostRequest(command, data, HttpContentTypes.ApplicationXml);
        }

        public dynamic Post(string command, object data, string accept)
        {
            //var httpRequest = MakePostRequest(command, data, accept);

            //return httpRequest.Response.DynamicBody;
        }

        public void Authenticate(NetworkCredential credentials)
        {
            Authenticate(credentials.UserName, credentials.Password);
        }

        public void Authenticate(string username, string password)
        {
            IsAuthenticated = false;
            this.username = String.Empty;
            authenticationCookie = null;

            dynamic credentials = new ExpandoObject();

            credentials.login = username;
            credentials.password = password;

            //try
            //{
            //    HttpClient response = MakePostRequest("user/login", credentials, HttpContentTypes.ApplicationXml);

            //    if (response.Response.StatusCode == HttpStatusCode.OK)
            //    {
            //        if (String.Compare(response.Response.DynamicBody.login, "ok", StringComparison.CurrentCultureIgnoreCase) != 0)
            //        {
            //            throw new AuthenticationException("Authentication Failed");
            //        }
            //        IsAuthenticated = true;
            //        authenticationCookie = response.Response.Cookies;
            //        this.username = username;
            //    }
            //    else
            //    {
            //        throw new AuthenticationException(response.Response.StatusDescription);
            //    }
            //}
            //catch (HttpException exception)
            //{
            //    throw new AuthenticationException(exception.StatusDescription);
            //}
        }

        public void Logout()
        {
            IsAuthenticated = false;
            username = null;
            authenticationCookie = null;
        }

        public bool IsAuthenticated { get; private set; }

        public IUser GetCurrentAuthenticatedUser()
        {

            //var user = Get<User>("user/current");

            //if (user != null)
            //{
            //    user.Username = username;

            //    return user;
            //}

            return null;
        }

        public async Task<JsonValue> Get(string command)
        {

            var response = await Client.GetAsync(uriConstructor.ConstructBaseUri(command));
            response.EnsureSuccessStatusCode();
            var jsonValue = JsonValue.Parse(await response.Content.ReadAsStringAsync());
            HttpStatusCode = response.StatusCode;

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new HttpRequestException("Insufficient Rights.");
                }
                else
                {
                    throw new HttpRequestException("Unable to get data.");
                }
            }

            return jsonValue;

        }

        //string GetFileContentType(string filename)
        //{
        //var mime = "application/octetstream";
        //var extension = Path.GetExtension(filename);
        //if (extension != null)
        //{
        //    var ext = extension.ToLower();
        //    var rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
        //    if (rk != null && rk.GetValue("Content Type") != null)
        //        mime = rk.GetValue("Content Type").ToString();
        //}
        //return mime;
        //}

        HttpClient MakePostRequest(string command, object data, string accept)
        {
            var httpRequest = Client();

            //httpRequest.Request.Accept = accept;

            //httpRequest.Post(uriConstructor.ConstructBaseUri(command), data,
            //                 HttpContentTypes.ApplicationXWwwFormUrlEncoded);

            //HttpStatusCode = httpRequest.Response.StatusCode;

            return httpRequest;
        }


        HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    var cookieContainer = new CookieContainer();
                    var handler = new HttpClientHandler()
                    {
                        CookieContainer = cookieContainer
                    };

                    var httpClient = new HttpClient(handler);

                    httpClient
                        .DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client = httpClient;
                }

                return client;
            }
        }


    }
}