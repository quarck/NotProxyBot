using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotProxyBotServer.Telegram
{
    public class TelegramBot : TelegramBotCore
    {
        public TelegramBot(ITelegramBotApi api, int numWorkers) : base(api, numWorkers)
        {
        }

        internal override void HandleUserMessage(ITelegramBotApi api, Update update)
        {
            HandleUserMessageAsync(api, update).Wait();
        }

        private async Task HandleUserMessageAsync(ITelegramBotApi api, Update update)        
        {
            Telegram.User from = update.Message.From;
            Telegram.Chat chat = update.Message.Chat;
            long userId = update.Message.From.Id;
            string text = update.Message.Text ?? "";


            if (UserState<AuthValidFlag>.ExistsFor(userId))
            {
                var r = await api.RespondToUpdate(update, $"Hello {update.Message.From.ToString()}, I cannot understand {update.Message.Text ?? ""}");
            }
            else
            {
                if (text == ApiKeys.BOT_SECRET_AUTH_KEY)
                {
                    UserState<AuthValidFlag>.LoadOrDefault(userId).Save();
                    var r = await api.RespondToUpdate(update, $"Hello {update.Message.From.ToString()}, you are now welcome");
                }
                else
                {
                    var r = await api.RespondToUpdate(update, $"{update.Message.From.ToString()} ??");
                }
            }
        }
    }
}
