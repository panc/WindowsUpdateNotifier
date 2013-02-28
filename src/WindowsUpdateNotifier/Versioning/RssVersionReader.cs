using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WindowsUpdateNotifier.Versioning
{
    public class RssVersionReader
    {
        private const string FEED_URL = "http://wun.codeplex.com/project/feeds/rss?ProjectRSSFeed=codeplex%3a%2f%2frelease%2fwun";

        public List<RssVersionItem> Execute()
        {
            using (var reader = XmlReader.Create(FEED_URL))
            {
                var doc = new XmlDocument();
                doc.Load(reader);

                return _ParseItems(doc).ToList();
            }
        }

        private IEnumerable<RssVersionItem> _ParseItems(XmlNode doc)
        {
            var nodes = doc.SelectNodes("rss/channel/item");

            foreach (XmlNode node in nodes)
            {
                yield return new RssVersionItem (
                    _ParseElement(node, "title"),
                    _ParseElement(node, "link"), 
                    _ParseDateTimeElement(node, "pubDate"));
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