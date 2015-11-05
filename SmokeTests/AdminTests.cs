using System;
using NUnit.Framework;
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
            Driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 30));
            UserActions = new UserActions(Driver, BaseUrl);
        }

        [Test]
        public void LoginTest()
        {
            var fields = UserActions.NavigateToLoginPage();

            AssertLoginPageFields(fields);

            UserActions.SendLogin(fields, Username, Password);

            Driver.Navigate().GoToUrl(BaseUrl + "admin/");

            Assert.AreEqual("Edge Dashboard", Driver.Title);
        }

        private void AssertLoginPageFields(LoginFields fields)
        {
            Assert.IsNotNull(fields.UsernameField);
            Assert.IsNotNull(fields.PasswordField);
            Assert.IsNotNull(fields.SubmitButton);
        }

        public void Login()
        {
            var fields = UserActions.NavigateToLoginPage();
            UserActions.SendLogin(fields, Username, Password);
        }

        [Test]
        public void CreatePage()
        {
            Login();
        }
    }
}
