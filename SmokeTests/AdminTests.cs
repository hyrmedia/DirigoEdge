using System;
using NUnit.Framework;
using OpenQA.Selenium;
using SmokeTests.Models;

namespace SmokeTests
{
    [TestFixture]
    public class AdminTests : SmokeTestScaffolds
    {
        private UserActions UserActions { get; set; }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            Driver = WebDriverFactory.GetDefaultDriver();

            UserActions = new UserActions(Driver, BaseUrl);
        }

        [Test]
        public void LoginTest()
        {
            var fields = UserActions.NavigateToLoginPage();

            AssertLoginPageFields(fields);

            UserActions.SendLogin(fields, Username, Password);

            NavigateToPath("admin/");

            Assert.AreEqual("Edge Dashboard", Driver.Title);
        }

        private void AssertLoginPageFields(LoginFields fields)
        {
            Assert.IsNotNull(fields.UsernameField);
            Assert.IsNotNull(fields.PasswordField);
            Assert.IsNotNull(fields.SubmitButton);
        }

        [Test]
        [Ignore("No need to fail a build on this until we finish it")]
        public void TestEditContent()
        {
            UserActions.Login(Username, Password);
            NavigateToPath("admin/pages/managecontent/");

            Assert.AreEqual("Manage Content Pages", Driver.Title);
            var link = Driver.Element(".manageTable tr:first-child .title a");

            Assert.IsNotNull(link);

            link.Click();

            Assert.AreEqual("Edit Content", Driver.Title);

            var updateButton = Driver.Element("#SaveContentButton");

            Assert.IsNotNull(updateButton);

            updateButton.Click();

            var noty = Driver.Element(".noty_text");
            var text = noty.Text;

            Assert.IsNotNull(noty);
     //       Assert.AreEqual("Changes saved successfully.", noty.Text);

        }
    }
}
