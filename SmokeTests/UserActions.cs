using System;
using System.Threading;
using OpenQA.Selenium;
using SmokeTests.Models;

namespace SmokeTests
{
    public class UserActions
    {
        private IWebDriver Driver { get; }
        private String BaseUrl { get; }

        public UserActions(IWebDriver driver, String baseUrl)
        {
            Driver = driver;
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Navigates to the Account Login Page and returns the fields needed to log in
        /// </summary>
        /// <returns>The Username and Password Fields and Submit Button needed to log in</returns>
        public LoginFields NavigateToLoginPage()
        {
            Driver.NavigateToUrl(BaseUrl + "/account/login/");

            return new LoginFields
            {
                UsernameField = Driver.FindElement(By.Id("UserName")),
                PasswordField = Driver.FindElement(By.Id("Password")),
                SubmitButton = Driver.FindElement(By.ClassName("btn-default"))
            };
        }

        public static void SendLogin(LoginFields fields, String username, String password)
        {
            fields.UsernameField.SendKeys(username);
            fields.PasswordField.SendKeys(password);

            fields.SubmitButton.Click();
            Thread.Sleep(2000);
        }

        public void Login(String username, String password)
        {
            var fields = NavigateToLoginPage();
            SendLogin(fields, username, password);
        }
    }
}
