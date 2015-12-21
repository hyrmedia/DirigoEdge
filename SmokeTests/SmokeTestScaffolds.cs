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


        /// <summary>
        ///  Excecutes a HttpWebRequest
        /// </summary>
        /// <param name="url">The URL to load</param>
        /// <returns>The HttpWebResponse from executing the request</returns>
        protected static HttpWebResponse GetResponseFromUrl(String url)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            return (HttpWebResponse)webRequest.GetResponse();
        }


        /// <summary>
        /// Navigate to the given path relative to BaseUrl
        /// </summary>
        /// <param name="path"></param>
        protected void NavigateToPath(string path = "")
        {
            Driver.NavigateToUrl(BaseUrl + path);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Driver?.Quit();
        }
    }
}