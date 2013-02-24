using System;

namespace WindowsUpdateNotifier
{
    /// <summary>
    /// Represents a RSS feed item.
    /// </summary>
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
}