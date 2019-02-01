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
                var updates = await api.GetUpdates(offset: nextOffset, timeout: 60, limit: 1);
                if (updates != null && updates.Count > 0)
                {
                    foreach (var update in updates)
                    {
                        nextOffset = update.UpdateId + 1;
                        if (update.Message == null)
                            continue;

                        Console.WriteLine($"{update.Message.From.ToString()}: {update.Message.Text ?? ""}");
                    }
                }
                else
                {
                    Console.WriteLine("No Updates");
                }

                await Task.Delay(100);
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
