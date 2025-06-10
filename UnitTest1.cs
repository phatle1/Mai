using Allure.NUnit.Attributes;
using Allure.NUnit;
using Microsoft.Playwright;
using System.Text.Json.Serialization;
using Allure.Net.Commons;
 
namespace WeTransact.Publisher.AutomationTest.Production
{
    [AllureNUnit]
    public class PlaywrightTestBase
    {
        protected IPlaywright? _playwright;
        protected IBrowser? _browser;
        protected IPage? _page;
        protected IBrowserContext? _context;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            _playwright = await Playwright.CreateAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            if (_playwright != null)
            {
                _playwright.Dispose();
            }
        }

        [SetUp]
        public async Task Setup()
        {
            // Configure browser options
            var browserOptions = new BrowserTypeLaunchOptions
            {
                Headless = false, // Set to false for debugging
               
            };

            _browser = await _playwright.Chromium.LaunchAsync(browserOptions);
            
            // Create context with screenshot options
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
               
            });

            _page = await _context.NewPageAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            
        }

       

    }

    // Example test class
    [TestFixture]
    [AllureFeature("Web UI Tests")]
    public class ExampleTests : PlaywrightTestBase
    {
        [Test]
        [AllureStory("test")]
      
        public async Task ExampleTest()
        {            // Navigate to page
            await _page!.GotoAsync("https://www.google.com/");
            // Take a screenshot for documentation
           
            // Your test logic here
            var title = await _page.TitleAsync();
            var resultsDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "allure-results");
            Directory.CreateDirectory(resultsDir);

            var fileName = $"failure-{TestContext.CurrentContext.Test.Name}-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            var screenshotPath = Path.Combine(resultsDir, fileName);

            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });

            // ATTACH the screenshot to Allure. This is the critical line.
            AllureApi.AddAttachment("Failure Screenshot", "image/png", screenshotPath);

            // Debug log
            Console.WriteLine($"Screenshot attached: {screenshotPath}");
            Assert.Fail("failed");
        }
    }
    
}
