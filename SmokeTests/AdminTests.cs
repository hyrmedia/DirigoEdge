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

        [Test]
        public void Login()
        {
            Driver.Navigate().GoToUrl(BaseUrl + "/account/login/");

            var usernameField = Driver.FindElement(By.Id("UserName"));
            var passwordField = Driver.FindElement(By.Id("Password"));
            var submitButton = Driver.FindElement(By.ClassName("btn-default"));

            Assert.IsNotNull(usernameField);
            Assert.IsNotNull(passwordField);

            usernameField.SendKeys(Username);
            passwordField.SendKeys(Password);

            submitButton.Click();
            Thread.Sleep(2000);
            Driver.Navigate().GoToUrl(BaseUrl + "admin/");
            Assert.AreEqual("Edge Dashboard", Driver.Title);
        }
    }
}
