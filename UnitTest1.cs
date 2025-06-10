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
                Headless = true, // Set to false for debugging
                SlowMo = 50
            };

            _browser = await _playwright.Chromium.LaunchAsync(browserOptions);
            
            // Create context with screenshot options
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                RecordVideoDir = "videos/", // Optional: record videos
            });

            _page = await _context.NewPageAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            // Take screenshot on test failure
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                await TakeScreenshotOnFailure();
            }

            // Close browser resources
            if (_page != null)
            {
                await _page.CloseAsync();
            }
            
            if (_context != null)
            {
                await _context.CloseAsync();
            }
            
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
        }

        protected async Task TakeScreenshot(string name = "screenshot")
        {
            
            if (_page == null) return;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var testName = TestContext.CurrentContext.Test.Name;
            var fileName = $"{testName}_{name}_{timestamp}.png";
            // Always resolve allure-results relative to the git repo root


            var repoRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".."));
            var resultsDir = Path.Combine(repoRoot, "allure-results");
            Directory.CreateDirectory(resultsDir);
            var screenshotPath = Path.Combine(resultsDir, fileName);

            Console.WriteLine("Saving screenshot to: " + screenshotPath);
            Console.WriteLine("Working directory: " + Directory.GetCurrentDirectory());
            Console.WriteLine($"Screenshot exists after save: {File.Exists(screenshotPath)}");
            
            

            // Ensure directory exists


            // Take screenshot
            await _page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });

            // Attach to Allure report
            AllureApi.AddAttachment(fileName, "image/png", screenshotPath);
            
            TestContext.WriteLine($"Phat is here: {screenshotPath}");
        }

        private async Task TakeScreenshotOnFailure()
        {
            try
            {
                await TakeScreenshot("failure");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Failed to take screenshot: {ex.Message}");
            }
        }

        // Helper method to attach text to Allure report
        protected void AttachTextToAllure(string name, string content)
        {
            AllureApi.AddAttachment(name, "text/plain", content);
        }

        // Helper method to attach file to Allure report
        protected void AttachFileToAllure(string name, string filePath, string mimeType = "application/octet-stream")
        {
            if (File.Exists(filePath))
            {
                AllureApi.AddAttachment(name, mimeType, filePath);
            }
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
        {
            // Navigate to page
            await _page!.GotoAsync("https://www.google.com/");

            // Take a screenshot for documentation
            await TakeScreenshot("page_loaded");

            // Your test logic here
            var title = await _page.TitleAsync();
            Assert.Fail(title, Does.Contain("Example"));

           
        }
    }
    
}
