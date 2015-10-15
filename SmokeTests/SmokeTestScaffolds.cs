using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using NUnit.Framework;
using OpenQA.Selenium;
using static System.String;

namespace SmokeTests
{
    [TestFixture]
    public class SmokeTestScaffolds
    {
        protected IWebDriver Driver;
        protected static String BaseUrl;
        protected static String Username;
        protected static String Password;

        public SmokeTestScaffolds()
        {
            BaseUrl = ConfigurationManager.AppSettings["SmoketestUrl"] ?? "localhost";
            Username = ConfigurationManager.AppSettings["Username"] ?? "Username";
            Password = ConfigurationManager.AppSettings["Password"] ?? "Password";

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