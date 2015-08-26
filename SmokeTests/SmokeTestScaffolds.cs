using System;
using System.Net;
using NUnit.Framework;
using OpenQA.Selenium;

namespace SmokeTests
{
    [TestFixture]
    public class SmokeTestScaffolds
    {
        protected IWebDriver Driver;
        protected const String BaseUrl = "http://tfs2013/";

        protected static HttpWebResponse GetResponseFromUrl(String url)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)webRequest.GetResponse();
            return response;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Driver?.Quit();
        }
    }
}
