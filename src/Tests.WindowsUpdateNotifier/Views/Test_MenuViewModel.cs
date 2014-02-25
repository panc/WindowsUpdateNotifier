using NUnit.Framework;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    [TestFixture]
    public class Test_MenuViewModel
    {
        private MockVersionHelper mVersionHelper;

        [SetUp]
        public void Setup()
        {
            mVersionHelper = new MockVersionHelper
            {
                Copyright = "(C)",
                CurrentVersion = "9.9.9",
                IsNewVersionAvailable = false
            };
        }

        [Test]
        public void InitialState()
        {
            var viewModel = new MenuViewModel(new MockApplication(), mVersionHelper);

            Assert.AreEqual("http://wun.codeplex.com", viewModel.HomepageLink);
            Assert.AreEqual(!mVersionHelper.IsNewVersionAvailable, viewModel.IsLatestVersion);
            Assert.AreEqual(mVersionHelper.IsNewVersionAvailable, viewModel.IsNewVersionAvailable);
            Assert.AreEqual(mVersionHelper.Copyright, viewModel.CopyrightLabel);
            Assert.AreEqual(false, viewModel.IsSearchForUpdatesEnabled);
            Assert.AreEqual(string.Format("Version {0}", mVersionHelper.CurrentVersion), viewModel.VersionLabel);
            Assert.AreEqual(TextResources.ToolTip_NothingFound, viewModel.UpdateStateText);
        }

        [Test]
        public void SetVersionInfo()
        {
            var viewModel = new MenuViewModel(new MockApplication(), mVersionHelper);

            var versionHelper = new MockVersionHelper
            {
                Copyright = "(Copyright)",
                CurrentVersion = "1.1.1",
                IsNewVersionAvailable = true
            };
            
            viewModel.SetVersionInfo(versionHelper);

            Assert.AreEqual(!versionHelper.IsNewVersionAvailable, viewModel.IsLatestVersion);
            Assert.AreEqual(versionHelper.IsNewVersionAvailable, viewModel.IsNewVersionAvailable);
            Assert.AreEqual(versionHelper.Copyright, viewModel.CopyrightLabel);
            Assert.AreEqual(string.Format("Version {0}", versionHelper.CurrentVersion), viewModel.VersionLabel);
        }

        [Test]
        public void OpenSettings()
        {
            var application = new MockApplication();
            var viewModel = new MenuViewModel(application, mVersionHelper);

            viewModel.OpenSettingsCommand.Execute(null);
            Assert.AreEqual(1, application.OpenSettingsCount);
        }

        [Test]
        public void OpenWindowsUpdateControlPanel()
        {
            var application = new MockApplication();
            var viewModel = new MenuViewModel(application, mVersionHelper);

            viewModel.OpenWindowsUpdateControlPanelCommand.Execute(null);
            Assert.AreEqual(1, application.OpenWindowsUpdateControlPanelCount);
        }

        [Test]
        public void OpenDownloadPage()
        {
            var application = new MockApplication();
            var viewModel = new MenuViewModel(application, mVersionHelper);

            viewModel.OpenDownloadPageCommand.Execute(null);
            Assert.AreEqual(1, application.OpenDownloadPageCount);
        }
    }
}