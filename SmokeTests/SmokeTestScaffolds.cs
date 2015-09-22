using System;
using System.Configuration;
using System.Net;
using NUnit.Framework;
using OpenQA.Selenium;

namespace SmokeTests
{
    [TestFixture]
    public class SmokeTestScaffolds
    {
        protected IWebDriver Driver;
        protected static String BaseUrl; 

        public SmokeTestScaffolds()
        {
            BaseUrl  = ConfigurationManager.AppSettings["SmoketestUrl"];
        }

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