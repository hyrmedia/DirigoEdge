using OpenQA.Selenium;

namespace SmokeTests.Models
{
    public class LoginFields
    {
        public IWebElement UsernameField { get; set; }
        public IWebElement PasswordField { get; set; }
        public IWebElement SubmitButton { get; set; }
    }
}
