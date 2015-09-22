using System;
using System.Net;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SmokeTests
{
    [TestFixture]
    public class HomePageTests : SmokeTestScaffolds
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 15, 0));
        }

        #region HomePage
        [Test]
        public void LoadHomePage()
        {
            var response = GetResponseFromUrl(BaseUrl);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [Test]
        public void ValidateHomePage()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
            Driver.FindElement(By.CssSelector(".contentPage"));
        }
        #endregion
        #region Blogs
        [Test]
        public void LoadBlogListingPage()
        {
            var response = GetResponseFromUrl(BaseUrl + "blog");
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [Test]
        public void ValidateBlogListing()
        {
            Driver.Navigate().GoToUrl(BaseUrl + "blog");
            Driver.FindElement(By.CssSelector(".blogListing"));
        }
        #endregion
    }
}