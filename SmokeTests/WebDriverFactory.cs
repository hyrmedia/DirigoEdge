using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SmokeTests
{
    public static class WebDriverFactory
    {
        public static IWebDriver GetDefaultDriver()
        {
            var driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 30));

            return driver;
        }
    }
}
