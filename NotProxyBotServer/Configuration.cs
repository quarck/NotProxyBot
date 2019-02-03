using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace NotProxyBotServer
{
    static class Configuration
    {
        public static string API_KEY = ApiKeys.TELEGRAM_API_KEY;
        public static string SERVER_ROOT {
            get
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length < 1)
                {
                    Console.Error.WriteLine("give a server root at arg0");
                    Environment.Exit(-1);
                }

                Console.WriteLine(args[0]);

                return args[0];
            } 
        }
    }
}
