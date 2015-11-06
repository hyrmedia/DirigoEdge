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
        protected IWebDriver Driver { get; set; } 
        protected static String BaseUrl { get; set; } = ConfigurationManager.AppSettings["SmoketestUrl"] ?? "http://localhost/";
        protected static String Username { get; set; } = ConfigurationManager.AppSettings["Username"] ?? "Username";
        protected static String Password { get; set; } = ConfigurationManager.AppSettings["Password"] ?? "Password";

        public SmokeTestScaffolds()
        {
            LogTestParameters();
        }

        private static void LogTestParameters()
        {
            Trace.WriteLine("Testing: " + BaseUrl);
            Trace.WriteLine("Username: " + Username);
            Trace.WriteLine("Password: " + Password);
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