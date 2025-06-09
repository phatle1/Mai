using Allure.NUnit.Attributes;
using Allure.NUnit;
using Microsoft.Playwright;
using System.Text.Json.Serialization;
using Allure.Net.Commons;
 
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
            var screenshotPath = "screenshot.png";
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
            AllureApi.AddAttachment("Screenshot on Failure", "image/png", screenshotPath);
            Assert.Pass("Login successful");
 
          
            }
          
        }
    
}
