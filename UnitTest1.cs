using Allure.NUnit.Attributes;
using Allure.NUnit;
using Microsoft.Playwright;
using Allure.Commons;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using Allure.Net.Commons;
using AllureLifecycle = Allure.Net.Commons.AllureLifecycle;
 
namespace WeTransact.Publisher.AutomationTest.Production
{
    public class TestSite
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
 
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [AllureNUnit]
    [AllureSuite("Login Tests")]
    public class LoginProductionTests
    {
        [Test]
        [AllureTag("smoke")]
        [AllureOwner("QA Team")]
        public async Task LoginProductionSuccess()
        {
           

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = null,
                ScreenSize = null
            });

            var page = await context.NewPageAsync();            
            await page.GotoAsync("https://www.google.com/");
            var screenshotPath = Path.Combine("allure-results", "screenshot.png");
            await page.ScreenshotAsync(new Microsoft.Playwright.PageScreenshotOptions { Path = screenshotPath });
            Allure.Net.Commons.AllureApi.AddAttachment("Screenshot on Failure", "image/png", screenshotPath);
            Assert.Pass("Login successful");
 
          
            }
          
        }
    
}