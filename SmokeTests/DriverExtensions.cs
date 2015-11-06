using OpenQA.Selenium;

namespace SmokeTests
{
    public static class DriverExtenstions
    {
        public static void NavigateToUrl(this IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);
        }
    }
}