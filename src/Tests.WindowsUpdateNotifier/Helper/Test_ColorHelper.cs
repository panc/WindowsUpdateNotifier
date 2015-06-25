using System.Windows.Media;
using NUnit.Framework;

namespace WindowsUpdateNotifier
{
    [TestFixture]
    public class Test_ColorHelper
    {
        [Test]
        public void GetCorrectWindowsThemeColor()
        {
            var invalidColor = Color.FromArgb(255, 255, 0, 255);

            for (int i = 0; i < 100; i++)
            {
                var color = ColorHelper.GetWindowsThemeBackgroundColor();
                Assert.AreNotEqual(invalidColor, color);
            }
        }
    }
}