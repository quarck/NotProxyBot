// Online API manual: https://core.telegram.org/bots/api 

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NotProxyBotServer
{
    class TelegramBotApi: IDisposable
    {
        private HttpClient _httpClient = null;

        private static string BaseUriForMethod(string method) =>
            $"https://api.telegram.org/bot{Configuration.API_KEY}/{method}";

        public static readonly string BaseUriForGetUpdates = BaseUriForMethod("getUpdates");
        public static readonly string BaseUriForGetMe = BaseUriForMethod("getMe");
        public static readonly string BaseUriForSendMessage = BaseUriForMethod("sendMessage");
        public static readonly string BaseUriForSendContact = BaseUriForMethod("sendContact");
        public static readonly string BaseUriForGetFile = BaseUriForMethod("getFile");

        public TelegramBotApi()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.telegram.org/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<TResult> DoGetMethodCall<TResult>(string uri)
            where TResult: class 
        {
            Console.WriteLine($"Request: {uri}");

            TResult ret = null;
            try
            {
                string resp = await _httpClient.GetStringAsync(uri);
                
                if (!string.IsNullOrEmpty(resp))
                {
                    Console.WriteLine($"Raw result: {resp}");
                    ret = Telegram.CallResult<TResult>.FromJsonString(resp)?.Result ?? null;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.ToString());
            }

            return ret;
        }

        private static string ConstructRequestUri(string methodBaseUri, List<Tuple<String, String>> arguments)
        {
            if (arguments.Count > 0)
            {
                var sb = new StringBuilder(methodBaseUri);

                bool first = true;
                foreach (var arg in arguments)
                {
                    if (first)
                        sb.Append("?");
                    else
                        sb.Append("&");
                    first = false;

                    sb.Append(Uri.EscapeDataString(arg.Item1));

                    if (arg.Item2 != null)
                    {
                        sb.Append("=");
                        sb.Append(Uri.EscapeDataString(arg.Item2));
                    }
                }

                return sb.ToString();
            }
            else
            {
                return methodBaseUri;
            }
        }


        public async Task<Telegram.User> GetMe()
        {
            return await DoGetMethodCall<Telegram.User>(BaseUriForGetMe);
        }

        public async Task<List<Telegram.Update>> GetUpdates(
            long? offset = null, 
            long? limit = null, 
            long? timeout = null, // in seconds
            List<String> allowed_updates = null 
            )
        {
            var ub = new UriBuilder(BaseUriForGetUpdates);
            if (offset != null)
            {
                ub.AddArgument("offset", offset.Value);
            }
            if (limit != null)
            {
                ub.AddArgument("limit", limit.Value);
            }
            if (timeout != null)
            {
                ub.AddArgument("timeout", timeout.Value);
            }
            if (allowed_updates != null)
            {
                ub.AddArgument("allowed_updates", JsonConvert.SerializeObject(allowed_updates));
            }

            return await DoGetMethodCall<List<Telegram.Update>>(ub.ToString());
        }

        /*
        public static readonly string BaseUriForGetMe = BaseUriForMethod("getMe");
        public static readonly string BaseUriForSendMessage = BaseUriForMethod("sendMessage");
        public static readonly string BaseUriForSendContact = BaseUriForMethod("sendContact");
        public static readonly string BaseUriForGetFile = BaseUriForMethod("getFile");
*/

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TelegramBotApi() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
