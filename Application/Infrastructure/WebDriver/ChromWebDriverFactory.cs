using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.WebDriver
{
    using System;
    using Microsoft.Extensions.Logging;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;

    public class ChromWebDriverFactory
    {
        private ILogger _logger;

        public ChromWebDriverFactory(ILogger logger)
        {
            _logger = logger;
        }
        public ChromeDriver CreateDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--lang=en-US");
            options.SetLoggingPreference(LogType.Driver, OpenQA.Selenium.LogLevel.All);


            var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));

           _logger.LogInformation( driver.SessionId.ToString());

            return driver;
        }

        public RemoteWebDriver CreateRemoteDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--lang=en-US");
            options.SetLoggingPreference(LogType.Driver, OpenQA.Selenium.LogLevel.All);

            var driver = new RemoteWebDriver(new Uri("http://chrome:4444/wd/hub"), options.ToCapabilities(), TimeSpan.FromMinutes(0.5));
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));

            return driver;
        }
    }


}
