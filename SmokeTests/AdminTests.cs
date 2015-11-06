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

            Driver.NavigateToUrl(BaseUrl + "admin/");

            Assert.AreEqual("Edge Dashboard", Driver.Title);
        }

        private void AssertLoginPageFields(LoginFields fields)
        {
            Assert.IsNotNull(fields.UsernameField);
            Assert.IsNotNull(fields.PasswordField);
            Assert.IsNotNull(fields.SubmitButton);
        }

        [Test]
        public void CreatePage()
        {
            UserActions.Login(Username, Password);
        }
    }
}
