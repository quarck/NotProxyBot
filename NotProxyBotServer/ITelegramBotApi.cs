using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotProxyBotServer
{
    public interface ITelegramBotApi
    {
        Task<Telegram.User> GetMe();

        Task<List<Telegram.Update>> GetUpdates(
            long? offset = null,
            long? limit = null,
            long? timeout = null, // in seconds
            List<String> allowed_updates = null
            );

        Task<Telegram.Message> SendMessage(
            string chat_id, // required, can be integer 
            string text, // required 
            string parse_mode = null,
            bool? disable_web_page_preview = null,
            bool? disable_notification = null,
            long? reply_to_message_id = null,
            string reply_markup = null);

        Task<Telegram.Message> RespondToUpdate(
            Telegram.Update update,
            string text, // required 
            string parse_mode = null,
            bool? disable_web_page_preview = null,
            bool? disable_notification = null,
            bool quote_original_message = false,
            string reply_markup = null);
    }
}
