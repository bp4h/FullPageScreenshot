using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CrawlingController : ControllerBase
{
    private readonly PuppeteerService _puppeteerService;

    public CrawlingController(PuppeteerService puppeteerService)
    {
        _puppeteerService = puppeteerService;
    }

    [HttpPost("screenshot")]
    public async Task<IActionResult> TakeScreenshot([FromBody] string url)
    {
        var (fullPageScreenshot, smallScreenshot) = await _puppeteerService.TakeScreenshots(url);

        return new ContentResult
        {
            Content = "data:image/png;base64," + Convert.ToBase64String(fullPageScreenshot),
            ContentType = "image/png"
        };        
    }
}
