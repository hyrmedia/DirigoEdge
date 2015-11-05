using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SmokeTests.Models;

namespace SmokeTests
{
    public class UserActions
    {
        private IWebDriver Driver { get; set; }
        private String BaseUrl { get; set; }

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
            Driver.Navigate().GoToUrl(BaseUrl + "/account/login/");

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
    }
}
