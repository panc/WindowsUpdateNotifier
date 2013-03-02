using System;

namespace WindowsUpdateNotifier
{
    public class RssVersionItem
    {
        private const string RSS_TITLE_OLD = "Windows (8) Update Notifier - V";
        private const string RSS_TITLE = "Windows Update Notifier - V";

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
            var index = title.IndexOf(RSS_TITLE_OLD, 0, StringComparison.Ordinal);
            var length = RSS_TITLE_OLD.Length;

            if (index < 0)
            {
                index = title.IndexOf(RSS_TITLE, 0, StringComparison.Ordinal);
                length = RSS_TITLE.Length;
            }

            var version = title.Substring(index + length);

            return version.Substring(0, version.IndexOf(' '));
        }
    }
}