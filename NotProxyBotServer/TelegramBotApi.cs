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

        public async Task<Telegram.Message> SendMessage(
            string chat_id, // required, can be integer 
            string text, // required 
            string parse_mode = null, 
            bool? disable_web_page_preview = null, 
            bool? disable_notification = null, 
            long? reply_to_message_id = null, 
            string reply_markup = null)
        {
            var ub = new UriBuilder(BaseUriForSendMessage);
            ub.AddArgument("chat_id", chat_id);
            ub.AddArgument("text", text);

            if (parse_mode != null)
            {
                ub.AddArgument("parse_mode", parse_mode);
            }
            if (disable_web_page_preview != null)
            {
                ub.AddArgument("disable_web_page_preview", disable_web_page_preview.Value ? "true" : "false");
            }
            if (disable_notification != null)
            {
                ub.AddArgument("disable_notification", disable_notification.Value ? "true" : "false");
            }
            if (reply_to_message_id != null)
            {
                ub.AddArgument("reply_to_message_id", reply_to_message_id.Value);
            }
            if (reply_markup != null)
            {
                ub.AddArgument("reply_markup", reply_markup);
            }

            return await DoGetMethodCall<Telegram.Message>(ub.ToString());
        }

        public async Task<Telegram.Message> RespondToUpdate(
            Telegram.Update update,
            string text, // required 
            string parse_mode = null,
            bool? disable_web_page_preview = null,
            bool? disable_notification = null,
            bool quote_original_message = false,
            string reply_markup = null)
        {
            return await SendMessage(
                chat_id: update.Message.Chat.Id.ToString(), 
                text: text, 
                parse_mode: parse_mode, 
                disable_web_page_preview: disable_web_page_preview, 
                disable_notification: disable_notification, 
                reply_to_message_id: quote_original_message ? (long?)update.Message.MessageId : null,
                reply_markup: reply_markup);
        }

        /*
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
