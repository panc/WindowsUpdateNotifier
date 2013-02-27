using System;

namespace WindowsUpdateNotifier
{
    public class RssVersionItem
    {
        private const string RSS_TITLE = "Windows (8) Update Notifier - V";

        public RssVersionItem(string title, string link, DateTime date)
        {
            Date = date;
            Title = title;
            Link = link;

            Version = _GetVersion(title);
        }

        public DateTime Date { get; private set; }

        public string Title { get; private set; }

        public string Version { get; private set; }

        public string Link { get; private set; }

        private string _GetVersion(string title)
        {
            var index = title.IndexOf(RSS_TITLE, 0, StringComparison.Ordinal);
            var version = title.Substring(index + RSS_TITLE.Length);

            return version.Substring(0, version.IndexOf(' '));
        }
    }
}