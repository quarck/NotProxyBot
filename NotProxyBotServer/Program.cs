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
            new TelegramBot(new TelegramBotApi(), 4).Start();
            for (;;)
                Thread.Sleep(100000);
        }
    }
}
