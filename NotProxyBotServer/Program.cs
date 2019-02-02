using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NotProxyBotServer
{
    class Program
    {
        static async Task SendReq()
        {
            var api = new TelegramBotApi();        
            var me = await api.GetMe();
            if (me != null)
            {
                Console.WriteLine("me: {0}", me.ToJsonString());
            }

            long? nextOffset = null;

            for (; ; )
            {
                try
                {
                    var updates = await api.GetUpdates(offset: nextOffset, timeout: 60, limit: 1);
                    if (updates != null && updates.Count > 0)
                    {
                        foreach (var update in updates)
                        {
                            nextOffset = update.UpdateId + 1;
                            if (update.Message == null)
                                continue;

                            Console.WriteLine($"{update.Message.From.ToString()}: {update.Message.Chat.Id} {update.Message.Text ?? ""}");

                            long userId = update.Message.From.Id;
                            string command = update.Message.Text ?? "";


                            if (UserState<AuthValidFlag>.ExistsFor(userId))
                            {
                                var msg = await api.RespondToUpdate(update, $"Hello {update.Message.From.ToString()}, I cannot understand {update.Message.Text ?? ""}");
                            }
                            else
                            {
                                if (command == ApiKeys.BOT_SECRET_AUTH_KEY)
                                {
                                    UserState<AuthValidFlag>.LoadOrDefault(userId).Save();
                                    var msg = await api.RespondToUpdate(update, $"Hello {update.Message.From.ToString()}, you are now welcome");
                                }
                                else
                                {
                                    var msg = await api.RespondToUpdate(update, $"{update.Message.From.ToString()}, you are not authorized to execute comands!");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Updates");
                    }

                    await Task.Delay(100);
                }
                catch (TaskCanceledException ex)
                {
                    await Task.Delay(10*1000);
                }
            }
        }

        static void Main()
        {
            var task = SendReq();
            task.Wait();

            Console.ReadKey();
        }
    }
}
