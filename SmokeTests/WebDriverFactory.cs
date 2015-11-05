using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SmokeTests
{
    public static class WebDriverFactory
    {
        public static IWebDriver GetDefaultDriver()
        {
            return new ChromeDriver();
        }
    }
}
