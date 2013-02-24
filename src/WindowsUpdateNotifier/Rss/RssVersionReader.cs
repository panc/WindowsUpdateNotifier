using System;
using System.Xml;
using System.Collections.ObjectModel;

namespace WindowsUpdateNotifier
{
    /// <summary>
    /// Parses remote RSS 2.0 feeds.
    /// </summary>
    [Serializable]
    public class RssVersionReader : IDisposable
    {

        #region Constructors

        public RssVersionReader()
        { }

        public RssVersionReader(string feedUrl)
        {
            mFeedUrl = feedUrl;
        }

        #endregion

        #region Properties

        private string mFeedUrl;
        /// <summary>
        /// Gets or sets the URL of the RSS feed to parse.
        /// </summary>
        public string FeedUrl
        {
            get { return mFeedUrl; }
            set { mFeedUrl = value; }
        }

        private Collection<RssItem> mItems = new Collection<RssItem>();
        /// <summary>
        /// Gets all the items in the RSS feed.
        /// </summary>
        public Collection<RssItem> Items
        {
            get { return mItems; }
        }

        private string mTitle;
        /// <summary>
        /// Gets the title of the RSS feed.
        /// </summary>
        public string Title
        {
            get { return mTitle; }
        }

        private string mDescription;
        /// <summary>
        /// Gets the description of the RSS feed.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
        }

        private DateTime mLastUpdated;
        /// <summary>
        /// Gets the date and time of the retrievel and
        /// parsing of the remote RSS feed.
        /// </summary>
        public DateTime LastUpdated
        {
            get { return mLastUpdated; }
        }

        private TimeSpan _UpdateFrequenzy;
        /// <summary>
        /// Gets the time before the feed get's silently updated.
        /// Is TimeSpan.Zero unless the CreateAndCache method has been used.
        /// </summary>
        public TimeSpan UpdateFrequenzy
        {
            get { return _UpdateFrequenzy; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the remote RSS feed and parses it.
        /// </summary>
        /// <exception cref="System.Net.WebException" />
        public Collection<RssItem> Execute()
        {
            if (String.IsNullOrEmpty(FeedUrl))
                throw new ArgumentException("The feed url must be set");

            using (XmlReader reader = XmlReader.Create(FeedUrl))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                ParseElement(doc.SelectSingleNode("//channel"), "title", ref mTitle);
                ParseElement(doc.SelectSingleNode("//channel"), "description", ref mDescription);
                ParseItems(doc);

                mLastUpdated = DateTime.Now;

                return mItems;
            }
        }

        /// <summary>
        /// Parses the xml document in order to retrieve the RSS items.
        /// </summary>
        private void ParseItems(XmlDocument doc)
        {
            mItems.Clear();
            XmlNodeList nodes = doc.SelectNodes("rss/channel/item");

            foreach (XmlNode node in nodes)
            {
                RssItem item = new RssItem();
                ParseElement(node, "title", ref item.Title);
                ParseElement(node, "description", ref item.Description);
                ParseElement(node, "link", ref item.Link);

                string date = null;
                ParseElement(node, "pubDate", ref date);
                DateTime.TryParse(date, out item.Date);

                mItems.Add(item);
            }
        }

        /// <summary>
        /// Parses the XmlNode with the specified XPath query 
        /// and assigns the value to the property parameter.
        /// </summary>
        private void ParseElement(XmlNode parent, string xPath, ref string property)
        {
            XmlNode node = parent.SelectSingleNode(xPath);
            if (node != null)
                property = node.InnerText;
            else
                property = "Unresolvable";
        }

        #endregion

        #region IDisposable Members

        private bool mIsDisposed;

        /// <summary>
        /// Performs the disposal.
        /// </summary>
        public void Dispose()
        {
            if (mIsDisposed)
                return;

            mItems.Clear();
            mFeedUrl = null;
            mTitle = null;
            mDescription = null;
            mIsDisposed = true;
        }

        #endregion
    }

    #region RssItem struct

    /// <summary>
    /// Represents a RSS feed item.
    /// </summary>
    [Serializable]
    public struct RssItem
    {
        /// <summary>
        /// The publishing date.
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// The title of the item.
        /// </summary>
        public string Title;

        /// <summary>
        /// A description of the content or the content itself.
        /// </summary>
        public string Description;

        /// <summary>
        /// The link to the webpage where the item was published.
        /// </summary>
        public string Link;
    }

    #endregion
}