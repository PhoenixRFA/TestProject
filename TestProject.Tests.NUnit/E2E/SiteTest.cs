using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace TestProject.Tests.NUnit.E2E;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SiteTest : PageTest 
{
    private async Task _goToSite()
    {
        await Page.GotoAsync("https://localhost:7061/index.html");
    }

    private async Task<IResponse> _waitForResponse(string urlRegex)
    {
        IResponse response = await Page.WaitForResponseAsync(
            new Regex(urlRegex),
            new PageWaitForResponseOptions{ Timeout = 3000 }
        );

        return response;
    }
    
    private async Task<IResponse> _login(string email, string password)
    {
        await Page.FillAsync("#login-email", email);
        await Page.FillAsync("#login-password", password);

        await Page.ClickAsync("#login");

        return await _waitForResponse("users");
    }

    private const string UserEmail = "jib@email.com";
    private const string UserPassword = "123456";
    
    [Test]
    public async Task ProductsAreLoaded()
    {
        await _goToSite();
        
        await _waitForResponse("product");
        
        await Expect(Page.GetByText("Product A")).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task LoginIsOk()
    {
        await _goToSite();

        IResponse response = await _login(UserEmail, UserPassword);

        Assert.That(response.Ok, Is.True);
        
        ILocator loginWrap = Page.Locator("#login-wrap");
        await Expect(loginWrap).ToBeHiddenAsync();

        ILocator checkoutWrap = Page.Locator("#checkout-list-wrap");
        await Expect(checkoutWrap).ToBeVisibleAsync();

        ILocator userName = Page.Locator("#user-name");
        await Expect(userName).Not.ToBeEmptyAsync();
    }

    [Test]
    public async Task MakeOrder()
    {
        await _goToSite();
        await _waitForResponse("product");
        await _login(UserEmail, UserPassword);

        ILocator shopWrap = Page.Locator("#shop-list");
        ILocator product1 = shopWrap.GetByText("Product A");
        
        await Expect(product1).ToBeVisibleAsync();
        await product1.ClickAsync();

        ILocator checkoutWrap = Page.Locator("#checkout-list-wrap");
        await Expect(checkoutWrap).ToBeVisibleAsync();

        ILocator checkoutProduct = checkoutWrap.GetByText("Product A");
        await Expect(checkoutProduct).ToBeVisibleAsync();

        string? total = await Page.Locator("#total-label").TextContentAsync();
        Assert.That(total, Is.EqualTo("1000"));
        
        await product1.ClickAsync();
        ILocator checkoutInput = checkoutProduct.Locator("..").Locator("input");
        string value = await checkoutInput.InputValueAsync();
        Assert.That(value, Is.EqualTo("2"));
        
        total = await Page.Locator("#total-label").TextContentAsync();
        Assert.That(total, Is.EqualTo("2000"));

        IResponse response = await Page.RunAndWaitForResponseAsync(async () =>
            {
                ILocator makeOrderBtn = Page.GetByText("Make order");
                await makeOrderBtn.ClickAsync();
            }, new Regex("order"),
            new PageRunAndWaitForResponseOptions { Timeout = 3000 });
        
        Assert.That(response.Ok, Is.True);
    }
}