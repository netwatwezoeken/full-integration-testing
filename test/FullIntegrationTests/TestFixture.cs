using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using Xunit;

namespace FullIntegrationTests;

public class TestFixture : IAsyncLifetime
{
    private readonly FullIntegrationWebApplicationFactory _factory;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    public TestFixture()
    {
        _factory = new FullIntegrationWebApplicationFactory();
    }
    
    public async Task InitializeAsync()
    {
        await _factory.DatabaseContainer.StartAsync();
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 2000
        });
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions()
        {   
            IgnoreHTTPSErrors = true
        });
        Page = await _context.NewPageAsync();
        _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    public IPage? Page { get; set; }

    public async Task DisposeAsync()
    {
        await _factory.DatabaseContainer.StopAsync();
        if (_context != null)
            await _context.DisposeAsync();
        _playwright?.Dispose();
    }
}