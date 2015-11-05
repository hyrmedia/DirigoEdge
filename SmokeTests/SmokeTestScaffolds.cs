using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using NUnit.Framework;
using OpenQA.Selenium;

namespace SmokeTests
{
    public class SmokeTestScaffolds
    {
        protected IWebDriver Driver;
        protected static String BaseUrl;
        protected static String Username;
        protected static String Password;

        public SmokeTestScaffolds()
        {
            BaseUrl = ConfigurationManager.AppSettings["SmoketestUrl"] ?? "http://smoketest.qa.dirigodev.com/";
            Username = ConfigurationManager.AppSettings["Username"] ?? "cbelanger";
            Password = ConfigurationManager.AppSettings["Password"] ?? "Password1!";

            Trace.WriteLine("Testing " + BaseUrl);
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