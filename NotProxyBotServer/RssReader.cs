using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;

namespace NotProxyBotServer
{
    public class RssReader
    {
        /*using System.ServiceModel.Syndication;*/

        private HttpClient _httpClient;

        public RssReader(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<RssFeed> FetchAndParse(string uri)
        {
            XmlDocument document = null; 
            try
            {
                string resp = await _httpClient.GetStringAsync(uri);

                if (!string.IsNullOrEmpty(resp))
                {
                    Console.WriteLine($"Got Xml: {resp}");
                    document = new XmlDocument();
                    document.LoadXml(resp);
                    Console.WriteLine("parse ok");
                }
                else
                {
                    Console.WriteLine("Fetch failed");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.ToString());
                document = null;
            }

            return document != null ? ParseFeed(document) : null;
        }

        public RssFeed ParseFeed(XmlDocument document)
        {
            Console.WriteLine("parse feed enter");
            XmlNode root = document;

            if (!root.HasChildNodes)
                return null;

            XmlNode rss = root["rss"];
            if (rss == null || !rss.HasChildNodes)
                return null;

            XmlNode channel = rss["channel"];
            if (channel == null || !channel.HasChildNodes)
                return null;

            var ret = new RssFeed();
            ret.Items = new List<RssFeedItem>();

            for (int i = 0; i < channel.ChildNodes.Count; i++)
            {
                var child = channel.ChildNodes[i];
                switch (child.Name.ToLower())
                {
                    case "title":
                        ret.Title = child.Value;
                        break;
                    case "link":
                        ret.Link = child.Value;
                        break;
                    case "description":
                        ret.Description = child.Value;
                        break;
                    case "pubDate":
                        //ret.PublicationDate = DateTime.Parse()
                        break;

                    case "lastBuildDate":
                        //ret.LastBuildDate = DateTime.Parse();
                        break;

                    case "item":
                        {
                            var parsed = ParseItem(child);
                            if (parsed != null)
                                ret.Items.Add(parsed);
                            break;
                        }
                }
            }

            return ret;
        }

        private RssFeedItem ParseItem(XmlNode node)
        {
            if (!node.HasChildNodes)
                return null;

            RssFeedItem ret = new RssFeedItem();

            var title = node["title"];
            var description = node["description"];

            if (title == null && description == null)
                return null;

            ret.Title = title?.Value ?? "";
            ret.Description = description?.Value ?? "";
            ret.Link = node["link"]?.Value ?? "";
            //ret?.PublicationDate = ode["pubDate"]... parse...
            ret.Guid = node["guid"]?.Value ?? "";
            ret.EnclosureUrl = node["enclosure"]?.Attributes["url"]?.Value ?? "";

            return ret;        
        }


    }
}
