using Microsoft.Playwright;
using Xunit;

namespace FullIntegrationTests;

public class Contacts : IClassFixture<TestFixture>
{
    private readonly IPage? _page;
    private readonly TestFixture _fixture;

    public Contacts(TestFixture fixture)
    {
        _fixture = fixture;
        _page = fixture.Page;
    }
    
    [Fact]
    public async Task RemoveContact()
    {
        // Go to the page and wait for the loading to complete
        await _page.GotoAsync(_fixture.ServerAddress);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await _page.Locator(".contact-detail").IsVisibleAsync();
        
        // check that there's one contact
        Assert.Equal(1, await _page.Locator(".contact-detail").CountAsync());
        
        // Delete the contact
        await _page.Locator(".clickable").ClickAsync();
        await _page.GetByRole(AriaRole.Button, new() { NameString = "Confirm" }).ClickAsync();
        
        // Check if the contact is deleted
        await _page.GetByText("Page 1 of 0: showing 0 of 0 items. Previous Next").ClickAsync();
        Assert.Equal(0, await _page.Locator(".contact-detail").CountAsync());
    }
}