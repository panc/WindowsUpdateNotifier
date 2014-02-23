using NUnit.Framework;

namespace WindowsUpdateNotifier
{
    [TestFixture]
    public class Test_CommandLineHelper
    {
        [Test]
        public void TestUseDefaultSettingsWithShortArgument()
        {
            _TestParams(true, false, "-d");
        }

        [Test]
        public void TestUseDefaultSettingsWithLongArgument()
        {
            _TestParams(true, false, "-defaultSettings");
        }

        [Test]
        public void TestUseDefaultSettingsWithBothArguments()
        {
            _TestParams(true, false, "-defaultSettings", "-d");
        }

        [Test]
        public void TestCloseAfterCheckWithShortArgument()
        {
            _TestParams(false, true, "-c");
        }

        [Test]
        public void TestCloseAfterCheckWithLongArgument()
        {
            _TestParams(false, true, "-closeAfterCheck");
        }

        [Test]
        public void TestCloseAfterCheckWithBothArguments()
        {
            _TestParams(false, true, "-closeAfterCheck", "-c");
        }

        [Test]
        public void TestBothArguments()
        {
            _TestParams(true, true, "-defaultSettings", "-closeAfterCheck");
            _TestParams(true, true, "-defaultSettings", "-c");
            _TestParams(true, true, "-d", "-closeAfterCheck");
            _TestParams(true, true, "-d", "-c");
        }

        private void _TestParams(bool useDefaultSettings, bool closeAfterCheck, params string[] args)
        {
            var helper = new CommandLineHelper(args);
            Assert.AreEqual(useDefaultSettings, helper.UseDefaultSettings);
            Assert.AreEqual(closeAfterCheck, helper.CloseAfterCheck);
        }
    }
}