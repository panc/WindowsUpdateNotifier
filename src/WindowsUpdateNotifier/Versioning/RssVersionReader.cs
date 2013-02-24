using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WindowsUpdateNotifier.Versioning
{
    public class RssVersionReader
    {
        private const string FEED_URL = "";

        public List<RssItem> Execute()
        {
            using (var reader = XmlReader.Create(FEED_URL))
            {
                var doc = new XmlDocument();
                doc.Load(reader);

                return _ParseItems(doc).ToList();
            }
        }

        private IEnumerable<RssItem> _ParseItems(XmlNode doc)
        {
            var nodes = doc.SelectNodes("rss/channel/item");

            foreach (XmlNode node in nodes)
            {
                yield return new RssItem
                {
                    Title = _ParseElement(node, "title"),
                    Link = _ParseElement(node, "link"),
                    Date = _ParseDateTimeElement(node, "pubDate")
                };
            }
        }

        private DateTime _ParseDateTimeElement(XmlNode node, string xPath)
        {
            var value = _ParseElement(node, xPath);
            DateTime date;
            
            return DateTime.TryParse(value, out date)
                ? date
                : DateTime.Now;
        }

        private string _ParseElement(XmlNode parent, string xPath)
        {
            var node = parent.SelectSingleNode(xPath);
            return node != null 
                ? node.InnerText 
                : "";
        }
    }
}