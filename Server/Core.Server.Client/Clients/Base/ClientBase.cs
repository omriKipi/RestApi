﻿using Core.Server.Client.Interfaces;
using Core.Server.Client.Results;
using Core.Server.Shared.Resources;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server.Client.Clients
{
    public class ClientBase<TResource>
        : IClientBase
        where TResource : Resource
    {
        private const string Authorization = "Authorization";
        private string _token;
        private string _server;

        protected HttpClient Client;

        public string ServerUrl
        {
            get { return _server; }
            set
            {
                Client.BaseAddress = new Uri(value);
                _server = value;
            }
        }
        protected string ApiUrl => ServerUrl + GetApiRoute() + "/";

        protected ClientBase()
        {
            Client = new HttpClient(GetInsecureHandler());
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string Token
        {
            get { return _token; }
            set
            {
                _token = value;
                if (Client.DefaultRequestHeaders.Contains(Authorization))
                    Client.DefaultRequestHeaders.Remove(Authorization);
                Client.DefaultRequestHeaders.Add(Authorization, "Bearer " + value);
            }
        }

        protected virtual string GetApiRoute()
        {
            return typeof(TResource).Name.Replace(nameof(Resource), string.Empty);
        }

        protected Task<ActionResult<T>> SendMethod<T>(HttpMethod httpMethod)
        {
            return SendMethod<T>(string.Empty, httpMethod, null);
        }

        protected Task<ActionResult<T>> SendMethod<T>(HttpMethod httpMethod, string urlSubfix)
        {
            return SendMethod<T>(urlSubfix, httpMethod, null);
        }

        protected Task<ActionResult<T>> SendMethod<T>(HttpMethod httpMethod, object content)
        {
            return SendMethod<T>(string.Empty, httpMethod, content);
        }

        protected async Task<ActionResult> SendMethod(string urlSubfix, HttpMethod httpMethod, object content)
        {
            var response = await SendHttp(urlSubfix, httpMethod, content);
            return await GetResult(response);
        }

        protected async Task<ActionResult<T>> SendMethod<T>(string urlSubfix, HttpMethod httpMethod, object content)
        {
            var response = await SendHttp(urlSubfix, httpMethod, content);
            return await GetResult<T>(response);
        }

        private async Task<HttpResponseMessage> SendHttp(string urlSubfix, HttpMethod httpMethod, object content)
        {
            var request = new HttpRequestMessage(httpMethod, ApiUrl + urlSubfix);
            if (content != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return await Client.SendAsync(request);
        }

        protected Task<ActionResult> SendMethod(HttpMethod httpMethod)
        {
            return SendMethod(string.Empty, httpMethod, null);
        }

        protected Task<ActionResult> SendMethod(string urlSubfix, HttpMethod httpMethod)
        {
            return SendMethod(urlSubfix, httpMethod, null);
        }

        private async Task<ActionResult> GetOkResult(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return new OkResult();
            return await GetResult(response);
        }

        private async Task<ActionResult<T>> GetResult<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return new OkResultWithObject<T>(await response.Content.ReadAsAsync<T>());
            return new OkResultWithObject<T>(await GetResult(response));
        }

        private async Task<ActionResult> GetResult(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return new OkResult();

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    return await GetBadRequestResult(response);
                case HttpStatusCode.NotFound:
                    return new NotFoundResult();
                case HttpStatusCode.Unauthorized:
                    return new UnauthorizedResult();
                default:
                    return new StatusCodeResult((int)response.StatusCode);
            }
        }

        private async Task<ActionResult> GetBadRequestResult(HttpResponseMessage response)
        {
            var jsonValue = await response.Content.ReadAsStringAsync();
            var reason = JsonConvert.DeserializeAnonymousType(jsonValue, new { reason = "" }).reason;
            int.TryParse(reason, out int badRequestReason);
            if (badRequestReason != 0)
                return new BadRequestResult(badRequestReason);
            return JsonConvert.DeserializeObject<ValidationErrorResult>(jsonValue);
        }

        private HttpClientHandler GetInsecureHandler()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (cert.Issuer.Equals("CN=localhost"))
                        return true;
                    return errors == System.Net.Security.SslPolicyErrors.None;
                }
            };
            return handler;
        }
    }
}
