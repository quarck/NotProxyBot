using NotProxyBotServer.Telegram;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NotProxyBotServer
{
    class Program
    {
        static void Main()
        {
            var bot = new TelegramBot(new TelegramBotApi(), 4);
            bot.Start();
            bot.WaitAny();

            Console.ReadLine();
        }
    }
}
