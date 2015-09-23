using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SmokeTests
{
    [TestFixture]
    public class FrontEndTests
    {
        private IWebDriver WebDriver { get; set; }
        private String EgdeBaseUrl { get; set; }

        [SetUp]
        public void Setup()
        {
            WebDriver = new ChromeDriver();
            EgdeBaseUrl = "http://localhost/";
        }

        [Test]
        public void LoadHomePage()
        {
           
            WebDriver.Navigate().GoToUrl(EgdeBaseUrl);
            var elem = WebDriver.FindElement(By.Id("Main"));
        }

        public void TearDown()
        {
           WebDriver.Quit();
           WebDriver.Dispose();
        }
    }
}
