using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SmokeTests
{
    [TestFixture]
    public class AdminTests : SmokeTestScaffolds
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 30));
        }

        public class LoginFields
        {
            public IWebElement UsernameField { get; set; }
            public IWebElement PasswordField { get; set; }
            public IWebElement SubmitButton { get; set; }
        }

        public LoginFields LoadLoginPage()
        {
            Driver.Navigate().GoToUrl(BaseUrl + "/account/login/");

            return new LoginFields
            {
                UsernameField = Driver.FindElement(By.Id("UserName")),
                PasswordField = Driver.FindElement(By.Id("Password")),
                SubmitButton = Driver.FindElement(By.ClassName("btn-default"))
            };
        }

        [Test]
        public void LoginTest()
        {
            var fields = LoadLoginPage();

            AssertLoginPageFields(fields);

            SendLogin(fields);

            Driver.Navigate().GoToUrl(BaseUrl + "admin/");

            Assert.AreEqual("Edge Dashboard", Driver.Title);
        }

        private void AssertLoginPageFields(LoginFields fields)
        {
            Assert.IsNotNull(fields.UsernameField);
            Assert.IsNotNull(fields.PasswordField);
            Assert.IsNotNull(fields.SubmitButton);
        }

        public static void SendLogin(LoginFields fields)
        {
            fields.UsernameField.SendKeys(Username);
            fields.PasswordField.SendKeys(Password);

            fields.SubmitButton.Click();
            Thread.Sleep(2000);
        }


        public void Login()
        {
            var fields = LoadLoginPage();
            SendLogin(fields);
        }

        [Test]
        public void CreatePage()
        {
            Login();
        }
    }
}
