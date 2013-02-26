using System;

namespace WindowsUpdateNotifier
{
    public class RssVersionItem
    {
        public RssVersionItem(string title, string link, DateTime date)
        {
            Date = date;
            Title = title;
            Version = Title.Substring(0, 5); // todo
            Link = link;
        }

        public DateTime Date { get; private set; }

        public string Title { get; private set; }

        public string Version { get; private set; }

        public string Link { get; private set; }
    }
}