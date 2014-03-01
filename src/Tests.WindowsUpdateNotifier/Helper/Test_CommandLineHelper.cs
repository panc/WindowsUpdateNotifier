using System;
using NUnit.Framework;

namespace WindowsUpdateNotifier
{
    [TestFixture]
    public class Test_CommandLineHelper
    {
        private const string SETTINGS_FILE = @"C:\Temp\settings.xml";

        [Test]
        public void TestUseDefaultSettingsWithShortArgument()
        {
            _TestParams("", true, false, "-d");
        }

        [Test]
        public void TestUseDefaultSettingsWithLongArgument()
        {
            _TestParams("", true, false, "-defaultsettings");
            _TestParams("", true, false, "-defaultSettings");
        }

        [Test]
        public void TestUseDefaultSettingsWithBothArguments()
        {
            _TestParams("", true, false, "-defaultSettings", "-d");
        }

        [Test]
        public void TestCloseAfterCheckWithShortArgument()
        {
            _TestParams("", false, true, "-c");
        }

        [Test]
        public void TestCloseAfterCheckWithLongArgument()
        {
            _TestParams("", false, true, "-closeaftercheck");
            _TestParams("", false, true, "-closeAfterCheck");
        }

        [Test]
        public void TestCloseAfterCheckWithBothArguments()
        {
            _TestParams("", false, true, "-closeAfterCheck", "-c");
        }

        [Test]
        public void TestSettingsFileWithShortArgument()
        {
            _TestParams(SETTINGS_FILE, false, false, @"-s:" + SETTINGS_FILE);
        }

        [Test]
        public void TestSettingsFileWithLongArgument()
        {
            _TestParams(SETTINGS_FILE, false, false, @"-settingsfile:" + SETTINGS_FILE);
            _TestParams(SETTINGS_FILE, false, false, @"-settingsFile:" + SETTINGS_FILE);
        }

        [Test]
        public void TestSettingsFileWithBothArguments()
        {
            _TestParams(SETTINGS_FILE, false, false, @"-settingsfile:" + SETTINGS_FILE, @"-s:" + SETTINGS_FILE);
        }

        [Test]
        public void TestAllArguments()
        {
            _TestParams("", true, true, "-defaultSettings", "-closeAfterCheck");
            _TestParams("", true, true, "-defaultSettings", "-c");
            _TestParams("", true, true, "-d", "-closeAfterCheck");
            _TestParams("", true, true, "-d", "-c");

            _TestParams(SETTINGS_FILE, false, true, "-closeAfterCheck", "-settingsFile:" + SETTINGS_FILE);
            _TestParams(SETTINGS_FILE, false, true, "-c", "-s:" + SETTINGS_FILE);
            _TestParams(SETTINGS_FILE, false, true, "-closeAfterCheck", "-s:" + SETTINGS_FILE);
        }

        [Test]
        public void TestSettingsFileAndDefaultSettingsCanNotUsedTogether()
        {
            var args = new[] { "-defaultSettings", "-settingsFile:" + SETTINGS_FILE };
            Assert.Throws<NotSupportedException>(() => new CommandLineHelper(args));
        }

        private void _TestParams(string settingsFile, bool useDefaultSettings, bool closeAfterCheck, params string[] args)
        {
            var helper = new CommandLineHelper(args);
            Assert.AreEqual(useDefaultSettings, helper.UseDefaultSettings);
            Assert.AreEqual(closeAfterCheck, helper.CloseAfterCheck);
            Assert.AreEqual(settingsFile, helper.SettingsFile);
        }
    }
}