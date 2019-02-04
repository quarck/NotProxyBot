using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotProxyBotServer
{
    /*
  <item>
     <title>WRC expected to intervene in nurses&#39; strike</title>
     <link>http://www.rte.ie/news/health/2019/0204/1027438-inmo-nurses-strike/</link>
     <description>The Workplace Relations Commission is expected to intervene in a bid to break the deadlock between unions and management over pay and staff shortages.</description>
     <pubDate>Mon, 4 Feb 2019 16:30:54 +0000</pubDate>
     <guid isPermaLink="true">http://www.rte.ie/news/health/2019/0204/1027438-inmo-nurses-strike/</guid>
     <enclosure url="http://img.rasset.ie/0008f200-144.jpg" type="image/jpeg" length="4094" />
 </item>*/

    public class RssFeedItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Guid { get; set; }
        public string EnclosureUrl { get; set; } // picture 

        public string ToString()
        {
            return Title + " " + Link; 
        }
    }

    public class RssFeed
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime LastBuildDate { get; set; }

        public List<RssFeedItem> Items;

        public string ToString()
        {
            return string.Join(", ", Items.Select(x => x.ToString()));
        }
    }
}
